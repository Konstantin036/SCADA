using DataConcentrator.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ScadaWPF.Views
{
    public partial class EditAlarmWindow : Window
    {
        private Alarm _alarm;

        public EditAlarmWindow(Alarm alarm)
        {
            InitializeComponent();
            _alarm = alarm;

            txtLimit.Text = alarm.Limit.ToString();
            txtMessage.Text = alarm.Message;
            cmbType.SelectedIndex = alarm.Type == AlarmType.High ? 0 : 1;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _alarm.Limit = double.Parse(txtLimit.Text);
                _alarm.Message = txtMessage.Text;
                _alarm.Type = (cmbType.SelectedItem as ComboBoxItem)?.Content.ToString() == "High"
                    ? AlarmType.High : AlarmType.Low;

                using (var ctx = new DataConcentrator.Database.ScadaContext())
                {
                    var dbAlarm = ctx.Alarms.Find(_alarm.Id);
                    if (dbAlarm != null)
                    {
                        dbAlarm.Limit = _alarm.Limit;
                        dbAlarm.Message = _alarm.Message;
                        dbAlarm.Type = _alarm.Type;
                        ctx.SaveChanges();
                    }
                }
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška: {ex.Message}");
            }
        }
    }
}