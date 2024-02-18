namespace ProcessMonitor.Tests
{
    [TestFixture]
    public class ParseArgumentsTests
    {
        [TestCase(-5, 1, false)]
        [TestCase(5, -1, false)]
        [TestCase(5, 1, true)]
        public void ValidateOptions_ShouldReturnExpectedResult(int maxLifetime, int monitoringFrequency, bool expectedResult)
        {
            // Arrange
            var cmdOptions = new CmdOptions { MaxLifetime = maxLifetime, MonitoringFrequency = monitoringFrequency };

            // Act
            var result = cmdOptions.ValidateOptions();

            // Assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
