using CommandLine;

namespace ProcessMonitor
{
    public class CmdOptions
    {
        [Value(0, Required = true, HelpText = "Name of the process to monitor.")]
        public string ProcessName { get; set; }

        [Value(1, Required = true, HelpText = "Maximum lifetime PrcessName in minutes.")]
        public double MaxLifetime { get; set; }

        [Value(2, Required = true, HelpText = "Monitoring frequency in minutes.")]
        public double MonitoringFrequency { get; set; }

        public bool ValidateOptions()
        {
            if (MaxLifetime < 0)
            {
                Console.WriteLine("Error: MaxLifetime must be a non-negative value.");
                return false;
            }

            if (MonitoringFrequency < 0)
            {
                Console.WriteLine("Error: MonitoringFrequency must be a non-negative value.");
                return false;
            }

            return true;
        }
    }
}
