using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataConcentrator.Models
{
    public enum TagType { AI, AO, DI, DO }

    public abstract class Tag
    {
        [Key]
        public string TagName { get; set; }
        public string Description { get; set; }
        public string IOAddress { get; set; }
        public TagType Type { get; set; }
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
        public double CurrentValue { get; set; }

        [NotMapped]
        public List<Alarm> Alarms { get; set; } = new List<Alarm>();

        public AnalogInput() { Type = TagType.AI; }
    }

    public class AnalogOutput : OutputTag
    {
        public double LowLimit { get; set; }
        public double HighLimit { get; set; }
        public string Units { get; set; }
        public double CurrentValue { get; set; }

        public AnalogOutput() { Type = TagType.AO; }
    }

    public class DigitalInput : InputTag
    {
        public bool CurrentValue { get; set; }
        public DigitalInput() { Type = TagType.DI; }
    }

    public class DigitalOutput : OutputTag
    {
        public bool CurrentValue { get; set; }
        public DigitalOutput() { Type = TagType.DO; }
    }
}