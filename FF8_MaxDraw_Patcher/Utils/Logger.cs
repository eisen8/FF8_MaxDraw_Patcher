using FF8_MaxDraw_Patcher.Model;
using Microsoft.UI;
using Serilog;
using Serilog.Events;
using System;
using Windows.UI;
using static FF8_MaxDraw_Patcher.Utils.Logger;

namespace FF8_MaxDraw_Patcher.Utils
{
    /// <summary>
    /// Our main logging class.
    /// </summary>
    public class Logger
    {
        public event EventHandler<LogEventArgs>? MessageLogged;

        private readonly Serilog.ILogger sLogger;

        public Logger()
        {
            // configure serilog
            var config = new LoggerConfiguration()
                .WriteTo.File("logs/app-.log",
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true,
                    retainedFileCountLimit: 6);

#if DEBUG
            config = config.WriteTo.Console();
#endif

            Serilog.Log.Logger = config.CreateLogger();
            sLogger = Serilog.Log.Logger;
        }

        /// <summary>
        /// Logs a message with Info level.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Log(string message)
        {
            Log(message, LogLevel.Info);
        }

        /// <summary>
        /// Logs a message with Info level and an explicit color.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="color">The color to use.</param>
        public void Log(string message, Color color)
        {
            if (color == Colors.Green)
            {
                color = ColorHelper.FromArgb(255, 0, 204, 102); // Better geen for dark background
            }

            log(message, LogLevel.Info, color);
        }

        /// <summary>
        /// Logs a message with the specified log level.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="level">The log level to use.</param>
        /// <param name="consoleOnly">If true doesn't log to the UI</param>
        public void Log(string message, LogLevel level, bool consoleOnly = false)
        {
            Color color = toLogColor(level);
            log(message, level, color, false);
        }

        /// <summary>
        /// Logs an error message with exception details.
        /// </summary>
        /// <param name="e">The exception.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="consoleOnly">If true doesn't log to the UI</param>
        public void LogError(Exception e, string message, bool consoleOnly = false)
        {
            string logMessage = $"{message}\n{e}";
            Log(logMessage, LogLevel.Error, consoleOnly);
        }

        /// <summary>
        /// Logs a fatal message with exception details.
        /// </summary>
        /// <param name="e">The exception.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="consoleOnly">If true doesn't log to the UI</param>
        public void LogFatal(Exception e, string message, bool consoleOnly = false)
        {
            string logMessage = $"{message}\n{e}";
            Log(logMessage, LogLevel.Fatal, consoleOnly);
        }

        /// <summary>
        /// Logs the message, updating subscribers and writing to Serilog.
        /// </summary>
        /// <param name="e">The exception.</param>
        /// <param name="level">The log level to use.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="consoleOnly">If true doesn't log to the UI</param>
        private void log(string message, LogLevel level, Color color, bool consoleOnly = false)
        {
            // Update any subscribers
            if (!consoleOnly)
            {
                MessageLogged?.Invoke(this, new LogEventArgs(message, level, color));
            }

            // write to serilog
            sLogger.Write(toSerilogLevel(level), message);
        }

        /// <summary>
        /// Mapping of our log levels to Serilog log levels.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <returns>The corresponding Serilog level.</returns>
        private static LogEventLevel toSerilogLevel(LogLevel level)
        {
            return level switch
            {
                LogLevel.Debug => LogEventLevel.Debug,
                LogLevel.Info => LogEventLevel.Information,
                LogLevel.Warning => LogEventLevel.Warning,
                LogLevel.Error => LogEventLevel.Error,
                LogLevel.Fatal => LogEventLevel.Fatal,
                _ => LogEventLevel.Information
            };
        }

        /// <summary>
        /// Mapping of log levels to log colors for the UI log. Serilog has its own color scheme but this matches it.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <returns>The color to use.</returns>
        private static Color toLogColor(LogLevel level)
        {
            return level switch
            {
                LogLevel.Debug => Colors.Gray,
                LogLevel.Info => Colors.White,
                LogLevel.Warning => Colors.Yellow,
                LogLevel.Error => Colors.OrangeRed,
                LogLevel.Fatal => Colors.Red,
                _ => Colors.White
            };
        }

        public enum LogLevel
        {
            Debug = 0,
            Info = 1,
            Warning = 2,
            Error = 3,
            Fatal = 4
        }
    }
}
