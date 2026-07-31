using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Back.Classes;
using Back.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
namespace Front;

public class TagViewModel : ReactiveObject
{
    public Tag Tag { get; }

    private readonly WorkspaceTabViewModel? _parent;
    private string _name;
    public string Name{
        get => _name;
        private set => this.RaiseAndSetIfChanged(ref _name, value);
    }
    
    public ObservableCollection<ItemViewModel> ItemsWithTag { get; } = new();

    public TagViewModel(Tag tag, WorkspaceTabViewModel? parent = null)
    {
        Tag = tag;
        _parent = parent;
        _name = tag.Name;
        // Load this tag's items from the DB
        var service = App.Services.GetRequiredService<ItemService>();
        var items   = service.GetByTag(tag.Id);

        foreach (var item in items)
            ItemsWithTag.Add(new ItemViewModel(item));

    }
}