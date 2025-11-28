using FF8_MaxDraw_Patcher.Model;
using FF8_MaxDraw_Patcher.Utils;
using FF8_MaxDraw_Patcher.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Runtime.InteropServices;
using Windows.Graphics;
using Windows.UI;
using WinRT.Interop;
using AppWindow = Microsoft.UI.Windowing.AppWindow;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FF8_MaxDraw_Patcher
{
    /// <summary>
    /// The Main Window of the patcher application.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public static MainWindow? Instance { get; private set; }
        public TextBox FilePathBox => PathBox;

        private readonly AppWindow _appWindow;
        private readonly MainViewModel _viewModel;
        private readonly OverlappedPresenter _presenter;
        private readonly Logger _l;
        private readonly WindowMaximizeDisabler _maximizeDisabler;


        public MainWindow(MainViewModel viewModel, Logger logger)
        {
            _l = logger;

            InitializeComponent();
            Instance = this;
            _viewModel = viewModel;
            _appWindow = this.AppWindow;
            this.RootGrid.DataContext = _viewModel;
            _l.MessageLogged += OnLogAdded; // subscribe to log events

            // Set default size and remove old title bar
            _appWindow.Resize(new SizeInt32(500, 320));
            _appWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            _appWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            ExtendsContentIntoTitleBar = true;

            // Set presenter
            _presenter = (_appWindow.Presenter as OverlappedPresenter)!;
            _presenter.PreferredMinimumWidth = 500;
            _presenter.PreferredMinimumHeight = 320;
            _presenter.IsMaximizable = false;
            _presenter.SetBorderAndTitleBar(hasBorder: true, hasTitleBar: false);
            AppWindow.SetPresenter(_presenter);

            // Makes the custom Title Bar draggable.
            SetTitleBar(TitleBarDraggableArea);

            // Disable maximize on Title Bar double-click
            _maximizeDisabler = new WindowMaximizeDisabler(this);
        }

        /// <summary>
        /// Event when a new log is added.
        /// </summary>
        private void OnLogAdded(object? sender, LogEventArgs e)
        {
            DispatcherQueue.TryEnqueue(() => AppendLogToRichEditBox(e.Message, e.Color));
        }

        /// <summary>
        /// Appends logs to the log Rich Edit Box.
        /// </summary>
        /// <param name="e">The log details</param>
        private void AppendLogToRichEditBox(string message, Color color)
        {
            try
            {
                // Make sure we are on the UI thread
                if (!DispatcherQueue.HasThreadAccess)
                {
                    DispatcherQueue.TryEnqueue(() => AppendLogToRichEditBox(message, color));
                    return;
                }

                var doc = LogBox.Document;
                doc.GetText(TextGetOptions.None, out string currentText);
                int startPosition = currentText.Length;

                string newLine = $"{DateTime.Now:HH:mm:ss} -  {message}\n";
                var range = doc.GetRange(startPosition, startPosition);

                LogBox.IsReadOnly = false;
                range.SetText(TextSetOptions.None, newLine);

                // Apply the color formatting
                range.CharacterFormat.ForegroundColor = color;
                LogBox.IsReadOnly = true;

                // Auto-scroll
                doc = LogBox.Document;
                doc.GetText(TextGetOptions.None, out string text);
                int endPosition = text.Length;
                ITextRange range2 = doc.GetRange(endPosition, endPosition);
                doc.Selection.SetRange(endPosition, endPosition);
                doc.Selection.ScrollIntoView(PointOptions.None);
            }
            catch (Exception ex)
            {
                // Log the fatal to console only to avoid potential infinite loop
                _l.LogError(ex, "Error appending log to RichEditBox.", true);
            }
        }

        /// <summary>
        /// When the About button on the titlebar is clicked.
        /// </summary>
        private async void AboutBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new AboutDialog();
                dialog.XamlRoot = this.Content.XamlRoot;
                await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                _l.LogError(ex, "Failed to open About dialog.");
            }
        }

        /// <summary>
        /// When the minimize button on the titlebar is clicked.
        /// </summary>
        private async void MinimizeBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _presenter.Minimize();
            }
            catch (Exception ex)
            {
                _l.LogError(ex, "Failed to minimize window.");
            }
        }

        /// <summary>
        /// When the close button on the titlebar is clicked.
        /// </summary>
        private async void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                _l.LogError(ex, "Failed to close window.");
            }
        }
    }
}
