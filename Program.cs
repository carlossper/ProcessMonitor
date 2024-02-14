using CommandLine;
using NLog;

namespace ProcessMonitor
{
    public class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CmdOptions>(args)
                .WithParsed(options =>
                {
                    Monitor processMonitor = new(logger);
                    processMonitor.MonitorProcessSync(options.ProcessName, options.MaxLifetime, options.MonitoringFrequency);
                });
        }
    }
}