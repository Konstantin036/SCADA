using DataConcentrator.Database;
using DataConcentrator.Models;
using PLCSimulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DataConcentrator.Core
{
    public class DataConcentrator
    {
        private static DataConcentrator _instance;
        private static readonly object _lock = new object();

        private Dictionary<string, AnalogInput> _analogInputs;
        private Dictionary<string, DigitalInput> _digitalInputs;
        private Dictionary<string, AnalogOutput> _analogOutputs;
        private Dictionary<string, DigitalOutput> _digitalOutputs;

        public event Action<int> AlarmRaised;
        private SynchronizationContext _syncContext;

        private DataConcentrator()
        {
            _syncContext = SynchronizationContext.Current;
            _analogInputs = new Dictionary<string, AnalogInput>();
            _digitalInputs = new Dictionary<string, DigitalInput>();
            _analogOutputs = new Dictionary<string, AnalogOutput>();
            _digitalOutputs = new Dictionary<string, DigitalOutput>();

            PLCSimulator.PLCSimulator.Instance.ValueChanged += OnValueChanged;
            LoadFromDatabase();
        }

        public static DataConcentrator Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new DataConcentrator();
                    return _instance;
                }
            }
        }

        private void LoadFromDatabase()
        {
            try
            {
                using (var ctx = new ScadaContext())
                {
                    foreach (var tag in ctx.AnalogInputs.ToList())
                    {
                        tag.Alarms = ctx.Alarms
                            .Where(a => a.TagName == tag.TagName)
                            .ToList();
                        _analogInputs[tag.TagName] = tag;
                    }
                    foreach (var tag in ctx.DigitalInputs.ToList())
                        _digitalInputs[tag.TagName] = tag;
                    foreach (var tag in ctx.AnalogOutputs.ToList())
                        _analogOutputs[tag.TagName] = tag;
                    foreach (var tag in ctx.DigitalOutputs.ToList())
                        _digitalOutputs[tag.TagName] = tag;
                    foreach (var tag in _analogInputs.Values)
                        PLCSimulator.PLCSimulator.Instance.GetValue(tag.IOAddress);

                    foreach (var tag in _digitalInputs.Values)
                        PLCSimulator.PLCSimulator.Instance.GetValue(tag.IOAddress);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"DB greška: {ex.Message}", ex);
            }
        }

        private void OnValueChanged(string ioAddress, double value)
        {
            lock (_lock)
            {
                var tag = _analogInputs.Values
                    .FirstOrDefault(t => t.IOAddress == ioAddress && t.IsScanning);

                if (tag == null) return;

                double diff = Math.Abs(value - tag.CurrentValue);
                if (diff < tag.Deadband) return;

                tag.CurrentValue = value;
                _syncContext?.Post(_ => { }, null);

                CheckAlarms(tag);
            }
        }

        private void CheckAlarms(AnalogInput tag)
        {
            foreach (var alarm in tag.Alarms)
            {
                bool triggered = alarm.Type == AlarmType.High
                    ? tag.CurrentValue > alarm.Limit + tag.Hysteresis
                    : tag.CurrentValue < alarm.Limit - tag.Hysteresis;

                if (triggered && alarm.State == AlarmState.Inactive)
                {
                    alarm.State = AlarmState.Active;
                    SaveActivatedAlarm(alarm, tag.TagName);
                }
                else if (!triggered && alarm.State != AlarmState.Inactive)
                {
                    alarm.State = AlarmState.Inactive;
                }
            }
        }

        private void SaveActivatedAlarm(Alarm alarm, string tagName)
        {
            using (var ctx = new ScadaContext())
            {
                var activated = new ActivatedAlarm
                {
                    AlarmId = alarm.Id,
                    TagName = tagName,
                    Message = alarm.Message,
                    Timestamp = DateTime.Now,
                    State = AlarmState.Active
                };
                ctx.ActivatedAlarms.Add(activated);
                ctx.SaveChanges();
                AlarmRaised?.Invoke(activated.Id);
            }
        }

        public void AddTag(Tag tag)
        {
            using (var ctx = new ScadaContext())
            {
                switch (tag)
                {
                    case AnalogInput ai:
                        ctx.AnalogInputs.Add(ai);
                        _analogInputs[ai.TagName] = ai;
                        break;
                    case AnalogOutput ao:
                        ctx.AnalogOutputs.Add(ao);
                        _analogOutputs[ao.TagName] = ao;
                        break;
                    case DigitalInput di:
                        ctx.DigitalInputs.Add(di);
                        _digitalInputs[di.TagName] = di;
                        break;
                    case DigitalOutput dout:
                        ctx.DigitalOutputs.Add(dout);
                        _digitalOutputs[dout.TagName] = dout;
                        break;
                }
                ctx.SaveChanges();
            }
        }

        public void RemoveTag(string tagName)
        {
            using (var ctx = new ScadaContext())
            {
                if (_analogInputs.ContainsKey(tagName))
                {
                    // Obrisi alarme i aktivirane alarme
                    var alarms = ctx.Alarms.Where(a => a.TagName == tagName).ToList();
                    ctx.Alarms.RemoveRange(alarms);

                    var activatedAlarms = ctx.ActivatedAlarms.Where(a => a.TagName == tagName).ToList();
                    ctx.ActivatedAlarms.RemoveRange(activatedAlarms);

                    ctx.AnalogInputs.Remove(ctx.AnalogInputs.Find(tagName));
                    _analogInputs.Remove(tagName);
                }
                else if (_analogOutputs.ContainsKey(tagName))
                {
                    ctx.AnalogOutputs.Remove(ctx.AnalogOutputs.Find(tagName));
                    _analogOutputs.Remove(tagName);
                }
                else if (_digitalInputs.ContainsKey(tagName))
                {
                    var activatedAlarms = ctx.ActivatedAlarms.Where(a => a.TagName == tagName).ToList();
                    ctx.ActivatedAlarms.RemoveRange(activatedAlarms);

                    ctx.DigitalInputs.Remove(ctx.DigitalInputs.Find(tagName));
                    _digitalInputs.Remove(tagName);
                }
                else if (_digitalOutputs.ContainsKey(tagName))
                {
                    ctx.DigitalOutputs.Remove(ctx.DigitalOutputs.Find(tagName));
                    _digitalOutputs.Remove(tagName);
                }
                ctx.SaveChanges();
            }
        }

        public List<Tag> GetAllTags()
        {
            var all = new List<Tag>();
            all.AddRange(_analogInputs.Values);
            all.AddRange(_analogOutputs.Values);
            all.AddRange(_digitalInputs.Values);
            all.AddRange(_digitalOutputs.Values);
            return all;
        }

        public AnalogInput GetAnalogInput(string tagName)
        {
            _analogInputs.TryGetValue(tagName, out var tag);
            return tag;
        }

        public void ToggleScan(string tagName, bool isScanning)
        {
            if (_analogInputs.ContainsKey(tagName))
                _analogInputs[tagName].IsScanning = isScanning;
            else if (_digitalInputs.ContainsKey(tagName))
                _digitalInputs[tagName].IsScanning = isScanning;
        }

        public void WriteToOutput(string tagName, double value)
        {
            if (_analogOutputs.ContainsKey(tagName))
            {
                _analogOutputs[tagName].CurrentValue = value;
                PLCSimulator.PLCSimulator.Instance.SetValue(
                    _analogOutputs[tagName].IOAddress, value);
            }
            else if (_digitalOutputs.ContainsKey(tagName))
            {
                _digitalOutputs[tagName].CurrentValue = value > 0;
                PLCSimulator.PLCSimulator.Instance.SetValue(
                    _digitalOutputs[tagName].IOAddress, value);
            }
        }
    }
}