using System;

using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace MoonWai.Api.Utils
{
    public class LoggingSettings
    {
        public string        Path          { get; set; }
        public LogEventLevel Level         { get; set; }
        public LogEventLevel OverrideLevel { get; set; }
        public bool          UseConsole    { get; set; }
    }

    public static class Logging 
    {
        private static string defaultTemplate = "[{Timestamp:dd-MM-yyyy HH:mm:ss.fff}] [{Level:u3}] {ThreadId} {Message}{NewLine}{Exception}";

        public static Logger CreateLogger(LoggingSettings loggingSettings)
        {
            return new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
                .MinimumLevel.Is(loggingSettings.Level)
                .MinimumLevel.Override("System", loggingSettings.OverrideLevel)
                .MinimumLevel.Override("Microsoft", loggingSettings.OverrideLevel)
                .WriteTo.Async(i =>
                {
                    if (loggingSettings.UseConsole)
                        i.Console(loggingSettings.Level, outputTemplate: defaultTemplate);

                    var logPath = loggingSettings.Path + "/" + DateTime.Now.ToString("dd_MM_yyyy") + ".log";

                    i.File(logPath, outputTemplate: defaultTemplate, shared: true);
                })
                .CreateLogger();
        }
    }
}
