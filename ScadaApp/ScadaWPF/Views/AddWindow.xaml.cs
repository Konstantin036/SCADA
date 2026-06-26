using DataConcentrator.Core;
using DataConcentrator.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ScadaWPF.Views
{
    public partial class AddWindow : Window
    {
        private DataConcentrator.Core.DataConcentrator _dc;

        public AddWindow()
        {
            InitializeComponent();
            _dc = DataConcentrator.Core.DataConcentrator.Instance;

            // Sakrij sve panele na startu
            pnlInput.Visibility = Visibility.Collapsed;
            pnlAnalog.Visibility = Visibility.Collapsed;
            pnlAI.Visibility = Visibility.Collapsed;
            pnlOutput.Visibility = Visibility.Collapsed;
            pnlAlarm.Visibility = Visibility.Collapsed;

            // Popuni AI tagove za alarm combo
            var aiTags = _dc.GetAllTags()
                .OfType<AnalogInput>()
                .Select(t => t.TagName);
            foreach (var name in aiTags)
                cmbAlarmTag.Items.Add(name);
        }

        private void cmbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbType.SelectedItem == null) return;
            string selected = (cmbType.SelectedItem as ComboBoxItem)?.Content.ToString();

            pnlInput.Visibility = Visibility.Collapsed;
            pnlAnalog.Visibility = Visibility.Collapsed;
            pnlAI.Visibility = Visibility.Collapsed;
            pnlOutput.Visibility = Visibility.Collapsed;
            pnlAlarm.Visibility = Visibility.Collapsed;
            txtTagName.IsEnabled = true;
            txtIOAddress.IsEnabled = true;

            switch (selected)
            {
                case "AI":
                    pnlInput.Visibility = Visibility.Visible;
                    pnlAnalog.Visibility = Visibility.Visible;
                    pnlAI.Visibility = Visibility.Visible;
                    break;
                case "AO":
                    pnlAnalog.Visibility = Visibility.Visible;
                    pnlOutput.Visibility = Visibility.Visible;
                    break;
                case "DI":
                    pnlInput.Visibility = Visibility.Visible;
                    break;
                case "DO":
                    pnlOutput.Visibility = Visibility.Visible;
                    break;
                case "Alarm":
                    pnlAlarm.Visibility = Visibility.Visible;
                    txtTagName.IsEnabled = false;
                    txtIOAddress.IsEnabled = false;
                    break;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            string selected = (cmbType.SelectedItem as ComboBoxItem)?.Content.ToString();

            try
            {
                switch (selected)
                {
                    case "AI":
                        _dc.AddTag(new AnalogInput
                        {
                            TagName = txtTagName.Text,
                            Description = txtDescription.Text,
                            IOAddress = txtIOAddress.Text,
                            ScanTime = int.Parse(txtScanTime.Text),
                            LowLimit = double.Parse(txtLowLimit.Text),
                            HighLimit = double.Parse(txtHighLimit.Text),
                            Units = txtUnits.Text,
                            Deadband = double.Parse(txtDeadband.Text),
                            Hysteresis = double.Parse(txtHysteresis.Text)
                        });
                        break;
                    case "AO":
                        _dc.AddTag(new AnalogOutput
                        {
                            TagName = txtTagName.Text,
                            Description = txtDescription.Text,
                            IOAddress = txtIOAddress.Text,
                            LowLimit = double.Parse(txtLowLimit.Text),
                            HighLimit = double.Parse(txtHighLimit.Text),
                            Units = txtUnits.Text,
                            InitialValue = double.Parse(txtInitialValue.Text)
                        });
                        break;
                    case "DI":
                        _dc.AddTag(new DigitalInput
                        {
                            TagName = txtTagName.Text,
                            Description = txtDescription.Text,
                            IOAddress = txtIOAddress.Text,
                            ScanTime = int.Parse(txtScanTime.Text)
                        });
                        break;
                    case "DO":
                        _dc.AddTag(new DigitalOutput
                        {
                            TagName = txtTagName.Text,
                            Description = txtDescription.Text,
                            IOAddress = txtIOAddress.Text,
                            InitialValue = double.Parse(txtInitialValue.Text)
                        });
                        break;
                    case "Alarm":
                        AddAlarm();
                        return;
                    default:
                        MessageBox.Show("Izaberi tip.");
                        return;
                }
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška: {ex.Message}");
            }
        }

        private void AddAlarm()
        {
            if (cmbAlarmTag.SelectedItem == null)
            {
                MessageBox.Show("Izaberi AI tag.");
                return;
            }

            string tagName = cmbAlarmTag.SelectedItem.ToString();
            string alarmTypeStr = (cmbAlarmType.SelectedItem as ComboBoxItem)?.Content.ToString();

            using (var ctx = new DataConcentrator.Database.ScadaContext())
            {
                var alarm = new Alarm
                {
                    TagName = tagName,
                    Limit = double.Parse(txtAlarmLimit.Text),
                    Type = alarmTypeStr == "High" ? AlarmType.High : AlarmType.Low,
                    Message = txtAlarmMessage.Text,
                    State = AlarmState.Inactive
                };
                ctx.Alarms.Add(alarm);
                ctx.SaveChanges();

                // Dodaj u in-memory tag
                var aiTag = _dc.GetAnalogInput(tagName);
                aiTag?.Alarms.Add(alarm);
            }
            DialogResult = true;
            Close();
        }
    }
}