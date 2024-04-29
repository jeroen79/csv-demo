using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace JsonTransformer.Extensions
{
    public static class JsonExtensions
    {
        public static JsonValue? GetValue(this JsonObject jsonObject, string jsonPath)
        {
            // Last level
            if (jsonPath.Contains(".") == false)
            {
                if (jsonObject.ContainsKey(jsonPath))
                {
                    var value = jsonObject[jsonPath];
                    if (value != null && value is JsonValue)
                    {
                        return value.AsValue();
                    }
                }
            }
            else
            {
                // Not last level run recursive lookup
                var parentPath = jsonPath.Substring(0, jsonPath.IndexOf("."));
                var childPath = jsonPath.Substring(jsonPath.IndexOf(".") + 1);

                if (jsonObject.ContainsKey(parentPath))
                {
                    var subObject = jsonObject[parentPath];
                    if (subObject != null && subObject is JsonObject)
                    {
                        var value = subObject.AsObject().GetValue(childPath);
                        return value;
                    }
                }
            }

            return null;
        }
    }
}
