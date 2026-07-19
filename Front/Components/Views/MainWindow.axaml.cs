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
}
