using System;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace SpeedoMeter.Services
{
    /// <summary>
    /// High-performance network speed monitoring service with 1-5ms update intervals
    /// </summary>
    public class NetworkMonitor : IDisposable
    {
        private readonly Stopwatch _stopwatch = new();
        private long _lastBytesReceived;
        private long _lastBytesSent;
        private CancellationTokenSource? _cancellationTokenSource;
        private Task? _monitoringTask;
        private NetworkInterface? _activeInterface;

        /// <summary>
        /// Current download speed in bytes per second
        /// </summary>
        public double DownloadBytesPerSecond { get; private set; }

        /// <summary>
        /// Current upload speed in bytes per second
        /// </summary>
        public double UploadBytesPerSecond { get; private set; }

        /// <summary>
        /// Download speed in megabytes per second
        /// </summary>
        public double DownloadMBps => DownloadBytesPerSecond / (1024 * 1024);

        /// <summary>
        /// Upload speed in megabytes per second
        /// </summary>
        public double UploadMBps => UploadBytesPerSecond / (1024 * 1024);

        /// <summary>
        /// Download speed in megabits per second
        /// </summary>
        public double DownloadMbps => (DownloadBytesPerSecond * 8) / (1000 * 1000);

        /// <summary>
        /// Upload speed in megabits per second
        /// </summary>
        public double UploadMbps => (UploadBytesPerSecond * 8) / (1000 * 1000);

        /// <summary>
        /// Update interval in milliseconds (1-5ms for ultra-responsive monitoring)
        /// </summary>
        public int UpdateIntervalMs { get; set; } = 1;

        /// <summary>
        /// Event raised when speed values are updated
        /// </summary>
        public event EventHandler? SpeedUpdated;

        /// <summary>
        /// Start monitoring network speeds
        /// </summary>
        public void Start()
        {
            if (_monitoringTask != null)
                return;

            _cancellationTokenSource = new CancellationTokenSource();
            _activeInterface = GetActiveNetworkInterface();

            if (_activeInterface == null)
            {
                Debug.WriteLine("No active network interface found");
                return;
            }

            // Initialize baseline values
            var stats = _activeInterface.GetIPv4Statistics();
            _lastBytesReceived = stats.BytesReceived;
            _lastBytesSent = stats.BytesSent;
            _stopwatch.Start();

            // Start monitoring task
            _monitoringTask = Task.Run(MonitoringLoop, _cancellationTokenSource.Token);
        }

        /// <summary>
        /// Stop monitoring network speeds
        /// </summary>
        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
            _stopwatch.Stop();
            _monitoringTask?.Wait(100);
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
            _monitoringTask = null;
        }

        /// <summary>
        /// Main monitoring loop with high-precision timing
        /// </summary>
        private async Task MonitoringLoop()
        {
            var token = _cancellationTokenSource!.Token;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    // Check if interface is still valid, refresh if needed
                    if (_activeInterface == null || !IsInterfaceActive(_activeInterface))
                    {
                        _activeInterface = GetActiveNetworkInterface();
                        if (_activeInterface == null)
                        {
                            await Task.Delay(1000, token);
                            continue;
                        }
                    }

                    // Get current statistics
                    var stats = _activeInterface.GetIPv4Statistics();
                    long currentBytesReceived = stats.BytesReceived;
                    long currentBytesSent = stats.BytesSent;

                    // Calculate elapsed time in seconds
                    double elapsedSeconds = _stopwatch.Elapsed.TotalSeconds;

                    if (elapsedSeconds > 0)
                    {
                        // Calculate bytes transferred
                        long receivedDelta = currentBytesReceived - _lastBytesReceived;
                        long sentDelta = currentBytesSent - _lastBytesSent;

                        // Calculate speeds (bytes per second)
                        DownloadBytesPerSecond = receivedDelta / elapsedSeconds;
                        UploadBytesPerSecond = sentDelta / elapsedSeconds;

                        // Ensure non-negative values
                        if (DownloadBytesPerSecond < 0) DownloadBytesPerSecond = 0;
                        if (UploadBytesPerSecond < 0) UploadBytesPerSecond = 0;

                        // Raise update event
                        SpeedUpdated?.Invoke(this, EventArgs.Empty);
                    }

                    // Update baseline values
                    _lastBytesReceived = currentBytesReceived;
                    _lastBytesSent = currentBytesSent;
                    _stopwatch.Restart();

                    // Wait for next update interval
                    await Task.Delay(UpdateIntervalMs, token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Network monitoring error: {ex.Message}");
                    await Task.Delay(1000, token);
                }
            }
        }

        /// <summary>
        /// Get the active network interface with internet connectivity
        /// </summary>
        private NetworkInterface? GetActiveNetworkInterface()
        {
            try
            {
                var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(ni => ni.OperationalStatus == OperationalStatus.Up &&
                                ni.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                                ni.NetworkInterfaceType != NetworkInterfaceType.Tunnel &&
                                ni.GetIPv4Statistics().BytesReceived > 0)
                    .OrderByDescending(ni => ni.GetIPv4Statistics().BytesReceived)
                    .ToList();

                // Prefer Ethernet, then Wireless, then others
                return interfaces.FirstOrDefault(ni => ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet) ??
                       interfaces.FirstOrDefault(ni => ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) ??
                       interfaces.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting network interface: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Check if network interface is still active
        /// </summary>
        private bool IsInterfaceActive(NetworkInterface ni)
        {
            try
            {
                return ni.OperationalStatus == OperationalStatus.Up;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            Stop();
            _stopwatch?.Stop();
        }
    }
}
