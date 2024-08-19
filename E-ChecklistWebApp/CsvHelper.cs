using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace E_ChecklistWebApp
{
    public class CsvHelper
    {
        public static string ConvertToCsv<T>(IEnumerable<T> models)
        {
            if (models == null || !models.Any())
            {
                throw new ArgumentException("The model list is empty.");
            }

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var csvBuilder = new StringBuilder();

            // Add the header row
            csvBuilder.AppendLine(string.Join(",", properties.Select(p => EscapeCsvValue(p.Name))));

            // Add the data rows
            foreach (var model in models)
            {
                var values = properties.Select(p => EscapeCsvValue(p.GetValue(model, null)?.ToString() ?? string.Empty));
                csvBuilder.AppendLine(string.Join(",", values));
            }

            return csvBuilder.ToString();
        }

        private static string EscapeCsvValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            // If the value contains commas, quotes, or line breaks, enclose it in quotes
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
            {
                // Escape any existing quotes by doubling them
                value = value.Replace("\"", "\"\"");
                return $"\"{value}\"";
            }

            return value;
        }
    }
}