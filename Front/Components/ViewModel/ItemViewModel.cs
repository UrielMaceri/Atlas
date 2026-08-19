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
using Microsoft.EntityFrameworkCore.Storage;

namespace Front;

public class ItemViewModel : ReactiveObject
{
    private string _name;
    private string _description;
    private string _newName = string.Empty;
    private string _newDescription = string.Empty;
    private int _newCategory = 0;
    private CategoryViewModel? _selectedCategory;
    private bool _isEditingName;
    private bool _isEditingCategory;

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
    public string NewName
    {
        get => _newName;
        set => this.RaiseAndSetIfChanged(ref _newName, value);
    }
    public string NewDescription
    {
        get => _newDescription;
        set => this.RaiseAndSetIfChanged(ref _newDescription, value);
    }
    public int NewCategory
    {
        get => _newCategory;
        private set => this.RaiseAndSetIfChanged(ref _newCategory, value);
    }
    public CategoryViewModel? SelectedCategory
    {
        get => _selectedCategory;
        set => this.RaiseAndSetIfChanged(ref _selectedCategory, value);
    }
    public bool IsEditingName
    {
        get => _isEditingName;
        private set => this.RaiseAndSetIfChanged(ref _isEditingName, value);
    }
    public bool IsEditingCategory
    {
        get => _isEditingCategory;
        private set => this.RaiseAndSetIfChanged(ref _isEditingCategory, value);
    }

    public ICommand OpenFileCommand { get; }
    public ICommand DeleteItemCommand { get; }
    public ICommand RenameItemCommand { get; }
    public ICommand RenameCommit { get; }
    public ICommand RenameCancel { get; }
    public ICommand ChangeCategoryCommand { get; }
    public ICommand ChangeCategoryCommit { get; }
    public ICommand ChangeCategoryCancel { get; }

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
    public ObservableCollection<CategoryViewModel> CategoriesInWorkspace =>
        _parent?.WorkspaceTab.Categories ?? new ObservableCollection<CategoryViewModel>();
    public Action<string>? ShowNotification { get; set; }
    
    public ItemViewModel(Item item, CategoryViewModel? parent = null, Action<string>? showNotification = null)
    {
        Item = item;
        _parent  = parent;
        _name    = item.Name;
        _description = item.Description;
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
        DeleteItemCommand = new RelayCommand(DeleteItem);    
        
        RenameItemCommand = new RelayCommand(() =>
        {
            NewName = Name;
            NewDescription = Description;
            IsEditingName = true;
        });
        RenameCommit = new RelayCommand(() =>
            {
                if (string.IsNullOrWhiteSpace(NewName))
                {
                    ShowNotification?.Invoke("The item name is required.");
                    return;
                }

                var service = App.Services.GetRequiredService<ItemService>();
                service.Update(item.Id, NewName, NewDescription, item.Path, item.IconPath, item.CategoryId, item.IsFavorite);
                item.Name = NewName.Trim();
                item.Description = NewDescription.Trim();
                Name = item.Name;
                Description = item.Description;
                IsEditingName = false;
            }
        );
        RenameCancel = new RelayCommand(() =>
            {
                IsEditingName = false;
            }
        );

        ChangeCategoryCommand = new RelayCommand(() =>
        {
            SelectedCategory = null;
            IsEditingCategory = true;
        });
        ChangeCategoryCommit = new RelayCommand(() =>
        {
            if (SelectedCategory is null)
                return;

            if (SelectedCategory.Category.Id == item.CategoryId)
            {
                IsEditingCategory = false;
                return;
            }

            var service = App.Services.GetRequiredService<ItemService>();
            service.Update(item.Id, item.Name, item.Description, item.Path, item.IconPath,
                SelectedCategory.Category.Id, item.IsFavorite);
            item.CategoryId = SelectedCategory.Category.Id;
            _parent?.LoadItems(_parent.Category.Id);
            SelectedCategory.LoadItems(SelectedCategory.Category.Id);
            IsEditingCategory = false;
        });
        ChangeCategoryCancel = new RelayCommand(() =>
        {
            IsEditingCategory = false;
            SelectedCategory = null;
        });
    }

    private void OpenFile()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(FilePath) || !File.Exists(FilePath))
            {
                ShowNotification?.Invoke("File not found");
                return;
            }

            Process.Start(new ProcessStartInfo(FilePath) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            ShowNotification?.Invoke($"Unable to open file: {ex.Message}");
        }
    }
    private void DeleteItem()
    {
        if (_parent is null)
        {
            ShowNotification?.Invoke("No parent category available.");
            return;
        }

        var service = App.Services.GetRequiredService<ItemService>();
        service.Delete(Item.Id);
        _parent.LoadItems(_parent.Category.Id);
    }
}