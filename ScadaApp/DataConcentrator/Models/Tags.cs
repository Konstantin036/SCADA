using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataConcentrator.Models
{
    public enum TagType { AI, AO, DI, DO }

    public abstract class Tag : INotifyPropertyChanged
    {
        [Key]
        public string TagName { get; set; }
        public string Description { get; set; }
        public string IOAddress { get; set; }
        public TagType Type { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public abstract class InputTag : Tag
    {
        public int ScanTime { get; set; }
        public bool IsScanning { get; set; } = true;
    }

    public abstract class OutputTag : Tag
    {
        public double InitialValue { get; set; }
    }

    public class AnalogInput : InputTag
    {
        public double LowLimit { get; set; }
        public double HighLimit { get; set; }
        public string Units { get; set; }
        public double Deadband { get; set; }
        public double Hysteresis { get; set; }

        private double _currentValue;
        public double CurrentValue
        {
            get => _currentValue;
            set
            {
                _currentValue = value;
                OnPropertyChanged(nameof(CurrentValue));
            }
        }

        [NotMapped]
        public List<Alarm> Alarms { get; set; } = new List<Alarm>();

        public AnalogInput() { Type = TagType.AI; }
    }

    public class AnalogOutput : OutputTag
    {
        public double LowLimit { get; set; }
        public double HighLimit { get; set; }
        public string Units { get; set; }

        private double _currentValue;
        public double CurrentValue
        {
            get => _currentValue;
            set
            {
                _currentValue = value;
                OnPropertyChanged(nameof(CurrentValue));
            }
        }

        public AnalogOutput() { Type = TagType.AO; }
    }

    public class DigitalInput : InputTag
    {
        private bool _currentValue;
        public bool CurrentValue
        {
            get => _currentValue;
            set
            {
                _currentValue = value;
                OnPropertyChanged(nameof(CurrentValue));
            }
        }

        public DigitalInput() { Type = TagType.DI; }
    }

    public class DigitalOutput : OutputTag
    {
        private bool _currentValue;
        public bool CurrentValue
        {
            get => _currentValue;
            set
            {
                _currentValue = value;
                OnPropertyChanged(nameof(CurrentValue));
            }
        }

        public DigitalOutput() { Type = TagType.DO; }
    }
}