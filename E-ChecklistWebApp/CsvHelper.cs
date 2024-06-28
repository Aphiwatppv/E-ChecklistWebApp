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
            csvBuilder.AppendLine(string.Join(",", properties.Select(p => p.Name)));

            // Add the data rows
            foreach (var model in models)
            {
                var values = properties.Select(p => p.GetValue(model, null)?.ToString() ?? string.Empty);
                csvBuilder.AppendLine(string.Join(",", values));
            }

            return csvBuilder.ToString();
        }
    }
}