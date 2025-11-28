using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FF8_MaxDraw_Patcher;

/// <summary>
/// The About Dialog of the patcher application.
/// </summary>
public sealed partial class AboutDialog : ContentDialog
{
    public AboutDialog()
    {
        InitializeComponent();
        this.RequestedTheme = ElementTheme.Dark;
    }
}
