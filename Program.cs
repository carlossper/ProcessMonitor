using CommandLine;
using NLog;

namespace ProcessMonitor
{
    public class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// I honestly think I may have over engineered this but...
        /// I wanted to ensure proper console application exit occurred
        /// Even when waiting for the polling intervals.
        /// </summary>
        public static async Task Main(string[] args)
        {
            //Used for brevity and argument parsing
            Parser.Default.ParseArguments<CmdOptions>(args)
                .WithParsed(async options =>
                {
                    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                    var monitoringTask = new MonitoringService(logger)
                        .MonitorProcessAsync(options.ProcessName, options.MaxLifetime, options.MonitoringFrequency, cancellationTokenSource.Token);

                    while (true)
                    {
                        if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
                        {
                            //Cancel the monitoring task when 'Q' is pressed
                            cancellationTokenSource.Cancel();
                            break;
                        }
                    }

                    await monitoringTask;
                });
        }
    }
}