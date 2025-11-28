using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FF8_MaxDraw_Patcher.Utils
{
    /// <summary>
    /// Helper class to disable window maximize logic (i.e. double click on titlebar).
    /// </summary>
    public class WindowMaximizeDisabler
    {
        private readonly IntPtr _hwnd;
        private readonly IntPtr _prevWndProc;
        private readonly WndProcDelegate _newWndProc;

        private delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        private const int GWL_WNDPROC = -4;
        private const uint WM_NCLBUTTONDBLCLK = 0x00A3;

        public WindowMaximizeDisabler(Window window)
        {
            _hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            _newWndProc = new WndProcDelegate(CustomWndProc);
            _prevWndProc = NativeMethods.SetWindowLongPtr(_hwnd, GWL_WNDPROC, Marshal.GetFunctionPointerForDelegate(_newWndProc));
        }

        private IntPtr CustomWndProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == WM_NCLBUTTONDBLCLK)
                return IntPtr.Zero;

            return NativeMethods.CallWindowProc(_prevWndProc, hwnd, msg, wParam, lParam);
        }
    }
}
