using NLog;
using System.ComponentModel;
using System.Diagnostics;

namespace ProcessMonitor
{
    public class MonitoringService
    {
        private readonly ILogger logger;

        public MonitoringService(ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Monitors all <paramref name="processName"/> for execution time over <paramref name="maxLifetimeInMinutes"/>.
        /// Monitoring is done with an interval of <paramref name="pollingFrequencyInMinutes"/>
        /// </summary>
        /// <param name="processName"/>
        /// <param name="maxLifetimeInMinutes"/>
        /// <param name="pollingFrequencyInMinutes">The interval between monitoring operations.</param>
        /// <param name="cancellationToken"/>
        /// <returns>Process monitoring Task</returns>
        public async Task MonitorProcessAsync(string processName, double maxLifetimeInMinutes, double pollingFrequencyInMinutes, CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    //Ensure all polling operations check processes created in the meantime
                    Process[] allProcessesByName = Process.GetProcessesByName(processName);

                    foreach (var process in allProcessesByName)
                    {
                        if (!process.HasExited)
                        {
                            HandleProcessExit(maxLifetimeInMinutes, process);
                        }
                        else
                        {
                            logger.Info($"PID: {process.Id} has exited before our last polling operation.");
                        }
                    }

                    //Delay asynchronously to allow cancellation between polling operations
                    await Task.Delay(TimeSpan.FromMinutes(pollingFrequencyInMinutes), cancellationToken);
                }
            }
            catch (TaskCanceledException)
            {
                logger.Info("Monitoring task was canceled by pressing 'Q'.");
            }
        }

        /// <summary>
        /// Handles <paramref name="process"/> exit after <paramref name="maxLifetimeInMinutes"/>
        /// </summary>
        /// <param name="maxLifetimeInMinutes"/>
        /// <param name="process"/>
        private void HandleProcessExit(double maxLifetimeInMinutes, Process process)
        {
            if ((DateTime.Now - process.StartTime).TotalMinutes > maxLifetimeInMinutes)
            {
                try
                {
                    logger.Info($"PID: {process.Id} was killed.");
                    process.Kill();
                }
                catch (Win32Exception e)
                {
                    HandleWin32Exception(e, process.Id);
                }
            }
        }

        /// <summary>
        /// Specifically handles Win32 Exceptions that are thrown by Kill() implementation.
        /// </summary>
        /// <param name="ex"/>
        /// <param name="processId"/>
        private void HandleWin32Exception(Win32Exception e, int processId)
        {
            logger.Error($"Error killing process with ID {processId}: {e.Message}");
            logger.Error($"Win32 Error Code: {e.NativeErrorCode}");
        }
    }
}