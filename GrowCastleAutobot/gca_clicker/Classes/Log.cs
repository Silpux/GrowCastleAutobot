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


        [Conditional("DEBUG")]
        public static void D(string message)
        {
            string log = $"[{DTN}] [D] {message}\n";
            File.AppendAllText(Cst.LOG_FILE_PATH, log);
            Debug.Write(log);
        }

        [Conditional("DEBUG")]
        public static void T(string message)
        {
            string log = $"[{DTN}] [T] {message}\n";
            File.AppendAllText(Cst.LOG_FILE_PATH, log);
            Debug.Write(log);
        }
        public static void V(string message)
        {
            string log = $"[{DTN}] [V] {message}\n";
            File.AppendAllText(Cst.LOG_FILE_PATH, log);
            Debug.Write(log);
        }

        public static void I(string message)
        {
            string log = $"[{DTN}] [I] {message}\n";
            File.AppendAllText(Cst.LOG_FILE_PATH, log);
            Debug.Write(log);
        }

        public static void W(string message)
        {
            string log = $"[{DTN}] [W] {message}\n";
            File.AppendAllText(Cst.LOG_FILE_PATH, log);
            Debug.Write(log);
        }

        public static void E(string message)
        {
            string log = $"[{DTN}] [E] {message}\n";
            File.AppendAllText(Cst.LOG_FILE_PATH, log);
            Debug.Write(log);
        }

        public static void C(string message)
        {
            string log = $"[{DTN}] [F] {message}\n";
            File.AppendAllText(Cst.LOG_FILE_PATH, log);
            Debug.Write(log);
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
