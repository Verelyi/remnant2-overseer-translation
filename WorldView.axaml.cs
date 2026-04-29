using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using RemnantOverseer.ViewModels;
using System.Diagnostics;
using RemnantOverseer.Models;
using System;

namespace RemnantOverseer.Views;

public partial class WorldView : UserControl
{
    public WorldView()
    {
        if (Design.IsDesignMode)
        {
            // This can be before or after InitializeComponent.
            var settingsService = new Services.SettingsService();
            Design.SetDataContext(this, new WorldViewModel(settingsService, new Services.SaveDataService(settingsService)));
        }
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext as WorldViewModel is null) throw new Exception("DataContext is still empty");
        ((WorldViewModel)DataContext).OnViewLoaded();
    }

    // Flyout can only be shown by explicitly calling it
    private void FiltersButton_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        if (sender is Control control)
        {
            FlyoutBase.ShowAttachedFlyout(control);
        }
    }

    private void GenesisHintButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var flyout = new Flyout
        {
            Content = new GenesisTipViewModel(),
            Placement = PlacementMode.Center
        };
        flyout.ShowAt(ContentGrid);
    }

    private void ThaenHintButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var flyout = new Flyout
        {
            Content = new ThaenProgressViewModel(((WorldViewModel)DataContext!).ThaenTree),
            Placement = PlacementMode.Center
        };
        flyout.ShowAt(ContentGrid);
    }

    private void QuiltHintButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var flyout = new Flyout
        {
            Content = new PatchworkQuiltViewModel(((WorldViewModel)DataContext!).CompletedQuests),
            Placement = PlacementMode.Center
        };
        flyout.ShowAt(ContentGrid);
    }
	private void NoteTranslateButton_Click(object? sender, RoutedEventArgs e)
	{
		if (sender is Button btn && btn.DataContext is Item item && !string.IsNullOrEmpty(item.Description))
		{
			var text = Uri.EscapeDataString(item.Description);
			var url = $"https://translate.google.com/?sl=en&tl=ru&text={text}&op=translate";
			Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
		}
	}
}
