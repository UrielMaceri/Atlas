using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Back.Classes;
using Back.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
namespace Front;

public class CategoryViewModel : ReactiveObject
{
    private string _name;
    private bool _isEditingName;
    private string _editingName = string.Empty;

    public string Name
    {
        get => _name;
        private set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public bool IsEditingName
    {
        get => _isEditingName;
        set => this.RaiseAndSetIfChanged(ref _isEditingName, value);
    }

    public string EditingName
    {
        get => _editingName;
        set => this.RaiseAndSetIfChanged(ref _editingName, value);
    }

    public ICommand RenameCategory { get; }
    public ICommand DeleteCategory { get; }
    public ICommand CommitRenameCategory { get; }
    public ICommand CancelRenameCategory { get; }

    public Category Category { get; }
    private readonly WorkspaceTabViewModel _parent;
    public ObservableCollection<ItemViewModel> ItemsInCategory { get; } = new();
    public Action<string>? ShowNotification { get; set; }

    public CategoryViewModel(Category category, WorkspaceTabViewModel parent, Action<string>? showNotification = null)
    {
        Category = category;
        _parent  = parent;
        _name    = category.Name;
        ShowNotification = showNotification;

        // Load this category's items from the DB
        var service = App.Services.GetRequiredService<ItemService>();
        var items   = service.GetByCategory(category.Id);

        foreach (var item in items)
        {
            var itemVm = new ItemViewModel(item, this, showNotification);
            ItemsInCategory.Add(itemVm);
        }

        RenameCategory = new RelayCommand(() =>
        {
            EditingName = Name;
            IsEditingName = true;
        });

        DeleteCategory = new RelayCommand(() =>
        {
            if (ItemsInCategory.Count == 0)
            {
                var svc = App.Services.GetRequiredService<CategoryService>();
                svc.Delete(Category.Id);
                _parent.LoadCategories(_parent.Workspace.Id);
            } else
            {
                ShowNotification?.Invoke($"Cannot delete a Category with items.");
            }
        });

        CommitRenameCategory = new RelayCommand(() =>
        {
            var trimmed = (EditingName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(trimmed)) trimmed = Name;

            var svc = App.Services.GetRequiredService<CategoryService>();
            svc.Update(Category.Id, trimmed, Category.Description ?? string.Empty);

            // Update local state and refresh parent list
            Category.Name = trimmed;
            Name = trimmed;
            IsEditingName = false;
            _parent.LoadCategories(_parent.Workspace.Id);
        });

        CancelRenameCategory = new RelayCommand(() =>
        {
            IsEditingName = false;
            EditingName = Name;
        });
    }
}
