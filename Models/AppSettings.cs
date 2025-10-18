using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Newtonsoft.Json;

namespace SpeedoMeter.Models
{
    /// <summary>
    /// Application configuration settings with persistence
    /// </summary>
    public class AppSettings
    {
        private static readonly string ConfigFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SpeedoMeter",
            "settings.json");

        /// <summary>
        /// Window position - Left
        /// </summary>
        public double WindowLeft { get; set; } = 100;

        /// <summary>
        /// Window position - Top
        /// </summary>
        public double WindowTop { get; set; } = 100;

        /// <summary>
        /// Position lock state
        /// </summary>
        public bool IsPositionLocked { get; set; } = false;

        /// <summary>
        /// Text color (hex format)
        /// </summary>
        public string TextColor { get; set; } = "#00FF00";

        /// <summary>
        /// Font family name
        /// </summary>
        public string FontFamily { get; set; } = "Consolas";

        /// <summary>
        /// Font size in pixels
        /// </summary>
        public double FontSize { get; set; } = 11;

        /// <summary>
        /// Display unit (MB/s or Mbps)
        /// </summary>
        public SpeedUnit DisplayUnit { get; set; } = SpeedUnit.MBps;

        /// <summary>
        /// Enable auto-start with Windows
        /// </summary>
        public bool AutoStart { get; set; } = false;

        /// <summary>
        /// Update interval in milliseconds
        /// </summary>
        public int UpdateIntervalMs { get; set; } = 1;

        /// <summary>
        /// Show upload speed
        /// </summary>
        public bool ShowUploadSpeed { get; set; } = true;

        /// <summary>
        /// Show download speed
        /// </summary>
        public bool ShowDownloadSpeed { get; set; } = true;

        /// <summary>
        /// Number of decimal places
        /// </summary>
        public int DecimalPlaces { get; set; } = 1;

        /// <summary>
        /// Background opacity (0-1)
        /// </summary>
        public double BackgroundOpacity { get; set; } = 0.0;

        /// <summary>
        /// Load settings from file
        /// </summary>
        public static AppSettings Load()
        {
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    string json = File.ReadAllText(ConfigFilePath);
                    var settings = JsonConvert.DeserializeObject<AppSettings>(json);
                    return settings ?? new AppSettings();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
            }

            return new AppSettings();
        }

        /// <summary>
        /// Save settings to file
        /// </summary>
        public void Save()
        {
            try
            {
                string directory = Path.GetDirectoryName(ConfigFilePath)!;
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(ConfigFilePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}");
            }
        }

        /// <summary>
        /// Get Color from hex string
        /// </summary>
        public Color GetColor()
        {
            try
            {
                return (Color)ColorConverter.ConvertFromString(TextColor);
            }
            catch
            {
                return Colors.Lime;
            }
        }

        /// <summary>
        /// Get SolidColorBrush from settings
        /// </summary>
        public SolidColorBrush GetBrush()
        {
            return new SolidColorBrush(GetColor());
        }
    }

    /// <summary>
    /// Speed display units
    /// </summary>
    public enum SpeedUnit
    {
        /// <summary>
        /// Megabytes per second
        /// </summary>
        MBps,

        /// <summary>
        /// Megabits per second
        /// </summary>
        Mbps
    }

    /// <summary>
    /// Predefined color schemes
    /// </summary>
    public static class ColorSchemes
    {
        public static readonly string[] Colors = new[]
        {
            "#00FF00", // Bright Green
            "#00FFFF", // Cyan
            "#FF00FF", // Magenta
            "#FFFF00", // Yellow
            "#FF6600", // Orange
            "#FFFFFF", // White
            "#FF0000", // Red
            "#0099FF", // Sky Blue
            "#9900FF", // Purple
            "#00FF99", // Mint Green
            "#FF99CC", // Pink
            "#CCCCCC"  // Light Gray
        };

        public static string GetColorName(string hex)
        {
            return hex switch
            {
                "#00FF00" => "Bright Green",
                "#00FFFF" => "Cyan",
                "#FF00FF" => "Magenta",
                "#FFFF00" => "Yellow",
                "#FF6600" => "Orange",
                "#FFFFFF" => "White",
                "#FF0000" => "Red",
                "#0099FF" => "Sky Blue",
                "#9900FF" => "Purple",
                "#00FF99" => "Mint Green",
                "#FF99CC" => "Pink",
                "#CCCCCC" => "Light Gray",
                _ => "Custom"
            };
        }
    }
}
