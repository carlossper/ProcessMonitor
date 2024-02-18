using CommandLine;
using NLog;
using System;
using System.Threading;
using System.Threading.Tasks;

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
            CmdOptions options = null;

            Parser.Default.ParseArguments<CmdOptions>(args)
                .WithParsed(parsedOptions =>
                {
                    options = parsedOptions;
                })
                .WithNotParsed(errors =>
                {
                    Console.WriteLine("Error parsing command-line arguments:");
                    foreach (var error in errors)
                    {
                        Console.WriteLine(error);
                    }
                });

            if (options == null || !options.ValidateOptions())
            {
                Console.WriteLine("Invalid arguments. Please check the provided values.");
                return;
            }

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            var monitoringTask = new MonitoringService(logger)
                .MonitorProcessAsync(options.ProcessName, options.MaxLifetime, options.MonitoringFrequency, cancellationTokenSource.Token);

            try
            {
                while (!monitoringTask.IsCompleted)
                {
                    if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
                    {
                        //Cancel monitoring task when q is pressed
                        cancellationTokenSource.Cancel();
                        break;
                    }

                    //We don't need ultra fast reponsiveness here 
                    await Task.Delay(50);
                }
            }
            finally
            {
                await monitoringTask; //Always make sure the monitoring task is termianated
            }
        }
    }
}