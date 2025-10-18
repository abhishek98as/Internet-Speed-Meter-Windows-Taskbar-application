using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using SpeedoMeter.Models;
using SpeedoMeter.Services;

namespace SpeedoMeter
{
    /// <summary>
    /// Main window displaying network speeds in taskbar
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly NetworkMonitor _networkMonitor;
        private readonly AppSettings _settings;
        private readonly DispatcherTimer _uiUpdateTimer;
        private System.Windows.Forms.NotifyIcon? _notifyIcon;
        private System.Windows.Forms.ContextMenuStrip? _contextMenu;

        public MainWindow()
        {
            InitializeComponent();

            // Load settings
            _settings = AppSettings.Load();
            ApplySettings();

            // Initialize network monitor
            _networkMonitor = new NetworkMonitor
            {
                UpdateIntervalMs = _settings.UpdateIntervalMs
            };
            _networkMonitor.SpeedUpdated += OnSpeedUpdated;

            // UI update timer for smooth rendering
            _uiUpdateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50) // 20 FPS for smooth display
            };
            _uiUpdateTimer.Tick += UpdateUI;
            _uiUpdateTimer.Start();

            // Start monitoring
            _networkMonitor.Start();

            // Setup system tray
            InitializeSystemTray();

            // Sync auto-start setting
            if (_settings.AutoStart && !AutoStartManager.IsAutoStartEnabled())
            {
                AutoStartManager.EnableAutoStart();
            }
        }

        /// <summary>
        /// Apply loaded settings to UI
        /// </summary>
        private void ApplySettings()
        {
            // Position
            Left = _settings.WindowLeft;
            Top = _settings.WindowTop;

            // Font
            SpeedTextBlock.FontFamily = new FontFamily(_settings.FontFamily);
            SpeedTextBlock.FontSize = _settings.FontSize;

            // Color
            SpeedTextBlock.Foreground = _settings.GetBrush();

            // Background opacity
            if (MainBorder.Background is SolidColorBrush brush)
            {
                brush.Opacity = _settings.BackgroundOpacity;
            }
        }

        /// <summary>
        /// Event handler for speed updates from network monitor
        /// </summary>
        private void OnSpeedUpdated(object? sender, EventArgs e)
        {
            // This runs on background thread, UI update happens in UpdateUI timer
        }

        /// <summary>
        /// Update UI with current speeds
        /// </summary>
        private void UpdateUI(object? sender, EventArgs e)
        {
            try
            {
                double downloadSpeed, uploadSpeed;
                string unitText;

                if (_settings.DisplayUnit == SpeedUnit.MBps)
                {
                    downloadSpeed = _networkMonitor.DownloadMBps;
                    uploadSpeed = _networkMonitor.UploadMBps;
                    unitText = "MB/s";
                }
                else
                {
                    downloadSpeed = _networkMonitor.DownloadMbps;
                    uploadSpeed = _networkMonitor.UploadMbps;
                    unitText = "Mbps";
                }

                string format = $"F{_settings.DecimalPlaces}";
                string downloadText = _settings.ShowDownloadSpeed ? $"↓{downloadSpeed.ToString(format)}{unitText}" : "";
                string uploadText = _settings.ShowUploadSpeed ? $"↑{uploadSpeed.ToString(format)}{unitText}" : "";

                string displayText = $"{downloadText} {uploadText}".Trim();

                if (string.IsNullOrWhiteSpace(displayText))
                {
                    displayText = "No data";
                }

                SpeedTextBlock.Text = displayText;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UI update error: {ex.Message}");
            }
        }

        /// <summary>
        /// Initialize system tray icon and context menu
        /// </summary>
        private void InitializeSystemTray()
        {
            _notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                Text = "SpeedoMeter - Network Speed Monitor",
                Visible = true
            };

            // Load icon (using default if custom icon not available)
            try
            {
                _notifyIcon.Icon = System.Drawing.SystemIcons.Application;
            }
            catch
            {
                // Fallback to system icon
            }

            // Create context menu
            _contextMenu = new System.Windows.Forms.ContextMenuStrip();

            var settingsItem = new System.Windows.Forms.ToolStripMenuItem("Settings");
            settingsItem.Click += (s, e) => OpenSettings();
            _contextMenu.Items.Add(settingsItem);

            _contextMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());

            var lockItem = new System.Windows.Forms.ToolStripMenuItem("Lock Position")
            {
                Checked = _settings.IsPositionLocked,
                CheckOnClick = true
            };
            lockItem.Click += (s, e) => ToggleLock();
            _contextMenu.Items.Add(lockItem);

            var toggleUnitItem = new System.Windows.Forms.ToolStripMenuItem("Toggle Unit (MB/s ↔ Mbps)");
            toggleUnitItem.Click += (s, e) => ToggleUnit();
            _contextMenu.Items.Add(toggleUnitItem);

            var autoStartItem = new System.Windows.Forms.ToolStripMenuItem("Auto-start with Windows")
            {
                Checked = _settings.AutoStart,
                CheckOnClick = true
            };
            autoStartItem.Click += (s, e) => ToggleAutoStart();
            _contextMenu.Items.Add(autoStartItem);

            _contextMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());

            var exitItem = new System.Windows.Forms.ToolStripMenuItem("Exit");
            exitItem.Click += (s, e) => ExitApplication();
            _contextMenu.Items.Add(exitItem);

            _notifyIcon.ContextMenuStrip = _contextMenu;
            _notifyIcon.DoubleClick += (s, e) => OpenSettings();
        }

        /// <summary>
        /// Handle window dragging
        /// </summary>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_settings.IsPositionLocked && e.ClickCount == 1)
            {
                try
                {
                    DragMove();
                }
                catch
                {
                    // Ignore drag errors
                }

                // Save new position
                _settings.WindowLeft = Left;
                _settings.WindowTop = Top;
                _settings.Save();
            }
        }

        /// <summary>
        /// Toggle position lock
        /// </summary>
        private void ToggleLock()
        {
            _settings.IsPositionLocked = !_settings.IsPositionLocked;
            _settings.Save();

            if (_contextMenu != null)
            {
                foreach (System.Windows.Forms.ToolStripItem item in _contextMenu.Items)
                {
                    if (item is System.Windows.Forms.ToolStripMenuItem menuItem &&
                        menuItem.Text == "Lock Position")
                    {
                        menuItem.Checked = _settings.IsPositionLocked;
                    }
                }
            }

            System.Windows.Forms.MessageBox.Show(
                _settings.IsPositionLocked ? "Position locked" : "Position unlocked",
                "SpeedoMeter",
                System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Information);
        }

        /// <summary>
        /// Toggle display unit between MB/s and Mbps
        /// </summary>
        private void ToggleUnit()
        {
            _settings.DisplayUnit = _settings.DisplayUnit == SpeedUnit.MBps ? SpeedUnit.Mbps : SpeedUnit.MBps;
            _settings.Save();
        }

        /// <summary>
        /// Toggle auto-start with Windows
        /// </summary>
        private void ToggleAutoStart()
        {
            _settings.AutoStart = !_settings.AutoStart;

            if (_settings.AutoStart)
            {
                AutoStartManager.EnableAutoStart();
            }
            else
            {
                AutoStartManager.DisableAutoStart();
            }

            _settings.Save();

            if (_contextMenu != null)
            {
                foreach (System.Windows.Forms.ToolStripItem item in _contextMenu.Items)
                {
                    if (item is System.Windows.Forms.ToolStripMenuItem menuItem &&
                        menuItem.Text == "Auto-start with Windows")
                    {
                        menuItem.Checked = _settings.AutoStart;
                    }
                }
            }
        }

        /// <summary>
        /// Open settings window
        /// </summary>
        private void OpenSettings()
        {
            var settingsWindow = new SettingsWindow(_settings);
            settingsWindow.SettingsChanged += OnSettingsChanged;
            settingsWindow.ShowDialog();
        }

        /// <summary>
        /// Handle settings changes
        /// </summary>
        private void OnSettingsChanged(object? sender, EventArgs e)
        {
            ApplySettings();
            _networkMonitor.UpdateIntervalMs = _settings.UpdateIntervalMs;
        }

        /// <summary>
        /// Exit application
        /// </summary>
        private void ExitApplication()
        {
            Application.Current.Shutdown();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // Cleanup
            _uiUpdateTimer.Stop();
            _networkMonitor.Stop();
            _networkMonitor.Dispose();
            _notifyIcon?.Dispose();
            _contextMenu?.Dispose();

            base.OnClosing(e);
        }
    }
}
