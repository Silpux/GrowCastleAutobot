using gca.Classes;
using gca.Script;
using System.Configuration;
using System.Data;
using System.Windows;

namespace gca
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {

#if !DEBUG

        private static Mutex mutex = null!;
        private static bool ownsMutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            const string mutexName = "GrowCastleAutobot_Mutex";
            ownsMutex = false;

            try
            {
                mutex = new Mutex(true, mutexName, out ownsMutex);

                if (!ownsMutex)
                {
                    Log.E("App is already running");
                    IntPtr hWnd = WinAPI.FindWindow(null!, Cst.APP_TITLE);
                    if (hWnd != IntPtr.Zero)
                    {
                        WinAPI.SetForegroundWindow(hWnd);
                    }
                    Shutdown();
                    return;
                }
            }
            catch (Exception ex)
            {
                IntPtr hWnd = WinAPI.FindWindow(null!, Cst.APP_TITLE);
                if (hWnd != IntPtr.Zero)
                {
                    WinAPI.SetForegroundWindow(hWnd);
                }
                Log.E("Mutex error: " + ex.Message);
                Shutdown();
                return;
            }

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (ownsMutex && mutex != null)
            {
                mutex.ReleaseMutex();
                mutex.Dispose();
            }

            base.OnExit(e);
        }
#endif
    }

}
