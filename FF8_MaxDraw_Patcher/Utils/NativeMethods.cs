using System;
using System.Runtime.InteropServices;
using Windows.System;


namespace FF8_MaxDraw_Patcher.Utils
{
    internal static partial class NativeMethods
    {
        private const string USER32 = "user32.dll";


        [LibraryImport(USER32, EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
        internal static partial IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr newProc);

        [LibraryImport(USER32, EntryPoint = "CallWindowProcW")]
        internal static partial IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    }
}
