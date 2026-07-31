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
    public Category Category { get; }
    private readonly WorkspaceTabViewModel _parent;
    private string _name;
    public string Name{
        get => _name;
        private set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public ObservableCollection<ItemViewModel> ItemsInCategory { get; } = new();

    public CategoryViewModel(Category category, WorkspaceTabViewModel parent)
    {
        Category = category;
        _parent  = parent;
        _name    = category.Name;

        // Load this category's items from the DB
        var service = App.Services.GetRequiredService<ItemService>();
        var items   = service.GetByCategory(category.Id);

        foreach (var item in items)
            ItemsInCategory.Add(new ItemViewModel(item));
    }
}