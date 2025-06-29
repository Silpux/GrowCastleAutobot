#if DEBUG
#define TRACE
#endif
using gca_clicker.Clicker;
using gca_clicker.VM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gca_clicker.Classes
{
    public static class Log
    {
        private const int LAST_LOG_ENTRIES_COUNT = 100;
        private static readonly Queue<string> lastMessages = new();
        private static LogViewModel vm = null!;

        public static void Initialize(LogViewModel viewModel)
        {
            vm = viewModel;
        }
        private static string DTN => DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff");

        private static void AddMessage(string message)
        {
            lastMessages.Enqueue(message);
            while (lastMessages.Count > LAST_LOG_ENTRIES_COUNT)
            {
                lastMessages.Dequeue();
            }
            vm.UpdateLastLogs(lastMessages);
        }

        [Conditional("TRACE")]
        public static void T(string message)
        {
            string log = $"[{DTN}] [T] {message}\n";
            File.AppendAllText(Cst.LOG_FILE_PATH, log);
            AddMessage(message);
        }

        public static void I(string message)
        {
            string log = $"[{DTN}] [I] {message}\n";
            AddMessage(message);
        }

        public static void W(string message)
        {
            string log = $"[{DTN}] [W] {message}\n";
            AddMessage(message);
        }

        public static void E(string message)
        {
            string log = $"[{DTN}] [E] {message}\n";
            AddMessage(message);
        }

        public static void C(string message)
        {
            string log = $"[{DTN}] [C] {message}\n";
            AddMessage(message);
        }
    }
}
