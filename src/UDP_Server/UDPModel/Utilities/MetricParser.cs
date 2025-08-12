using System;
using System.Globalization;

namespace UDPModel.Utilities
{
    public static class MetricParser
    {
        public static (string Name, float Value) Parse(string metric)
        {
            var parts = metric.Split(':');

            if (parts.Length != 2)
            {
                throw new FormatException("Некорректный формат метрики");
            }

            var name = parts[0];
            var value = parts[1];

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new FormatException("Имя метрики не может быть пустым");
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new FormatException("Значение метрики не может быть пустым");
            }

            if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsedValue))
            {
                return (name, parsedValue);
            }
            else
            {
                throw new FormatException("Значение не может быть преобразовано из строки в число");
            }
        }
    }
}