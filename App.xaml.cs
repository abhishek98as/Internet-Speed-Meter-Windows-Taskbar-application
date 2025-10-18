using System;
using System.Windows;

namespace SpeedoMeter
{
    /// <summary>
    /// Main application entry point
    /// </summary>
    public partial class App : Application
    {
        private MainWindow? _mainWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Ensure single instance
            bool createdNew;
            using (var mutex = new System.Threading.Mutex(true, "SpeedoMeter_SingleInstance", out createdNew))
            {
                if (!createdNew)
                {
                    MessageBox.Show("SpeedoMeter is already running.", "Already Running",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    Shutdown();
                    return;
                }

                // Initialize main window
                _mainWindow = new MainWindow();
                _mainWindow.Show();

                // Keep mutex alive
                GC.KeepAlive(mutex);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }
}
