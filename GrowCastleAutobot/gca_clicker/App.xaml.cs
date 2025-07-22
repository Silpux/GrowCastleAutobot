using gca_clicker.Classes;
using System.Configuration;
using System.Data;
using System.Windows;

namespace gca_clicker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

#if !DEBUG

        private static Mutex mutex;
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
                    MessageBox.Show("App is already running");
                    Shutdown();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Mutex error: " + ex.Message);
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
