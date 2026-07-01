using DataConcentrator.Models;
using DataConcentrator.Services;
using ScadaWPF.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ScadaWPF.Views
{
    public partial class DetailsWindow : Window
    {
        private AnalogInput _tag;
        private ObservableCollection<Alarm> _alarms;
        public event Action<int> AlarmRemoved;

        public DetailsWindow(AnalogInput tag)
        {
            InitializeComponent();

            var viewModel = new MainViewModel();
            viewModel.Refresh();
            this.DataContext = viewModel;

            _tag = tag;
            txtTagInfo.Text = $"{tag.TagName} | {tag.Description} | " +
                              $"Current: {tag.CurrentValue:F2} {tag.Units}";

            _alarms = new ObservableCollection<Alarm>(tag.Alarms);
            dgAlarms.ItemsSource = _alarms;
        }

        private void btnRemoveAlarm_Click(object sender, RoutedEventArgs e)
        {
            var alarm = (sender as Button)?.DataContext as Alarm;
            AlarmRemoved?.Invoke(alarm.Id);
            if (alarm == null) return;

            using (var ctx = new DataConcentrator.Database.ScadaContext())
            {
                var dbAlarm = ctx.Alarms.Find(alarm.Id);
                if (dbAlarm != null)
                {
                    ctx.Alarms.Remove(dbAlarm);
                    ctx.SaveChanges();
                }
            }
            _tag.Alarms.Remove(alarm);
            _alarms.Remove(alarm);
        }

        private void btnEditAlarm_Click(object sender, RoutedEventArgs e)
        {
            var alarm = (sender as Button)?.DataContext as Alarm;
            if (alarm == null) return;

            var editWindow = new EditAlarmWindow(alarm);
            if (editWindow.ShowDialog() == true)
                dgAlarms.Items.Refresh();
        }

    }
}