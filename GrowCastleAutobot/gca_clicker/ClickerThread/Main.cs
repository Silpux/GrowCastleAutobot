using gca_clicker.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace gca_clicker
{
    public partial class MainWindow : Window
    {
        private void WorkerLoop()
        {
            try
            {
                while (true)
                {
                    Dispatcher.Invoke(() => InfoLabel.Content = "Thread running at: " + DateTime.Now.ToString("HH:mm:ss.fff"));
                    C();
                }
            }
            catch (OperationCanceledException)
            {
                if (!Dispatcher.HasShutdownStarted && !Dispatcher.HasShutdownFinished)
                {
                    Dispatcher.Invoke(() => InfoLabel.Content = "Thread interrupted");
                }
                isRunning = false;
                clickerThread = null!;
            }
        }

    }
}
