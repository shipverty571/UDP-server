using System.Collections.Generic;
using System.Linq;

namespace UDPModel.Repositories
{
    public class MetricRepository
    {
        private Dictionary<string, float> _metrics = new Dictionary<string, float>();

        public void Add(string name, float value)
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

        public string GetAll()
        {
            if (_metrics.Count == 0)
            {
                return "[METRIC] Нет данных";
            }

            var text = string.Join(" | ", _metrics.Select(kv => $"{kv.Key} = {kv.Value}"));
            return "[METRIC] " + text;
        }

        private void Update(string name, float value)
        {
            _metrics[name] = value;
        }
    }
}