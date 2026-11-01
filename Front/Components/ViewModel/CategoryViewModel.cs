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
    private string _description;
    private bool _isEditingName;
    private string _editingName = string.Empty;
    private string _editingDescription = string.Empty;
    private bool _canMoveUp;
    private bool _canMoveDown;
    private readonly Action<string>? _showNotification;

    public string Name
    {
        get => _name;
        private set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public string Description
    {
        get => _description;
        private set => this.RaiseAndSetIfChanged(ref _description, value);
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

    public string EditingDescription
    {
        get => _editingDescription;
        set => this.RaiseAndSetIfChanged(ref _editingDescription, value);
    }

    public bool CanMoveUp
    {
        get => _canMoveUp;
        private set => this.RaiseAndSetIfChanged(ref _canMoveUp, value);
    }

    public bool CanMoveDown
    {
        get => _canMoveDown;
        private set => this.RaiseAndSetIfChanged(ref _canMoveDown, value);
    }
    
    public ICommand RenameCategory { get; }
    public ICommand DeleteCategory { get; }
    public ICommand CommitRenameCategory { get; }
    public ICommand CancelRenameCategory { get; }
    public ICommand MoveUp { get; }
    public ICommand MoveDown { get; }

    public Category Category { get; }
    public WorkspaceTabViewModel WorkspaceTab => _parent;
    private readonly WorkspaceTabViewModel _parent;
    public ObservableCollection<ItemViewModel> ItemsInCategory { get; } = new();
    public Action<string>? ShowNotification { get; set; }
    private enum Direction { Forward, Backward };

    public ObservableCollection<ItemViewModel> LoadItems(int categoryId)
    {
        var service = App.Services.GetRequiredService<ItemService>();

        var Items = service.GetByCategory(categoryId);

        ItemsInCategory.Clear();
        foreach (var ite in Items)
        {
            var itemVm = new ItemViewModel(ite, this, ShowNotification);
            ItemsInCategory.Add(itemVm);
        }
        return ItemsInCategory;
    }

    public CategoryViewModel(Category category, WorkspaceTabViewModel parent, Action<string>? showNotification = null)
    {
        Category = category;
        _showNotification = showNotification;
        _parent  = parent;
        _name    = category.Name;
        _description = category.Description;

        // Load this category's items from the DB
        var categoryService = App.Services.GetRequiredService<CategoryService>();
        var itemService = App.Services.GetRequiredService<ItemService>();
        var items   = itemService.GetByCategory(category.Id);

        var lastCategory = categoryService.GetMaxValue(category.WorkspaceId);
        CanMoveUp = Category.SortOrder > 1;
        CanMoveDown = Category.SortOrder < lastCategory;

        foreach (var item in items)
        {
            var itemVm = new ItemViewModel(item, this, showNotification);
            ItemsInCategory.Add(itemVm);
        }

        RenameCategory = new RelayCommand(() =>
        {
            EditingName = Name;
            EditingDescription = Description;
            IsEditingName = true;
        });

        DeleteCategory = new RelayCommand(() =>
        {
            if (ItemsInCategory.Count == 0)
            {
                categoryService.Delete(Category.Id);
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
            var trimmedDescription = (EditingDescription ?? string.Empty).Trim();

            categoryService.Update(Category.Id, trimmed, trimmedDescription);

            // Update local state and refresh parent list
            Category.Name = trimmed;
            Category.Description = trimmedDescription;
            Name = trimmed;
            Description = trimmedDescription;
            IsEditingName = false;
            _parent.LoadCategories(_parent.Workspace.Id);
        });

        CancelRenameCategory = new RelayCommand(() =>
        {
            IsEditingName = false;
            EditingName = Name;
            EditingDescription = Description;
        });

        MoveUp = new RelayCommand(() =>
        {
            categoryService.UpdateSorting(Category.Id, (int)Direction.Backward);
            _parent.LoadCategories(_parent.Workspace.Id);
        });
        MoveDown = new RelayCommand(() =>
        {
            categoryService.UpdateSorting(Category.Id, (int)Direction.Forward);
            _parent.LoadCategories(_parent.Workspace.Id);
        });

        
    }
}
