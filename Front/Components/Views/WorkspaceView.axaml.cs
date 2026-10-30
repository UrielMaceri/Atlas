using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using System.Linq;

namespace Front;

public partial class WorkspaceView : UserControl
{
    public WorkspaceView()
    {
        InitializeComponent();
        CategoryDropSurface.AddHandler(DragDrop.DragEnterEvent, OnWorkspaceDragOver, handledEventsToo: true);
        CategoryDropSurface.AddHandler(DragDrop.DragOverEvent, OnWorkspaceDragOver, handledEventsToo: true);
        CategoryDropSurface.AddHandler(DragDrop.DragLeaveEvent, OnWorkspaceDragLeave, handledEventsToo: true);
        CategoryDropSurface.AddHandler(DragDrop.DropEvent, OnWorkspaceDrop, handledEventsToo: true);

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

    private void OnWorkspaceDragOver(object? sender, DragEventArgs e)
    {
        SetCategoryOutlinesVisible(e.DataTransfer.Contains(DataFormat.File));
    }

    private void OnWorkspaceDragLeave(object? sender, DragEventArgs e)
    {
        SetCategoryOutlinesVisible(false);
    }

    private void OnWorkspaceDrop(object? sender, DragEventArgs e)
    {
        SetCategoryOutlinesVisible(false);
    }

    private void SetCategoryOutlinesVisible(bool isVisible)
    {
        foreach (var categoryRow in CategoryDropSurface.GetVisualDescendants().OfType<CategoryRowView>())
        {
            categoryRow.SetDropOutlineVisible(isVisible);
        }
    }
}
