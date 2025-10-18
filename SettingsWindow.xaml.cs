using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SpeedoMeter.Models;
using SpeedoMeter.Services;

namespace SpeedoMeter
{
    /// <summary>
    /// Settings window for customization options
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly AppSettings _settings;
        private bool _isInitializing = true;

        public event EventHandler? SettingsChanged;

        public SettingsWindow(AppSettings settings)
        {
            InitializeComponent();
            _settings = settings;
            LoadSettings();
            _isInitializing = false;
        }

        /// <summary>
        /// Load current settings into UI controls
        /// </summary>
        private void LoadSettings()
        {
            // Load color schemes
            foreach (var color in ColorSchemes.Colors)
            {
                var item = new ComboBoxItem
                {
                    Content = ColorSchemes.GetColorName(color),
                    Tag = color,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color))
                };
                ColorComboBox.Items.Add(item);

                if (color == _settings.TextColor)
                {
                    ColorComboBox.SelectedItem = item;
                }
            }

            if (ColorComboBox.SelectedItem == null && ColorComboBox.Items.Count > 0)
            {
                ColorComboBox.SelectedIndex = 0;
            }

            // Load font families
            var fonts = Fonts.SystemFontFamilies.OrderBy(f => f.Source).ToList();
            var monospaceFonts = new[] { "Consolas", "Courier New", "Lucida Console", "Cascadia Code", "Cascadia Mono" };

            // Add monospace fonts first
            foreach (var fontName in monospaceFonts)
            {
                var font = fonts.FirstOrDefault(f => f.Source == fontName);
                if (font != null)
                {
                    var item = new ComboBoxItem
                    {
                        Content = font.Source,
                        FontFamily = font
                    };
                    FontFamilyComboBox.Items.Add(item);

                    if (font.Source == _settings.FontFamily)
                    {
                        FontFamilyComboBox.SelectedItem = item;
                    }
                }
            }

            // Add separator
            FontFamilyComboBox.Items.Add(new Separator());

            // Add remaining fonts
            foreach (var font in fonts.Where(f => !monospaceFonts.Contains(f.Source)))
            {
                var item = new ComboBoxItem
                {
                    Content = font.Source,
                    FontFamily = font
                };
                FontFamilyComboBox.Items.Add(item);

                if (font.Source == _settings.FontFamily)
                {
                    FontFamilyComboBox.SelectedItem = item;
                }
            }

            if (FontFamilyComboBox.SelectedItem == null && FontFamilyComboBox.Items.Count > 0)
            {
                FontFamilyComboBox.SelectedIndex = 0;
            }

            // Load other settings
            FontSizeSlider.Value = _settings.FontSize;
            FontSizeLabel.Text = _settings.FontSize.ToString("F0");

            OpacitySlider.Value = _settings.BackgroundOpacity;
            OpacityLabel.Text = _settings.BackgroundOpacity.ToString("F1");

            UnitComboBox.SelectedIndex = _settings.DisplayUnit == SpeedUnit.MBps ? 0 : 1;

            DecimalPlacesSlider.Value = _settings.DecimalPlaces;
            DecimalPlacesLabel.Text = _settings.DecimalPlaces.ToString();

            ShowDownloadCheckBox.IsChecked = _settings.ShowDownloadSpeed;
            ShowUploadCheckBox.IsChecked = _settings.ShowUploadSpeed;

            UpdateIntervalSlider.Value = _settings.UpdateIntervalMs;
            UpdateIntervalLabel.Text = _settings.UpdateIntervalMs.ToString();

            AutoStartCheckBox.IsChecked = _settings.AutoStart;
        }

        /// <summary>
        /// Save settings and close window
        /// </summary>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate at least one speed is shown
            if (!_settings.ShowDownloadSpeed && !_settings.ShowUploadSpeed)
            {
                MessageBox.Show("At least one speed (Download or Upload) must be enabled.",
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _settings.Save();
            SettingsChanged?.Invoke(this, EventArgs.Empty);
            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Cancel without saving
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// Reset to default settings
        /// </summary>
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Reset all settings to defaults?",
                "Confirm Reset",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var defaults = new AppSettings();

                _settings.TextColor = defaults.TextColor;
                _settings.FontFamily = defaults.FontFamily;
                _settings.FontSize = defaults.FontSize;
                _settings.BackgroundOpacity = defaults.BackgroundOpacity;
                _settings.DisplayUnit = defaults.DisplayUnit;
                _settings.DecimalPlaces = defaults.DecimalPlaces;
                _settings.ShowDownloadSpeed = defaults.ShowDownloadSpeed;
                _settings.ShowUploadSpeed = defaults.ShowUploadSpeed;
                _settings.UpdateIntervalMs = defaults.UpdateIntervalMs;

                _isInitializing = true;
                LoadSettings();
                _isInitializing = false;
            }
        }

        private void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isInitializing || ColorComboBox.SelectedItem is not ComboBoxItem item) return;
            _settings.TextColor = item.Tag as string ?? "#00FF00";
        }

        private void FontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isInitializing || FontFamilyComboBox.SelectedItem is not ComboBoxItem item) return;
            _settings.FontFamily = item.Content as string ?? "Consolas";
        }

        private void FontSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isInitializing) return;
            _settings.FontSize = FontSizeSlider.Value;
            FontSizeLabel.Text = FontSizeSlider.Value.ToString("F0");
        }

        private void OpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isInitializing) return;
            _settings.BackgroundOpacity = OpacitySlider.Value;
            OpacityLabel.Text = OpacitySlider.Value.ToString("F1");
        }

        private void UnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isInitializing) return;
            _settings.DisplayUnit = UnitComboBox.SelectedIndex == 0 ? SpeedUnit.MBps : SpeedUnit.Mbps;
        }

        private void DecimalPlacesSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isInitializing) return;
            _settings.DecimalPlaces = (int)DecimalPlacesSlider.Value;
            DecimalPlacesLabel.Text = DecimalPlacesSlider.Value.ToString("F0");
        }

        private void ShowDownloadCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (_isInitializing) return;
            _settings.ShowDownloadSpeed = ShowDownloadCheckBox.IsChecked ?? true;
        }

        private void ShowUploadCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (_isInitializing) return;
            _settings.ShowUploadSpeed = ShowUploadCheckBox.IsChecked ?? true;
        }

        private void UpdateIntervalSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isInitializing) return;
            _settings.UpdateIntervalMs = (int)UpdateIntervalSlider.Value;
            UpdateIntervalLabel.Text = UpdateIntervalSlider.Value.ToString("F0");
        }

        private void AutoStartCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (_isInitializing) return;
            _settings.AutoStart = AutoStartCheckBox.IsChecked ?? false;
        }
    }
}
