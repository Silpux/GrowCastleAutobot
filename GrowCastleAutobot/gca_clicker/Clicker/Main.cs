using gca_clicker.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace gca_clicker
{
    public partial class MainWindow : Window
    {

        private bool backgroundMode;

        private nint hwnd;

        private Bitmap currentScreen;

        private void WorkerLoop()
        {
            try
            {
                while (true)
                {
                    //Dispatcher.Invoke(() => InfoLabel.Content = "Thread running at: " + DateTime.Now.ToString("HH:mm:ss.fff"));
                    //C();

                    Lclick(50, 150);
                    Wait(1000);

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
