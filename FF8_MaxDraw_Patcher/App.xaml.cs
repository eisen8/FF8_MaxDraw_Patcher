using FF8_MaxDraw_Patcher.Patch;
using FF8_MaxDraw_Patcher.Services;
using FF8_MaxDraw_Patcher.Services.Interfaces;
using FF8_MaxDraw_Patcher.Utils;
using FF8_MaxDraw_Patcher.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FF8_MaxDraw_Patcher
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window? _window;
        private IServiceProvider? _services { get; set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Logger l = new Logger();
            ConfigureServices(l);
            InitializeComponent();
        }

        private void ConfigureServices(Logger logger)
        {
            var services = new ServiceCollection();

            // Register Views
            services.AddSingleton<MainWindow>();
            services.AddSingleton<AboutDialog>();
            services.AddSingleton<IUIService, UIService>();

            // Register ViewModels
            services.AddSingleton<MainViewModel>();

            // Register FileSystem
            services.AddSingleton<IFileSystem, FileSystem>();

            // Register Services
            services.AddSingleton<IFileValidator, FileValidator>();
            services.AddSingleton<IPatcher, Patcher>();

            // Register Utils
            services.AddSingleton<Logger>(logger);

            // Register Patch
            services.AddSingleton<IPatch>(new MaxDrawPatchDetails());

            // Build
            _services = services.BuildServiceProvider();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _window = _services!.GetRequiredService<MainWindow>();
            _window.Title = "FF8 MaxDraw Patcher";
            _window.Activate();
        }
    }
}
