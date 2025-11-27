using FF8_MaxDraw_Patcher.Model;
using FF8_MaxDraw_Patcher.Services;
using FF8_MaxDraw_Patcher.Utils;
using FF8_MaxDraw_Patcher.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics;
using Windows.Storage;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using AppWindow = Microsoft.UI.Windowing.AppWindow;
using Path = System.IO.Path;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FF8_MaxDraw_Patcher
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public static MainWindow? Instance { get; private set; }
        public TextBox FilePathBox => PathBox;

        private readonly AppWindow _appWindow;
        private readonly MainViewModel _viewModel;
        private readonly OverlappedPresenter _presenter;
        private readonly Logger _l;

        public MainWindow(MainViewModel viewModel, Logger logger)
        {
            _l = logger;

            InitializeComponent();
            Instance = this;
            _viewModel = viewModel;
            _appWindow = this.AppWindow;
            this.RootGrid.DataContext = _viewModel;
            _viewModel.Logs.CollectionChanged += Logs_CollectionChanged;

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


            SetTitleBar(TitleBarDraggableArea);
        }


        private void Logs_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (LogEntry entry in e.NewItems)
                {
                    AppendLogToRichEditBox(entry);
                }
            }
        }

        private void AppendLogToRichEditBox(LogEntry entry)
        {
            if (!DispatcherQueue.HasThreadAccess)
            {
                DispatcherQueue.TryEnqueue(() => AppendLogToRichEditBox(entry));
                return;
            }

            var doc = LogBox.Document;
            doc.GetText(TextGetOptions.None, out string currentText);
            int startPosition = currentText.Length;

            string newLine = $"[{DateTime.Now:HH:mm:ss}] -  {entry.Message}\n";
            var range = doc.GetRange(startPosition, startPosition);

            LogBox.IsReadOnly = false;
            range.SetText(TextSetOptions.None, newLine);

            // Apply the color formatting
            range.CharacterFormat.ForegroundColor = entry.Color;
            LogBox.IsReadOnly = true;

            // Auto-scroll
            doc = LogBox.Document;
            doc.GetText(TextGetOptions.None, out string text);
            int endPosition = text.Length;
            ITextRange range2 = doc.GetRange(endPosition, endPosition);
            doc.Selection.SetRange(endPosition, endPosition);
            doc.Selection.ScrollIntoView(PointOptions.None);
        }

        private async void AboutBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AboutDialog();
            dialog.XamlRoot = this.Content.XamlRoot;
            await dialog.ShowAsync();
        }

        private async void MinimizeBtn_Click(object sender, RoutedEventArgs e)
        {
            _presenter.Minimize();
        }

        private async void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
