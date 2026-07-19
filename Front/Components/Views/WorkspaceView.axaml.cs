using Avalonia.Controls;
using Avalonia.Input;

namespace Front;

public partial class WorkspaceView : UserControl
{
    public WorkspaceView()
    {
        InitializeComponent();

        AddHandler(KeyDownEvent, (_, e) =>
        {
            if (DataContext is not WorkspaceTabViewModel vm) return;
            if (!vm.IsEditingName) return;

            if (e.Key == Key.Enter)
            {
                vm.CommitRenameCommand.Execute(null);
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                vm.CancelRenameCommand.Execute(null);
                e.Handled = true;
            }
        }, handledEventsToo: false);
    }
}
