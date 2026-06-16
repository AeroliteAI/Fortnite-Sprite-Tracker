using System.Diagnostics;
using FortniteSpriteTracker.Models;

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
    }

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
