using Avalonia.Controls;
using Avalonia.Input;

namespace Front;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var vm = new MainWindowViewModel();
        DataContext = vm;

        // Setup title bar drag and window buttons
        SetupTitleBar();

        // Handle Enter/Escape for tab rename TextBoxes
        AddHandler(KeyDownEvent, (_, e) =>
        {
            if (DataContext is not MainWindowViewModel mvm) return;
            if (mvm.SelectedTab is not WorkspaceTabViewModel tab) return;
            if (!tab.IsEditingName) return;

            if (e.Key == Key.Enter)
            {
                tab.CommitRenameCommand.Execute(null);
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                tab.CancelRenameCommand.Execute(null);
                e.Handled = true;
            }
        }, handledEventsToo: false);
    }

    private void SetupTitleBar()
    {
        // Get references to title bar elements
        var titleBar = this.FindControl<Border>("TitleBar");
        var minimizeBtn = this.FindControl<Button>("MinimizeButton");
        var maximizeBtn = this.FindControl<Button>("MaximizeButton");
        var closeBtn = this.FindControl<Button>("CloseButton");

        if (titleBar == null) return;

        // Make title bar draggable
        titleBar.PointerPressed += (sender, e) =>
        {
            // Only drag if not clicking a button
            if (e.Source is Button) return;

            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                BeginMoveDrag(e);
            }
        };

        // Double-click to maximize/restore
        titleBar.DoubleTapped += (_, e) =>
        {
            if (e.Source is Button) return;
            ToggleMaximize();
        };

        // Window control buttons
        if (minimizeBtn != null)
        {
            minimizeBtn.Click += (_, _) => WindowState = WindowState.Minimized;
        }

        if (maximizeBtn != null)
        {
            maximizeBtn.Click += (_, _) => ToggleMaximize();
        }

        if (closeBtn != null)
        {
            closeBtn.Click += (_, _) => Close();
        }
    }

    private void ToggleMaximize()
    {
        WindowState = WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }
}
