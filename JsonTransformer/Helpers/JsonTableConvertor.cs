using JsonTransformer.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Dynamic;
using System.Text.Json.Nodes;

// Disable warning for ExpandoObject casting
#pragma warning disable CS8619

namespace JsonTransformer.Helpers
{
    public class JsonTableConvertor
    {
        private readonly ILogger _logger;

        public JsonTableConvertor(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger<JsonFetcher>();
        }

        public List<ExpandoObject>? ReadAsDynamicList(string json)
        {
            var jsonNode = JsonArray.Parse(json);

            if (jsonNode == null || jsonNode is not JsonArray)
            {
                _logger.LogError("Error parsing json array!");
                return null;
            }

            var jsonArray = jsonNode.AsArray();

            if (!jsonArray.Any())
            {
                _logger.LogInformation("No Data!");
                return null;
            }

            var records = new List<ExpandoObject>();

            // We use the first object of our data to conctruct the columns, for this demo we assume al object are valid and have identical fields
            var first = jsonArray.First()!.AsObject();

            // For this demo we just use all string columns since we convert it to csv
            var fields = GetFieldNames(first);

            foreach (var record in jsonArray.Select(j => j.AsObject()))
            {
                var newRecord = new ExpandoObject();

                foreach (var field in fields)
                {
                    // Get subfield using json extension that supports path lookup
                    var value = record.GetValue(field);

                    ((IDictionary<string, Object>)newRecord).Add(field, value!.AsValue().ToString());
                }
                records.Add(newRecord);
            }

            return records;
        }

        private List<string> GetFieldNames(JsonObject jsonObject, string? root = null)
        {
            // Loop plain fields
            var list = new List<string>();
            foreach (var property in jsonObject.Where(p => p.Value is JsonValue))
            {
                string name = property.Key;
                if (root != null)
                {
                    name = $"{root}.{name}";
                }
                list.Add(name);
            }

            // Loop objects
            foreach (var property in jsonObject.Where(p => p.Value is JsonObject))
            {
                var fields = GetFieldNames(property.Value!.AsObject(), property.Key);
                list.AddRange(fields);
            }

            return list;
        }

    }
}
