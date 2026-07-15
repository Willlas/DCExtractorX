using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Custom.IO
{
    /// <summary>
    /// Class   :   "CsvExporter"
    /// 
    /// Purpose :   a small, generic CSV writer used by the analysis/reporting subsystems (models.csv, summary.csv,
    /// analysis.csv, errors.csv, warnings.csv, unsupported.csv). Not tied to any particular row type - callers
    /// supply a header row and per-row cell values.
    /// </summary>
    public static class CsvExporter
    {
        /// <summary>
        /// Writes a CSV file, creating the destination directory if needed. Overwrites any existing file.
        /// </summary>
        /// <param name="szFilePath">The full path of the .csv file to write.</param>
        /// <param name="tHeaders">The column headers, in order.</param>
        /// <param name="tRows">The rows to write; each row's cell count should match tHeaders' count.</param>
        public static void Write(string szFilePath, IEnumerable<string> tHeaders, IEnumerable<IEnumerable<object>> tRows)
        {
            string szDirectory = Path.GetDirectoryName(szFilePath);
            if (string.IsNullOrEmpty(szDirectory) == false)
                Directory.CreateDirectory(szDirectory);

            using (StreamWriter tWriter = new StreamWriter(szFilePath, false, Encoding.UTF8))
            {
                tWriter.WriteLine(string.Join(",", tHeaders.Select(Escape)));

                foreach (IEnumerable<object> tRow in tRows)
                    tWriter.WriteLine(string.Join(",", tRow.Select(tValue => Escape(FormatValue(tValue)))));
            }
        }

        static string FormatValue(object tValue)
        {
            switch (tValue)
            {
                case null:
                    return string.Empty;
                case bool bValue:
                    return bValue ? "true" : "false";
                case float fValue:
                    return fValue.ToString("0.###", CultureInfo.InvariantCulture);
                case double dValue:
                    return dValue.ToString("0.###", CultureInfo.InvariantCulture);
                default:
                    return Convert.ToString(tValue, CultureInfo.InvariantCulture) ?? string.Empty;
            }
        }

        /// <summary>
        /// Escapes a single CSV field, quoting it if it contains a comma, quote, or newline.
        /// </summary>
        static string Escape(string szField)
        {
            if (szField.IndexOfAny(new[] { ',', '"', '\n', '\r' }) < 0)
                return szField;

            return "\"" + szField.Replace("\"", "\"\"") + "\"";
        }
    }
}
