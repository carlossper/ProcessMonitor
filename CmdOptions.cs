using CommandLine;

namespace ProcessMonitor
{
    public class CmdOptions
    {
        [Value(0, Required = true, HelpText = "Name of the process to monitor.")]
        public string ProcessName { get; set; }

        [Value(1, Required = true, HelpText = "Maximum lifetime PrcessName in minutes.")]
        public int MaxLifetime { get; set; }

        [Value(2, Required = true, HelpText = "Monitoring frequency in minutes.")]
        public int MonitoringFrequency { get; set; }
    }
}
