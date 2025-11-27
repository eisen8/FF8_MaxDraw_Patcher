using Microsoft.UI;
using Color = Windows.UI.Color;

namespace FF8_MaxDraw_Patcher.Model
{
    public class LogEntry
    {
        public string Message { get; set; } = "";
        public Color Color { get; set; } = Colors.White;
    }
}
