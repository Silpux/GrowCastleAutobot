using gca_clicker.Clicker;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace gca_clicker.Classes
{
    public static class Log
    {
        private static string DTN => DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff");

        private static void SaveLog(string message, string logLevel, int line)
        {
            string log = $"[{DTN} {line,-4}] [{logLevel}] {message}\n";
            File.AppendAllText(Cst.LOG_FILE_PATH, log);
            Debug.Write(log);
        }

        [Conditional("DEBUG")]
        public static void D(string message, [CallerLineNumber] int line = 0)
        {
            SaveLog(message, "D", line);
        }

        [Conditional("DEBUG")]
        public static void T(string message, [CallerLineNumber] int line = 0)
        {
            SaveLog(message, "T", line);
        }
        public static void V(string message, [CallerLineNumber] int line = 0)
        {
            SaveLog(message, "V", line);
        }

        public static void I(string message, [CallerLineNumber] int line = 0)
        {
            SaveLog(message, "I", line);
        }

        public static void W(string message, [CallerLineNumber] int line = 0)
        {
            SaveLog(message, "W", line);
        }

        public static void E(string message, [CallerLineNumber] int line = 0)
        {
            SaveLog(message, "E", line);
        }

        public static void Q(string message, [CallerLineNumber] int line = 0)
        {
            SaveLog(message, "Q", line);
        }

        public static void C(string message, [CallerLineNumber] int line = 0)
        {
            SaveLog(message, "F", line);
        }

        public static void ST(
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string member = "")
        {
            // skip 2 because 1-st is current method, and for second we need line number, and name is known
            var st = new System.Diagnostics.StackTrace(2, true).ToString();

            var callerInfo = $"   at {member}():line {line}";
            string log = $"Stack trace:\n{callerInfo}\n{st.ToString()}\n";
            File.AppendAllText(Cst.LOG_FILE_PATH, log);
            Debug.Write(log);
        }
    }
}
