using System.Diagnostics;
using System.Windows.Interop;
using Microsoft.Win32;
using FortniteSpriteTracker.Helpers;
using FortniteSpriteTracker.Models;
using FortniteSpriteTracker.ViewModels;

namespace FortniteSpriteTracker.Views;

public partial class OptionsWindow : Window
{
    public OptionsWindow()
    {
        InitializeComponent();
        SpritesFolderPath.Text   = AppPaths.Sprites;
        DataFolderPath.Text      = AppPaths.Data;
        CurrentVersionText.Text  = $"v{PatchNotes.CurrentVersion}";
        PatchNotesItemsControl.ItemsSource = PatchNotes.History;

        if (File.Exists(AppPaths.Mastered))
            MasteredIcon.Source = new System.Windows.Media.Imaging.BitmapImage(
                new Uri(AppPaths.Mastered, UriKind.Absolute));

        if (DataContext is MainViewModel vm)
            OverlayOutputPathText.Text = string.IsNullOrWhiteSpace(vm.OverlayOutputPath)
                ? AppPaths.Base : vm.OverlayOutputPath;
    }

    private void BrowseOverlayOutput_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFolderDialog { Title = "Choose overlay output folder" };
        if (dlg.ShowDialog() != true) return;
        if (DataContext is not MainViewModel vm) return;
        vm.OverlayOutputPath         = dlg.FolderName;
        OverlayOutputPathText.Text   = dlg.FolderName;
    }

    private void ResetOverlayOutput_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel vm) return;
        vm.OverlayOutputPath       = string.Empty;
        OverlayOutputPathText.Text = AppPaths.Base;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        var hwnd = new WindowInteropHelper(this).Handle;
        NativeMethods.EnableRoundedCorners(hwnd);
        NativeMethods.EnableAcrylic(hwnd);
    }

    private void Header_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        => DragMove();

    private void CloseOptions_Click(object sender, RoutedEventArgs e) => Close();

    private void FilesTab_Click(object sender, RoutedEventArgs e)
    {
        FilesPanel.Visibility        = Visibility.Visible;
        InstructionsPanel.Visibility = Visibility.Collapsed;
        PatchNotesPanel.Visibility   = Visibility.Collapsed;
        FilesTabBtn.Style        = (Style)Resources["TabBtnActiveStyle"];
        InstructionsTabBtn.Style = (Style)Resources["TabBtnStyle"];
        PatchNotesTabBtn.Style   = (Style)Resources["TabBtnStyle"];
    }

    private void InstructionsTab_Click(object sender, RoutedEventArgs e)
    {
        FilesPanel.Visibility        = Visibility.Collapsed;
        InstructionsPanel.Visibility = Visibility.Visible;
        PatchNotesPanel.Visibility   = Visibility.Collapsed;
        FilesTabBtn.Style        = (Style)Resources["TabBtnStyle"];
        InstructionsTabBtn.Style = (Style)Resources["TabBtnActiveStyle"];
        PatchNotesTabBtn.Style   = (Style)Resources["TabBtnStyle"];
    }

    private void PatchNotesTab_Click(object sender, RoutedEventArgs e)
    {
        FilesPanel.Visibility        = Visibility.Collapsed;
        InstructionsPanel.Visibility = Visibility.Collapsed;
        PatchNotesPanel.Visibility   = Visibility.Visible;
        FilesTabBtn.Style        = (Style)Resources["TabBtnStyle"];
        InstructionsTabBtn.Style = (Style)Resources["TabBtnStyle"];
        PatchNotesTabBtn.Style   = (Style)Resources["TabBtnActiveStyle"];
    }

    private void OpenSprites_Click(object sender, RoutedEventArgs e)
    {
        Directory.CreateDirectory(AppPaths.Sprites);
        Process.Start("explorer.exe", AppPaths.Sprites);
    }

    private void OpenData_Click(object sender, RoutedEventArgs e)
    {
        Directory.CreateDirectory(AppPaths.Data);
        Process.Start("explorer.exe", AppPaths.Data);
    }
}
