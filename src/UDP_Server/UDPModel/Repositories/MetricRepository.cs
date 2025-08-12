using System.Collections.Generic;

namespace UDPModel.Repositories
{
    public class MetricRepository
    {
        private Dictionary<string, float> _metrics = new Dictionary<string, float>();

        private object _sync = new object();

        public void Add(string name, float value)
        {
            lock (_sync)
            {
                if (_metrics.ContainsKey(name))
                {
                    Update(name, value);
                }
                else
                {
                    _metrics.Add(name, value);
                }
            }
        }

        public Dictionary<string, float> GetAll()
        {
            lock (_sync)
            {
                return new Dictionary<string, float>(_metrics);
            }
        }

        private void Update(string name, float value)
        {
            _metrics[name] = value;
        }
    }
}