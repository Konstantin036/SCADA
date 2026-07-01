using System;
using System.ComponentModel.DataAnnotations;

namespace DataConcentrator.Models
{
    public enum AlarmType { High, Low }
    public enum AlarmState { Active, Acknowledged, Inactive }

    public class Alarm
    {
        [Key]
        public int Id { get; set; }
        public string TagName { get; set; }
        public double Limit { get; set; }
        public AlarmType Type { get; set; }
        public string Message { get; set; }
        public AlarmState State { get; set; } = AlarmState.Inactive;
    }

    public class ActivatedAlarm
    {
        [Key]
        public int Id { get; set; }
        public int AlarmId { get; set; }
        public string TagName { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public AlarmState State { get; set; } = AlarmState.Active;
    }

    public class TagValueHistory
    {
        [Key]
        public int Id { get; set; }
        public string TagName { get; set; }
        public double Value { get; set; }
        public DateTime Timestamp { get; set; }
    }
}