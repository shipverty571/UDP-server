using System.Globalization;

namespace UDPModel
{
    public static class MetricParser
    {
        public static (string Name, float Value) Parse(string metric)
        {
            var parts = metric.Split(':');

            return (parts[0], float.Parse(parts[1], CultureInfo.InvariantCulture));
        }
    }
}