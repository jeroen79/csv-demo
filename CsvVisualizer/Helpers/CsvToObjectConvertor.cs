using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace CsvVisualizer.Helpers
{
    public class CsvToObjectConvertor
    {
        private readonly ILogger _logger;

        public CsvToObjectConvertor(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger<CsvToObjectConvertor>();
        }


        public List<T> Convert<T>(string csvData, char seperator = ';')
        {
            var result = new List<T>();

            // Convert csv to list of expandos
            var records = new List<ExpandoObject>();
            var lines = csvData.Split("\n");

            if (lines.Length < 2)
            {
                _logger.LogWarning("Empty CSV Detected!");
                return result;
            }

            var columns = lines.First().TrimEnd().Split(seperator);

            Type type = typeof(T);

            foreach (var line in lines.Skip(1))
            {
                var record = (T)Activator.CreateInstance(type)!;

                var parts = line.TrimEnd().Split(seperator);

                // Skip if format doesn't match header
                if (parts.Length != columns.Length)
                {
                    continue;
                }

                // Convert line to dictionary
                Dictionary<string, string> row = parts.Select((r, i) => new
                {
                    Key = columns[i],
                    Value = r
                }).ToDictionary(r => r.Key, r => r.Value);

                SetProperties(record, row, type);
                result.Add(record);
            }


            return result;
        }
        private void SetProperties(object record, Dictionary<string, string> row, Type type, string? prefix = null)
        {
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                string propertyName = property.Name;

                if (prefix != null)
                {
                    propertyName = $"{prefix}.{propertyName}";
                }

                var propType = property.PropertyType;

                // Skip subObjects
                if (propType.IsValueType || propType == typeof(string))
                {
                    // Skip if it doesn't exist
                    if (row.ContainsKey(propertyName) == false)
                    {
                        continue;
                    }

                    string stringValue = row[propertyName].ToString()!;

                    // Todo implement other types
                    if (propType == typeof(decimal))
                    {
                        decimal value = decimal.Parse(stringValue);
                        property.SetValue(record, value);
                    }
                    else if (propType == typeof(bool))
                    {
                        bool value = bool.Parse(stringValue);
                        property.SetValue(record, value);
                    }
                    else if (propType == typeof(int))
                    {
                        int value = int.Parse(stringValue);
                        property.SetValue(record, value);
                    }
                    else if (propType == typeof(string))
                    {
                        property.SetValue(record, stringValue);
                    }

                }
                else // Procces sub objects
                {
                    var subrecord = Activator.CreateInstance(propType)!;

                    SetProperties(subrecord, row, propType, propertyName);
                    property.SetValue(record, subrecord);
                }


            }
        }

    }
}
