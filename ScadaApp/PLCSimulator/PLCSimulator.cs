using System;
using System.Collections.Generic;
using System.Threading;

namespace PLCSimulator
{
    public class PLCSimulator
    {
        private static PLCSimulator _instance;
        private static readonly object _lock = new object();

        private Dictionary<string, double> _values;
        private Random _random;
        private Timer _timer;

        public event Action<string, double> ValueChanged;

        private PLCSimulator()
        {
            _random = new Random();
            _values = new Dictionary<string, double>();
            _timer = new Timer(SimulateValues, null, 0, 1000);
        }

        public static PLCSimulator Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new PLCSimulator();
                    return _instance;
                }
            }
        }

        private void SimulateValues(object state)
        {
            lock (_lock)
            {
                foreach (var key in _values.Keys)
                {
                    _values[key] += _random.NextDouble() * 10 - 5;
                    ValueChanged?.Invoke(key, _values[key]);
                }
            }
        }

        public double GetValue(string ioAddress)
        {
            lock (_lock)
            {
                if (!_values.ContainsKey(ioAddress))
                    _values[ioAddress] = _random.NextDouble() * 100;
                return _values[ioAddress];
            }
        }

        public void SetValue(string ioAddress, double value)
        {
            lock (_lock)
            {
                _values[ioAddress] = value;
                ValueChanged?.Invoke(ioAddress, value);
            }
        }
    }
}