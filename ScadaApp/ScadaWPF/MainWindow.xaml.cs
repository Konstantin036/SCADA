using DataConcentrator.Core;
using DataConcentrator.Models;
using ScadaWPF.Helpers;
using ScadaWPF.Views;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ScadaWPF
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Tag> _tags;
        private ObservableCollection<ActivatedAlarm> _alarms;
        private DataConcentrator.Core.DataConcentrator _dc;

        public MainWindow()
        {
            InitializeComponent();
            _dc = DataConcentrator.Core.DataConcentrator.Instance;
            _tags = new ObservableCollection<Tag>(_dc.GetAllTags());
            _alarms = new ObservableCollection<ActivatedAlarm>();

            dgTags.ItemsSource = _tags;
            dgAlarms.ItemsSource = _alarms;

            _dc.AlarmRaised += OnAlarmRaised;
            Logger.Log("APP_START", "SCADA application started");
        }

        private void OnAlarmRaised(int alarmId)
        {
            Dispatcher.Invoke(() =>
            {
                using (var ctx = new DataConcentrator.Database.ScadaContext())
                {
                    var alarm = ctx.ActivatedAlarms.Find(alarmId);
                    if (alarm != null)
                    {
                        _alarms.Add(alarm);
                        Logger.Log("ALARM_RAISED", $"Tag: {alarm.TagName}, Message: {alarm.Message}");
                    }
                }
            });
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddWindow();
            if (addWindow.ShowDialog() == true)
            {
                _tags.Clear();
                foreach (var tag in _dc.GetAllTags())
                    _tags.Add(tag);
                Logger.Log("TAG_ADDED");
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            var tag = (sender as Button)?.DataContext as Tag;
            if (tag == null) return;

            var result = MessageBox.Show($"Remove tag '{tag.TagName}'?",
                "Confirm", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                _dc.RemoveTag(tag.TagName);
                _tags.Remove(tag);
                Logger.Log("TAG_REMOVED", $"Tag: {tag.TagName}");
            }
        }

        private void btnDetails_Click(object sender, RoutedEventArgs e)
        {
            var tag = (sender as Button)?.DataContext as AnalogInput;
            if (tag == null)
            {
                MessageBox.Show("Details available only for Analog Input tags.");
                return;
            }
            var detailsWindow = new DetailsWindow(tag);
            detailsWindow.ShowDialog();
        }

        private void chkScan_Click(object sender, RoutedEventArgs e)
        {
            var chk = sender as CheckBox;
            var tag = chk?.DataContext as Tag;
            if (tag == null) return;

            _dc.ToggleScan(tag.TagName, chk.IsChecked == true);
            Logger.Log("SCAN_TOGGLE", $"Tag: {tag.TagName}, Scanning: {chk.IsChecked}");
        }

        private void btnAcknowledge_Click(object sender, RoutedEventArgs e)
        {
            var alarm = (sender as Button)?.DataContext as ActivatedAlarm;
            if (alarm == null) return;

            alarm.State = AlarmState.Acknowledged;
            using (var ctx = new DataConcentrator.Database.ScadaContext())
            {
                var dbAlarm = ctx.ActivatedAlarms.Find(alarm.Id);
                if (dbAlarm != null)
                {
                    dbAlarm.State = AlarmState.Acknowledged;
                    ctx.SaveChanges();
                }
            }
            dgAlarms.Items.Refresh();
            Logger.Log("ALARM_ACKNOWLEDGED", $"AlarmId: {alarm.Id}, Tag: {alarm.TagName}");
        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            using (var ctx = new DataConcentrator.Database.ScadaContext())
            {
                var lines = ctx.AnalogInputs.ToList()
                    .Where(ai => {
                        double mid = (ai.HighLimit + ai.LowLimit) / 2;
                        return ai.CurrentValue >= mid - 5 && ai.CurrentValue <= mid + 5;
                    })
                    .Select(ai => $"{ai.TagName} | {ai.CurrentValue:F2} | " +
                                  $"Mid: {(ai.HighLimit + ai.LowLimit) / 2:F2}");

                File.WriteAllLines("report.txt", lines);
                MessageBox.Show("Report generated: report.txt");
                Logger.Log("REPORT_GENERATED");
            }
        }
    }
}