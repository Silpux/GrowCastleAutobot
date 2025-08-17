using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gca.Classes
{
    public static class CrystalsCollectionTimeLogger
    {
        private const string SEPARATOR = "|";
        private const string DATETIME_FORMAT = "dd/MM/yyyy HH:mm:ss.fff";
        private const string TIMEDIFF_FORMAT = "hh\\:mm\\:ss\\.fff";

        public static void AddTimeStampWithDiff(string path)
        {
            DateTime now = DateTime.Now;

            path = Path.GetFullPath(path);

            string? dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

            string? prevLine = null;
            if (File.Exists(path))
            {
                prevLine = Utils.ReadLastLine(path);
            }

            bool parsed = TryParseTimestamp(prevLine, out DateTime prevDt);

            string lineToAppend = parsed
                ? $"{now.ToString(DATETIME_FORMAT, CultureInfo.InvariantCulture)} {SEPARATOR} {FormatTimespan(now - prevDt)}"
                : $"{now.ToString(DATETIME_FORMAT, CultureInfo.InvariantCulture)}";

            using var fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read);
            using var sw = new StreamWriter(fs);
            sw.Write(lineToAppend);
            sw.Write(Environment.NewLine);
        }

        private static bool TryParseTimestamp(string? line, out DateTime dt)
        {
            dt = DateTime.MinValue;
            if (string.IsNullOrWhiteSpace(line))
            {
                return false;
            }

            string firstPart = line.Split(SEPARATOR)[0].Trim();

            if (DateTime.TryParseExact(firstPart, DATETIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                return true;
            }

            return false;
        }

        private static string FormatTimespan(TimeSpan ts)
        {
            if (ts.TotalDays >= 1)
            {
                return string.Empty;
            }
            return ts.ToString(TIMEDIFF_FORMAT);
        }

    }

}
