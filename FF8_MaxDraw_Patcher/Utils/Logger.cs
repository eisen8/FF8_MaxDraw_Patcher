using Microsoft.UI;
using System;
using Windows.UI;

namespace FF8_MaxDraw_Patcher.Utils
{
    public class Logger
    {
        public void Log(string message, Color? color = null)
        {
            color ??= Colors.White;
            // todo
        }

        public void LogError(string message)
        {
            // todo
        }

        public void LogError(string message, Exception e)
        {
            // todo
        }
    }
}
