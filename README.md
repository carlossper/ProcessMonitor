## ProcessMonitor
Basic console app for Process monitoring written in C#, targeting .NET6.


### Features

- **Process Monitoring:** Keep an eye on the execution time of specified processes.
- **Automatic Termination:** Kill processes exceeding a specified maximum lifetime.
- **User-Friendly:** Simple command-line interface for easy configuration.

### Usage

1. Build and run the solution using Visual Studio or your preferred IDE.
2. Follow the on-screen instructions to input process details.
3. Press 'Q' anytime to stop monitoring.

### Configuration

Adjust settings like process name, max lifetime, and monitoring frequency through the command line.

Example:
```bash
dotnet run --processName notepad --maxLifetime 1 --monitoringFrequency 0.5
