using NLog;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace ProcessMonitor
{
    public class Monitor
    {
        private readonly ILogger logger;

        public Monitor(ILogger logger)
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
        public void MonitorProcessSync(string processName, int maxLifetimeInMinutes, int pollingFrequencyInMinutes)
        {
            while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q))
            {
                // Ensure all polling operations check processes created in the meantime
                Process[] allProcessesByName = Process.GetProcessesByName(processName);

                HandleProcessExit(maxLifetimeInMinutes, allProcessesByName);

                // Blocking main thread as we antecipate no interactions for a synchronous version
                Thread.Sleep(TimeSpan.FromMinutes(pollingFrequencyInMinutes));
            }
        }

        /// <summary>
        /// Handles <paramref name="allProcessesByName"/> exits after <paramref name="maxLifetimeInMinutes"/>
        /// </summary>
        /// <param name="maxLifetimeInMinutes"/>
        /// <param name="allProcessesByName"/>
        private void HandleProcessExit(int maxLifetimeInMinutes, Process[] allProcessesByName)
        {
            foreach (var process in allProcessesByName.ToList())
            {
                if (!process.HasExited)
                {
                    KillProcessOverThreshold(maxLifetimeInMinutes, process);
                }
                else
                {
                    logger.Info($"PID: {process.Id} has exited before our last polling operation.");
                }
            }
        }

        /// <summary>
        /// Kills any <paramref name="process"/> that is running for longer than <paramref name="maxLifetimeInMinutes"/>.
        /// </summary>
        /// <param name="maxLifetimeInMinutes"/>
        /// <param name="process"/>
        private void KillProcessOverThreshold(int maxLifetimeInMinutes, Process process)
        {
            if ((DateTime.Now - process.StartTime).TotalSeconds > maxLifetimeInMinutes)
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
