using FF8_MaxDraw_Patcher.Services.Interfaces;
using FF8_MaxDraw_Patcher.Utils;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace FF8_MaxDraw_Patcher.Services
{
    /// <summary>
    /// Provides UI services (such as creating dialogs) for the view models.
    /// </summary>
    public class UIService : IUIService
    {
        private readonly Logger _l;

        public UIService(Logger logger)
        {
            _l = logger;
        }

        /// <summary>
        /// Creates the UI file picker
        /// </summary>
        /// <returns>The selected file</returns>
        public async Task<StorageFile> FilePicker()
        {
            var picker = new FileOpenPicker();

            // Required in WinUI 3 Desktop apps to initialize with window handle
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(MainWindow.Instance);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            // Filter for exectable files
            picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            picker.FileTypeFilter.Add(".exe");

            StorageFile file = await picker.PickSingleFileAsync();

            return file;
        }

        /// <summary>
        /// Sets the focus of the PathBox to the end of the filename rather than the start so that user can see file name. 
        /// </summary>
        public void RequestPathBoxFocus()
        {
            TextBox? pathBox = MainWindow.Instance?.FilePathBox;

            if (pathBox != null)
            {
                pathBox.SelectionStart = pathBox.Text.Length;
                pathBox.SelectionLength = 0;
                pathBox.Focus(FocusState.Programmatic);
            }
        }
    }
}
