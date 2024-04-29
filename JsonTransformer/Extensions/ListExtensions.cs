using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JsonTransformer.Extensions
{
    public static class ListExtensions
    {
        // Converts a list of ExpandoObjects to a csv, all object needs to be the same format!
        public static string? ToCSVString(this List<ExpandoObject> list, char seperator = ';', char? textQualifier = null, bool useHeaderRow = true)
        {
            if (list == null || list.Count == 0)
            {
                return null;
            }

            var sw = new StringWriter();

            var first = list.First();

            if (useHeaderRow)
            {
                string headerLine = "";
                foreach (KeyValuePair<string, object?> property in first)
                {
                    headerLine += property.Key + seperator;
                }
                // Trim last seperator and add line break
                headerLine = headerLine.TrimEnd(seperator) + "\n";
                sw.Write(headerLine);
            }

            foreach (var listItem in list)
            {
                string line = "";
                foreach (KeyValuePair<string, object?> property in listItem)
                {

                    if (textQualifier != null)
                    {
                        line += textQualifier;
                    }

                    line += property.Value;

                    if (textQualifier != null)
                    {
                        line += textQualifier;
                    }

                    line += seperator;

                }
                line = line.TrimEnd(seperator) + "\n";

                sw.Write(line);
            }

            return sw.ToString();
        }
    }
}
