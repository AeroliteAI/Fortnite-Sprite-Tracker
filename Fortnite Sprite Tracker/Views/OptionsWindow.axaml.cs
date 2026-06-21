using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using FortniteSpriteTracker.Models;
using FortniteSpriteTracker.ViewModels;

namespace FortniteSpriteTracker.Views;

public partial class OptionsWindow : Window
{
    public OptionsWindow()
    {
        InitializeComponent();

        SpritesFolderPath.Text  = AppPaths.Sprites;
        DataFolderPath.Text     = AppPaths.Data;
        CurrentVersionText.Text = $"v{PatchNotes.CurrentVersion}";
        PatchNotesItemsControl.ItemsSource = PatchNotes.History;

        MasteredIcon.Source = Services.ImageCacheService.LoadAssetImage("Mastered.png");

        DataContextChanged += (_, _) =>
        {
            if (DataContext is MainViewModel vm)
                OverlayOutputPathText.Text = string.IsNullOrWhiteSpace(vm.OverlayOutputPath)
                    ? AppPaths.Base : vm.OverlayOutputPath;
        };
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        if (TryGetPlatformHandle() is { } h)
            Helpers.NativeMethods.EnableRoundedCorners(h.Handle);
    }

    // ── Tab switching ────────────────────────────────────────────────────────

    private void FilesTab_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        FilesPanel.IsVisible        = true;
        InstructionsPanel.IsVisible = false;
        PatchNotesPanel.IsVisible   = false;
        FilesTabBtn.Classes.Set("active", true);
        InstructionsTabBtn.Classes.Set("active", false);
        PatchNotesTabBtn.Classes.Set("active", false);
    }

    private void InstructionsTab_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        FilesPanel.IsVisible        = false;
        InstructionsPanel.IsVisible = true;
        PatchNotesPanel.IsVisible   = false;
        FilesTabBtn.Classes.Set("active", false);
        InstructionsTabBtn.Classes.Set("active", true);
        PatchNotesTabBtn.Classes.Set("active", false);
    }

    private void PatchNotesTab_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        FilesPanel.IsVisible        = false;
        InstructionsPanel.IsVisible = false;
        PatchNotesPanel.IsVisible   = true;
        FilesTabBtn.Classes.Set("active", false);
        InstructionsTabBtn.Classes.Set("active", false);
        PatchNotesTabBtn.Classes.Set("active", true);
    }

    // ── Folder open buttons ──────────────────────────────────────────────────

    private void OpenSprites_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Directory.CreateDirectory(AppPaths.Sprites);
        Process.Start(new ProcessStartInfo("explorer.exe", AppPaths.Sprites) { UseShellExecute = true });
    }

    private void OpenData_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Directory.CreateDirectory(AppPaths.Data);
        Process.Start(new ProcessStartInfo("explorer.exe", AppPaths.Data) { UseShellExecute = true });
    }

    // ── Overlay output folder ────────────────────────────────────────────────

    private async void BrowseOverlayOutput_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var result = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Choose overlay output folder",
            AllowMultiple = false,
        });
        if (result.Count == 0 || DataContext is not MainViewModel vm) return;
        var path = result[0].Path.LocalPath;
        vm.OverlayOutputPath       = path;
        OverlayOutputPathText.Text = path;
    }

    private void ResetOverlayOutput_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel vm) return;
        vm.OverlayOutputPath       = string.Empty;
        OverlayOutputPathText.Text = AppPaths.Base;
    }

    // ── Window chrome ────────────────────────────────────────────────────────

    private void Header_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            BeginMoveDrag(e);
    }

    private void Close_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        => Close();
}
