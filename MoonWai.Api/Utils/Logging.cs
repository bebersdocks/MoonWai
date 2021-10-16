using System;

using Serilog;
using Serilog.Core;
using Serilog.Events;

public static class Logging 
{
    private static string defaultTemplate = "[{Timestamp:dd-MM-yyyy HH:mm:ss.fff}] [{Level:u3}] {ThreadId} {Message}{NewLine}{Exception}";

    public static Logger CreateLogger(
        string logPath = "../_logs/",
        LogEventLevel level = LogEventLevel.Debug,
        LogEventLevel overrideLevel = LogEventLevel.Warning,
        bool useConsole = true)
    {
        return new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .MinimumLevel.Is(level)
            .MinimumLevel.Override("System", overrideLevel)
            .MinimumLevel.Override("Microsoft", overrideLevel)
            .WriteTo.Async(i =>
            {
                if (useConsole)
                    i.Console(level, outputTemplate: defaultTemplate);

                i.File(logPath + DateTime.Now.ToString("dd_MM_yyyy") + ".log", outputTemplate: defaultTemplate, shared: true);
            })
            .CreateLogger();
    }
}
