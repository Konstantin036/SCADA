using DataConcentrator.Models;
using System;
using System.Windows;

namespace ScadaWPF.Views
{
    public partial class EditWindow : Window
    {
        private Tag _tag;

        public EditWindow(Tag tag)
        {
            InitializeComponent();
            _tag = tag;

            pnlInput.Visibility = Visibility.Collapsed;
            pnlAnalog.Visibility = Visibility.Collapsed;
            pnlAI.Visibility = Visibility.Collapsed;
            pnlOutput.Visibility = Visibility.Collapsed;

            txtTagName.Text = tag.TagName;
            txtDescription.Text = tag.Description;
            txtIOAddress.Text = tag.IOAddress;

            switch (tag)
            {
                case AnalogInput ai:
                    pnlInput.Visibility = Visibility.Visible;
                    pnlAnalog.Visibility = Visibility.Visible;
                    pnlAI.Visibility = Visibility.Visible;
                    txtScanTime.Text = ai.ScanTime.ToString();
                    txtLowLimit.Text = ai.LowLimit.ToString();
                    txtHighLimit.Text = ai.HighLimit.ToString();
                    txtUnits.Text = ai.Units;
                    txtDeadband.Text = ai.Deadband.ToString();
                    txtHysteresis.Text = ai.Hysteresis.ToString();
                    break;
                case AnalogOutput ao:
                    pnlAnalog.Visibility = Visibility.Visible;
                    pnlOutput.Visibility = Visibility.Visible;
                    txtLowLimit.Text = ao.LowLimit.ToString();
                    txtHighLimit.Text = ao.HighLimit.ToString();
                    txtUnits.Text = ao.Units;
                    txtInitialValue.Text = ao.InitialValue.ToString();
                    break;
                case DigitalInput di:
                    pnlInput.Visibility = Visibility.Visible;
                    txtScanTime.Text = di.ScanTime.ToString();
                    break;
                case DigitalOutput dout:
                    pnlOutput.Visibility = Visibility.Visible;
                    txtInitialValue.Text = dout.InitialValue.ToString();
                    break;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _tag.Description = txtDescription.Text;
                _tag.IOAddress = txtIOAddress.Text;

                using (var ctx = new DataConcentrator.Database.ScadaContext())
                {
                    switch (_tag)
                    {
                        case AnalogInput ai:
                            ai.ScanTime = int.Parse(txtScanTime.Text);
                            ai.LowLimit = double.Parse(txtLowLimit.Text);
                            ai.HighLimit = double.Parse(txtHighLimit.Text);
                            ai.Units = txtUnits.Text;
                            ai.Deadband = double.Parse(txtDeadband.Text);
                            ai.Hysteresis = double.Parse(txtHysteresis.Text);
                            var dbAI = ctx.AnalogInputs.Find(ai.TagName);
                            if (dbAI != null)
                            {
                                dbAI.Description = ai.Description;
                                dbAI.IOAddress = ai.IOAddress;
                                dbAI.ScanTime = ai.ScanTime;
                                dbAI.LowLimit = ai.LowLimit;
                                dbAI.HighLimit = ai.HighLimit;
                                dbAI.Units = ai.Units;
                                dbAI.Deadband = ai.Deadband;
                                dbAI.Hysteresis = ai.Hysteresis;
                            }
                            break;
                        case AnalogOutput ao:
                            ao.LowLimit = double.Parse(txtLowLimit.Text);
                            ao.HighLimit = double.Parse(txtHighLimit.Text);
                            ao.Units = txtUnits.Text;
                            ao.InitialValue = double.Parse(txtInitialValue.Text);
                            var dbAO = ctx.AnalogOutputs.Find(ao.TagName);
                            if (dbAO != null)
                            {
                                dbAO.Description = ao.Description;
                                dbAO.IOAddress = ao.IOAddress;
                                dbAO.LowLimit = ao.LowLimit;
                                dbAO.HighLimit = ao.HighLimit;
                                dbAO.Units = ao.Units;
                                dbAO.InitialValue = ao.InitialValue;
                            }
                            break;
                        case DigitalInput di:
                            di.ScanTime = int.Parse(txtScanTime.Text);
                            var dbDI = ctx.DigitalInputs.Find(di.TagName);
                            if (dbDI != null)
                            {
                                dbDI.Description = di.Description;
                                dbDI.IOAddress = di.IOAddress;
                                dbDI.ScanTime = di.ScanTime;
                            }
                            break;
                        case DigitalOutput dout:
                            dout.InitialValue = double.Parse(txtInitialValue.Text);
                            var dbDO = ctx.DigitalOutputs.Find(dout.TagName);
                            if (dbDO != null)
                            {
                                dbDO.Description = dout.Description;
                                dbDO.IOAddress = dout.IOAddress;
                                dbDO.InitialValue = dout.InitialValue;
                            }
                            break;
                    }
                    ctx.SaveChanges();
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