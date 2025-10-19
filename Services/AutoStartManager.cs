using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace SpeedoMeter.Services
{
    /// <summary>
    /// Manages Windows startup registry entries for auto-start functionality
    /// </summary>
    public static class AutoStartManager
    {
        private const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string AppName = "SpeedoMeter";

        /// <summary>
        /// Enable auto-start with Windows
        /// </summary>
        public static bool EnableAutoStart()
        {
            try
            {
                // For single-file apps, use AppContext.BaseDirectory instead of Assembly.Location
                string exePath = Process.GetCurrentProcess().MainModule?.FileName ?? 
                                 Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.exe");

                // Handle .dll case for .NET Core/5+
                if (exePath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                {
                    exePath = exePath.Replace(".dll", ".exe");
                }

                if (!File.Exists(exePath))
                {
                    Debug.WriteLine($"Executable not found: {exePath}");
                    return false;
                }

                using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, true))
                {
                    if (key != null)
                    {
                        key.SetValue(AppName, $"\"{exePath}\"");
                        Debug.WriteLine($"Auto-start enabled: {exePath}");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error enabling auto-start: {ex.Message}");
            }

            return false;
        }

        /// <summary>
        /// Disable auto-start with Windows
        /// </summary>
        public static bool DisableAutoStart()
        {
            try
            {
                using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, true))
                {
                    if (key != null)
                    {
                        if (key.GetValue(AppName) != null)
                        {
                            key.DeleteValue(AppName);
                            Debug.WriteLine("Auto-start disabled");
                        }
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error disabling auto-start: {ex.Message}");
            }

            return false;
        }

        /// <summary>
        /// Check if auto-start is currently enabled
        /// </summary>
        public static bool IsAutoStartEnabled()
        {
            try
            {
                using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, false))
                {
                    if (key != null)
                    {
                        object? value = key.GetValue(AppName);
                        return value != null;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking auto-start status: {ex.Message}");
            }

            return false;
        }

        /// <summary>
        /// Toggle auto-start state
        /// </summary>
        public static bool ToggleAutoStart()
        {
            if (IsAutoStartEnabled())
            {
                return DisableAutoStart();
            }
            else
            {
                return EnableAutoStart();
            }
        }
    }
}
