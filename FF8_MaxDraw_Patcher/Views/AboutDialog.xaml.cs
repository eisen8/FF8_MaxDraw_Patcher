using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;

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
        LoadAboutText();
    }

    private void LoadAboutText()
    {
        // Get file version from Assembly
        var assembly = Assembly.GetExecutingAssembly();
        string fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion ?? "0.0.0.0";

        AboutRichTextBlock.Blocks.Clear();

        var paragraph = new Paragraph();

        paragraph.Inlines.Add(new Run { Text = "FF8 Max Draw Patcher" });
        paragraph.Inlines.Add(new LineBreak());
        paragraph.Inlines.Add(new Run { Text = $"Version: {fileVersion}-TEST" });
        paragraph.Inlines.Add(new LineBreak());
        paragraph.Inlines.Add(new Run { Text = "Author: Eisen" });
        paragraph.Inlines.Add(new LineBreak());
        paragraph.Inlines.Add(new Run { Text = "https://github.com/eisen8/FF8_MaxDraw_Patcher" });
      
        AboutRichTextBlock.Blocks.Add(paragraph);
    }
}
