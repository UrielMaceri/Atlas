using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using System;
using System.Threading.Tasks;

namespace Front;

public partial class HomeView : UserControl
{
    public HomeView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is HomeViewModel homeViewModel)
            homeViewModel.DeleteConfirmationAction = ConfirmDeleteAsync;
    }

    private async Task ConfirmDeleteAsync(WorkspaceCardViewModel card)
    {
        var owner = TopLevel.GetTopLevel(this) as Window;
        if (owner is null)
            return;

        var dialog = new Window
        {
            Title = "Delete workspace",
            Width = 390,
            Height = 230,
            CanResize = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new Border
            {
                Padding = new Thickness(24),
                Child = new StackPanel
                {
                    Spacing = 16,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = $"{card.Name}",
                            FontSize = 18,
                            FontWeight = Avalonia.Media.FontWeight.SemiBold
                        },
                        new TextBlock
                        {
                            Text = $"{card.Description}",
                            FontSize = 18,
                            FontWeight = Avalonia.Media.FontWeight.SemiBold
                        },
                        new TextBlock
                        {
                            Text = "Are you sure?",
                            TextWrapping = Avalonia.Media.TextWrapping.Wrap
                        },
                        new TextBlock
                        {
                            Text = "This action cannot be undone.",
                            TextWrapping = Avalonia.Media.TextWrapping.Wrap
                        },
                        new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            Spacing = 8,
                            Children =
                            {
                                new Button
                                {
                                    Content = "Cancel",
                                    Classes = { "action-button" }
                                },
                                new Button
                                {
                                    Content = "Delete",
                                    Classes = { "action-button", "danger-btn" }
                                }
                            }
                        }
                    }
                }
            }
        };

        var buttons = ((StackPanel)((StackPanel)((Border)dialog.Content!).Child!).Children[4]).Children;
        ((Button)buttons[0]).Click += (_, _) => dialog.Close(false);
        ((Button)buttons[1]).Click += (_, _) => dialog.Close(true);

        if (await dialog.ShowDialog<bool?>(owner) == true)
            card.DeleteConfirmed();
    }
}
