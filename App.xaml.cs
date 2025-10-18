using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace SpeedoMeter
{
    /// <summary>
    /// Main application entry point
    /// </summary>
    public partial class App : Application
    {
        private MainWindow? _mainWindow;
        private System.Threading.Mutex? _mutex;

        public App()
        {
            // Global exception handlers
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            DispatcherUnhandledException += OnDispatcherUnhandledException;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Ensure single instance
                bool createdNew;
                _mutex = new System.Threading.Mutex(true, "SpeedoMeter_SingleInstance", out createdNew);
                
                if (!createdNew)
                {
                    MessageBox.Show("SpeedoMeter is already running.", "Already Running",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    _mutex = null; // Don't try to release a mutex we don't own
                    Shutdown();
                    return;
                }

                // Initialize main window
                _mainWindow = new MainWindow();
                _mainWindow.Show();
            }
            catch (Exception ex)
            {
                LogError($"Startup error: {ex.Message}\n{ex.StackTrace}");
                MessageBox.Show($"Failed to start SpeedoMeter:\n\n{ex.Message}\n\nCheck the log file in your AppData folder.",
                    "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                if (_mutex != null)
                {
                    _mutex.ReleaseMutex();
                    _mutex.Dispose();
                    _mutex = null;
                }
            }
            catch (Exception ex)
            {
                // Ignore mutex errors on exit
                System.Diagnostics.Debug.WriteLine($"Mutex release error: {ex.Message}");
            }
            base.OnExit(e);
        }        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            LogError($"Unhandled exception: {exception?.Message}\n{exception?.StackTrace}");
            MessageBox.Show($"A fatal error occurred:\n\n{exception?.Message}\n\nThe application will now close.",
                "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LogError($"Dispatcher exception: {e.Exception.Message}\n{e.Exception.StackTrace}");
            MessageBox.Show($"An error occurred:\n\n{e.Exception.Message}\n\nCheck the log file for details.",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void LogError(string message)
        {
            try
            {
                string logPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "SpeedoMeter", "error.log");
                Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
                File.AppendAllText(logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n\n");
            }
            catch
            {
                // Ignore logging errors
            }
        }
    }
}
