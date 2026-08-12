using System;
using System.Diagnostics;
using Avalonia;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using Avalonia.Media.Imaging;
using Back.Classes;
using Back.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Avalonia.OpenGL.Controls;

namespace Front;

public class ItemViewModel : ReactiveObject
{
    private string _name;
    public string Name{
        get => _name;
        private set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public ICommand OpenFileCommand { get; }
    public string FilePath => Item.Path;

    public Item Item { get; }
    private readonly CategoryViewModel? _parent;
    private Bitmap? _iconBitmap;
    public Bitmap? IconBitmap
    {
        get => _iconBitmap;
        private set => this.RaiseAndSetIfChanged(ref _iconBitmap, value);
    }
    
    public ObservableCollection<TagViewModel> TagsInItem { get; } = new();
    public Action<string>? ShowNotification { get; set; }
    
    public ItemViewModel(Item item, CategoryViewModel? parent = null, Action<string>? showNotification = null)
    {
        Item = item;
        _parent  = parent;
        _name    = item.Name;
        ShowNotification = showNotification;
        // icon path is now handled via IconBitmap; raw path left on Item.IconPath

        // Load bitmap for the icon (file path saved in Item.IconPath)
        try
        {
            if (!string.IsNullOrWhiteSpace(item.IconPath) && File.Exists(item.IconPath))
            {
                using var fs = File.OpenRead(item.IconPath);
                IconBitmap = new Bitmap(fs);
            }
            else
            {
                // Fallback to a placeholder file located in the app output Assets folder
                try
                {
                    var exeAsset = Path.Combine(AppContext.BaseDirectory ?? string.Empty, "Assets", "placeholder.png");
                    if (File.Exists(exeAsset))
                    {
                        using var fs2 = File.OpenRead(exeAsset);
                        IconBitmap = new Bitmap(fs2);
                    }
                    else
                    {
                        IconBitmap = null;
                    }
                }
                catch
                {
                    IconBitmap = null;
                }
            }
        }
        catch
        {
            IconBitmap = null;
        }

        // Load this item's tags from the DB
        var service = App.Services.GetRequiredService<ItemService>();
        var tags   = service.GetTagsByItem(item.Id);

        foreach (var tag in tags)
            TagsInItem.Add(new TagViewModel(tag));


        OpenFileCommand = new RelayCommand(OpenFile);
    }

    private void OpenFile()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(FilePath) || !File.Exists(FilePath))
                ShowNotification?.Invoke($"File not found");

            Process.Start(new ProcessStartInfo(FilePath) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            ShowNotification?.Invoke($"Unable to open file: {ex.Message}");
        }
    }
}