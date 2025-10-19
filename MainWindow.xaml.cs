using System;
using System.ComponentModel;
using System.Linq;
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

            // Background opacity - FIX: Use MainBorder.Background, not this.Background
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

            // Load custom icon
            try
            {
                string iconPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "app.ico");
                if (System.IO.File.Exists(iconPath))
                {
                    _notifyIcon.Icon = new System.Drawing.Icon(iconPath);
                }
                else
                {
                    // Try to load from embedded resources
                    var iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/Resources/app.ico"));
                    if (iconStream != null)
                    {
                        _notifyIcon.Icon = new System.Drawing.Icon(iconStream.Stream);
                    }
                    else
                    {
                        _notifyIcon.Icon = System.Drawing.SystemIcons.Application;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load icon: {ex.Message}");
                _notifyIcon.Icon = System.Drawing.SystemIcons.Application;
            }

            // Create context menu
            _contextMenu = new System.Windows.Forms.ContextMenuStrip();

            // Settings (main)
            var settingsItem = new System.Windows.Forms.ToolStripMenuItem("⚙ Settings...");
            settingsItem.Click += (s, e) => OpenSettings();
            _contextMenu.Items.Add(settingsItem);

            _contextMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());

            // Quick Configuration submenu
            var configMenu = new System.Windows.Forms.ToolStripMenuItem("⚡ Quick Configuration");

            // Color schemes submenu
            var colorMenu = new System.Windows.Forms.ToolStripMenuItem("🎨 Color Scheme");
            string[] colorNames = { "Bright Green", "Cyan", "Magenta", "Yellow", "Orange",
                                   "White", "Red", "Sky Blue", "Purple", "Mint Green", "Pink", "Light Gray" };
            for (int i = 0; i < ColorSchemes.Colors.Length; i++)
            {
                string color = ColorSchemes.Colors[i];
                string name = colorNames[i];
                var colorItem = new System.Windows.Forms.ToolStripMenuItem(name)
                {
                    Checked = _settings.TextColor == color,
                    Tag = color
                };
                colorItem.Click += (s, e) =>
                {
                    var item = (System.Windows.Forms.ToolStripMenuItem)s!;
                    _settings.TextColor = item.Tag!.ToString()!;
                    _settings.Save();
                    ApplySettings();
                    UpdateColorMenuChecks();
                };
                colorMenu.DropDownItems.Add(colorItem);
            }
            configMenu.DropDownItems.Add(colorMenu);

            // Font size submenu
            var fontSizeMenu = new System.Windows.Forms.ToolStripMenuItem("📏 Font Size");
            int[] fontSizes = { 8, 9, 10, 11, 12, 13, 14, 15, 16 };
            foreach (int size in fontSizes)
            {
                var sizeItem = new System.Windows.Forms.ToolStripMenuItem($"{size} px")
                {
                    Checked = Math.Abs(_settings.FontSize - size) < 0.5,
                    Tag = size
                };
                sizeItem.Click += (s, e) =>
                {
                    var item = (System.Windows.Forms.ToolStripMenuItem)s!;
                    _settings.FontSize = (int)item.Tag!;
                    _settings.Save();
                    ApplySettings();
                    UpdateFontSizeMenuChecks();
                };
                fontSizeMenu.DropDownItems.Add(sizeItem);
            }
            configMenu.DropDownItems.Add(fontSizeMenu);

            // Decimal places submenu
            var decimalMenu = new System.Windows.Forms.ToolStripMenuItem("🔢 Decimal Places");
            for (int i = 0; i <= 3; i++)
            {
                int places = i;
                var decimalItem = new System.Windows.Forms.ToolStripMenuItem($"{places} decimal{(places == 1 ? "" : "s")}")
                {
                    Checked = _settings.DecimalPlaces == places,
                    Tag = places
                };
                decimalItem.Click += (s, e) =>
                {
                    var item = (System.Windows.Forms.ToolStripMenuItem)s!;
                    _settings.DecimalPlaces = (int)item.Tag!;
                    _settings.Save();
                    UpdateDecimalMenuChecks();
                };
                decimalMenu.DropDownItems.Add(decimalItem);
            }
            configMenu.DropDownItems.Add(decimalMenu);

            // Opacity submenu
            var opacityMenu = new System.Windows.Forms.ToolStripMenuItem("👁 Background Opacity");
            double[] opacities = { 0.0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0 };
            foreach (double opacity in opacities)
            {
                var opacityItem = new System.Windows.Forms.ToolStripMenuItem($"{(int)(opacity * 100)}%")
                {
                    Checked = Math.Abs(_settings.BackgroundOpacity - opacity) < 0.05,
                    Tag = opacity
                };
                opacityItem.Click += (s, e) =>
                {
                    var item = (System.Windows.Forms.ToolStripMenuItem)s!;
                    _settings.BackgroundOpacity = (double)item.Tag!;
                    _settings.Save();
                    ApplySettings();
                    UpdateOpacityMenuChecks();
                };
                opacityMenu.DropDownItems.Add(opacityItem);
            }
            configMenu.DropDownItems.Add(opacityMenu);

            // Refresh Rate submenu
            var refreshRateMenu = new System.Windows.Forms.ToolStripMenuItem("⚡ Refresh Rate");
            var refreshRates = new[]
            {
                (Label: "10 ms (Very Fast)", Value: 10),
                (Label: "40 ms (Fast)", Value: 40),
                (Label: "80 ms (Quick)", Value: 80),
                (Label: "1 second (Default)", Value: 1000),
                (Label: "2 seconds (Slow)", Value: 2000),
                (Label: "3 seconds (Slower)", Value: 3000),
                (Label: "4 seconds (Very Slow)", Value: 4000),
                (Label: "5 seconds (Slowest)", Value: 5000)
            };
            foreach (var rate in refreshRates)
            {
                var refreshItem = new System.Windows.Forms.ToolStripMenuItem(rate.Label)
                {
                    Checked = _settings.UpdateIntervalMs == rate.Value,
                    Tag = rate.Value
                };
                refreshItem.Click += (s, e) =>
                {
                    var item = (System.Windows.Forms.ToolStripMenuItem)s!;
                    _settings.UpdateIntervalMs = (int)item.Tag!;
                    _settings.Save();
                    _networkMonitor.UpdateIntervalMs = _settings.UpdateIntervalMs;
                    UpdateRefreshRateMenuChecks();
                };
                refreshRateMenu.DropDownItems.Add(refreshItem);
            }
            configMenu.DropDownItems.Add(refreshRateMenu);

            _contextMenu.Items.Add(configMenu);

            _contextMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());

            // Display options
            var displayMenu = new System.Windows.Forms.ToolStripMenuItem("📊 Display Options");

            var showDownloadItem = new System.Windows.Forms.ToolStripMenuItem("Show Download Speed")
            {
                Checked = _settings.ShowDownloadSpeed,
                CheckOnClick = true
            };
            showDownloadItem.Click += (s, e) =>
            {
                var item = (System.Windows.Forms.ToolStripMenuItem)s!;
                if (!item.Checked && !_settings.ShowUploadSpeed)
                {
                    System.Windows.Forms.MessageBox.Show("At least one speed must be visible!",
                        "SpeedoMeter", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Warning);
                    item.Checked = true;
                    return;
                }
                _settings.ShowDownloadSpeed = item.Checked;
                _settings.Save();
            };
            displayMenu.DropDownItems.Add(showDownloadItem);

            var showUploadItem = new System.Windows.Forms.ToolStripMenuItem("Show Upload Speed")
            {
                Checked = _settings.ShowUploadSpeed,
                CheckOnClick = true
            };
            showUploadItem.Click += (s, e) =>
            {
                var item = (System.Windows.Forms.ToolStripMenuItem)s!;
                if (!item.Checked && !_settings.ShowDownloadSpeed)
                {
                    System.Windows.Forms.MessageBox.Show("At least one speed must be visible!",
                        "SpeedoMeter", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Warning);
                    item.Checked = true;
                    return;
                }
                _settings.ShowUploadSpeed = item.Checked;
                _settings.Save();
            };
            displayMenu.DropDownItems.Add(showUploadItem);

            _contextMenu.Items.Add(displayMenu);

            _contextMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());

            // Position lock
            var lockItem = new System.Windows.Forms.ToolStripMenuItem("🔒 Lock Position")
            {
                Checked = _settings.IsPositionLocked,
                CheckOnClick = true
            };
            lockItem.Click += (s, e) => ToggleLock();
            _contextMenu.Items.Add(lockItem);

            // Toggle unit
            var toggleUnitItem = new System.Windows.Forms.ToolStripMenuItem("🔄 Toggle Unit (MB/s ↔ Mbps)");
            toggleUnitItem.Click += (s, e) => ToggleUnit();
            _contextMenu.Items.Add(toggleUnitItem);

            // Auto-start
            var autoStartItem = new System.Windows.Forms.ToolStripMenuItem("🚀 Auto-start with Windows")
            {
                Checked = _settings.AutoStart,
                CheckOnClick = true
            };
            autoStartItem.Click += (s, e) => ToggleAutoStart();
            _contextMenu.Items.Add(autoStartItem);

            _contextMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());

            // About
            var aboutItem = new System.Windows.Forms.ToolStripMenuItem("ℹ About SpeedoMeter");
            aboutItem.Click += (s, e) => ShowAbout();
            _contextMenu.Items.Add(aboutItem);

            // Exit
            var exitItem = new System.Windows.Forms.ToolStripMenuItem("❌ Exit");
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
        /// Update color menu checks
        /// </summary>
        private void UpdateColorMenuChecks()
        {
            if (_contextMenu == null) return;

            var configMenu = _contextMenu.Items.Cast<System.Windows.Forms.ToolStripItem>()
                .FirstOrDefault(i => i.Text == "⚡ Quick Configuration") as System.Windows.Forms.ToolStripMenuItem;

            if (configMenu != null)
            {
                var colorMenu = configMenu.DropDownItems.Cast<System.Windows.Forms.ToolStripItem>()
                    .FirstOrDefault(i => i.Text == "🎨 Color Scheme") as System.Windows.Forms.ToolStripMenuItem;

                if (colorMenu != null)
                {
                    foreach (System.Windows.Forms.ToolStripMenuItem item in colorMenu.DropDownItems)
                    {
                        item.Checked = item.Tag?.ToString() == _settings.TextColor;
                    }
                }
            }
        }

        /// <summary>
        /// Update font size menu checks
        /// </summary>
        private void UpdateFontSizeMenuChecks()
        {
            if (_contextMenu == null) return;

            var configMenu = _contextMenu.Items.Cast<System.Windows.Forms.ToolStripItem>()
                .FirstOrDefault(i => i.Text == "⚡ Quick Configuration") as System.Windows.Forms.ToolStripMenuItem;

            if (configMenu != null)
            {
                var fontSizeMenu = configMenu.DropDownItems.Cast<System.Windows.Forms.ToolStripItem>()
                    .FirstOrDefault(i => i.Text == "📏 Font Size") as System.Windows.Forms.ToolStripMenuItem;

                if (fontSizeMenu != null)
                {
                    foreach (System.Windows.Forms.ToolStripMenuItem item in fontSizeMenu.DropDownItems)
                    {
                        item.Checked = Math.Abs(_settings.FontSize - (int)item.Tag!) < 0.5;
                    }
                }
            }
        }

        /// <summary>
        /// Update decimal menu checks
        /// </summary>
        private void UpdateDecimalMenuChecks()
        {
            if (_contextMenu == null) return;

            var configMenu = _contextMenu.Items.Cast<System.Windows.Forms.ToolStripItem>()
                .FirstOrDefault(i => i.Text == "⚡ Quick Configuration") as System.Windows.Forms.ToolStripMenuItem;

            if (configMenu != null)
            {
                var decimalMenu = configMenu.DropDownItems.Cast<System.Windows.Forms.ToolStripItem>()
                    .FirstOrDefault(i => i.Text == "🔢 Decimal Places") as System.Windows.Forms.ToolStripMenuItem;

                if (decimalMenu != null)
                {
                    foreach (System.Windows.Forms.ToolStripMenuItem item in decimalMenu.DropDownItems)
                    {
                        item.Checked = _settings.DecimalPlaces == (int)item.Tag!;
                    }
                }
            }
        }

        /// <summary>
        /// Update opacity menu checks
        /// </summary>
        private void UpdateOpacityMenuChecks()
        {
            if (_contextMenu == null) return;

            var configMenu = _contextMenu.Items.Cast<System.Windows.Forms.ToolStripItem>()
                .FirstOrDefault(i => i.Text == "⚡ Quick Configuration") as System.Windows.Forms.ToolStripMenuItem;

            if (configMenu != null)
            {
                var opacityMenu = configMenu.DropDownItems.Cast<System.Windows.Forms.ToolStripItem>()
                    .FirstOrDefault(i => i.Text == "👁 Background Opacity") as System.Windows.Forms.ToolStripMenuItem;

                if (opacityMenu != null)
                {
                    foreach (System.Windows.Forms.ToolStripMenuItem item in opacityMenu.DropDownItems)
                    {
                        item.Checked = Math.Abs(_settings.BackgroundOpacity - (double)item.Tag!) < 0.05;
                    }
                }
            }
        }

        /// <summary>
        /// Update refresh rate menu checks
        /// </summary>
        private void UpdateRefreshRateMenuChecks()
        {
            if (_contextMenu == null) return;

            var configMenu = _contextMenu.Items.Cast<System.Windows.Forms.ToolStripItem>()
                .FirstOrDefault(i => i.Text == "⚡ Quick Configuration") as System.Windows.Forms.ToolStripMenuItem;

            if (configMenu != null)
            {
                var refreshRateMenu = configMenu.DropDownItems.Cast<System.Windows.Forms.ToolStripItem>()
                    .FirstOrDefault(i => i.Text == "⚡ Refresh Rate") as System.Windows.Forms.ToolStripMenuItem;

                if (refreshRateMenu != null)
                {
                    foreach (System.Windows.Forms.ToolStripMenuItem item in refreshRateMenu.DropDownItems)
                    {
                        item.Checked = _settings.UpdateIntervalMs == (int)item.Tag!;
                    }
                }
            }
        }

        /// <summary>
        /// Show about dialog
        /// </summary>
        private void ShowAbout()
        {
            string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";
            string message = $"SpeedoMeter v{version}\n\n" +
                           "Real-time network speed monitor for Windows\n\n" +
                           "Features:\n" +
                           "• Ultra-responsive 1-5ms updates\n" +
                           "• MB/s and Mbps display modes\n" +
                           "• 12 color schemes\n" +
                           "• Customizable fonts and sizes\n" +
                           "• Drag-and-drop positioning\n" +
                           "• Auto-start with Windows\n\n" +
                           "© 2025 SpeedoMeter\n" +
                           "MIT License";

            System.Windows.Forms.MessageBox.Show(message, "About SpeedoMeter",
                System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Information);
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
