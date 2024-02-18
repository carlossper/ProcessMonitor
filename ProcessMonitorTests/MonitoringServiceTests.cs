using Moq;
using NLog;
using System.Diagnostics;

namespace ProcessMonitor.Tests
{
    [TestFixture]
    public class MonitoringServiceTests
    {
        private Mock<ILogger> loggerMock;

        [SetUp]
        public void Setup()
        {
            // Setting up a mock logger for testing
            loggerMock = new Mock<ILogger>();
        }

        [TestCase("notepad", 0.01, 0.05)]
        [TestCase("explorer", 0.01, 0.05)]
        public async Task MonitorProcessAsync_StartProcessAndHandleLifetimeExceeded_ShouldKillProcess(string processName, double maxLifetime, double pollingFrequency)
        {
            // Arrange
            int processId = await RunAndVerifyMonitoring(processName, maxLifetime, pollingFrequency, 10000);

            // Act
            await Task.Delay(5000); // Allow time for the monitoring task to execute

            // Assert
            VerifyProcessKilled(processId);
        }

        private async Task<int> RunAndVerifyMonitoring(string processName, double maxLifetime, double pollingFrequency, int cancellationDelay)
        {
            // Arrange
            Process process = Process.Start(processName);
            Assert.IsNotNull(process, $"Failed to start the process: {processName}");

            int processId = process.Id;

            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(cancellationDelay))
            {
                // Act
                var monitoringService = new MonitoringService(loggerMock.Object);
                await Task.WhenAll(monitoringService.MonitorProcessAsync(processName, maxLifetime, pollingFrequency, cancellationTokenSource.Token), Task.Delay(5000));
            }

            // Cleanup: Ensure the process is terminated after monitoring
            process?.Kill();
            process?.WaitForExit();

            return processId;
        }

        private void VerifyExitMessageLogged(int processId)
        {
            // Assert
            loggerMock.Verify(logger => logger.Info($"PID: {processId} has exited before our last polling operation."), Times.Once);
        }

        private void VerifyProcessKilled(int processId)
        {
            // Assert
            loggerMock.Verify(logger => logger.Info($"PID: {processId} was killed."), Times.Once);
        }
    }
}