using Microsoft.ML.OnnxRuntime;
using Microsoft.UI;
using static FF8_MaxDraw_Patcher.Utils.Logger;
using Color = Windows.UI.Color;

namespace FF8_MaxDraw_Patcher.Model
{
    /// <summary>
    /// Represents an argument for a log event.
    /// </summary>
    public class LogEventArgs
    {
        /// <summary>
        /// The log message.
        /// </summary>
        public string Message;

        /// <summary>
        /// An option color for overriding the default color of the log level.
        /// </summary>
        public Color Color;

        /// <summary>
        /// The log level.
        /// </summary>
        public LogLevel LogLevel;

        public LogEventArgs(string message, LogLevel level, Color? color = null)
        {
            Message = message;
            LogLevel = level;
            Color = color ?? Colors.White;
        }
    }
}
