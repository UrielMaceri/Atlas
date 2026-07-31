using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Back.Classes;
using Back.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
namespace Front;

public class ItemViewModel : ReactiveObject
{
    public Item Item { get; }
    private readonly CategoryViewModel? _parent;
    private string _name;
    public string Name{
        get => _name;
        private set => this.RaiseAndSetIfChanged(ref _name, value);
    }
    
    public ObservableCollection<TagViewModel> TagsInItem { get; } = new();
    
    public ItemViewModel(Item item, CategoryViewModel? parent = null)
    {
        Item = item;
        _parent  = parent;
        _name    = item.Name;

        // Load this item's tags from the DB
        var service = App.Services.GetRequiredService<ItemService>();
        var tags   = service.GetTagsByItem(item.Id);

        foreach (var tag in tags)
            TagsInItem.Add(new TagViewModel(tag));
    }
}