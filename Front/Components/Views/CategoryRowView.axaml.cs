using Avalonia.Controls;
using Avalonia.Input;
using Back.Services;
using Back.Classes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;

namespace Front;

public partial class CategoryRowView : UserControl
{
    public CategoryRowView()
    {
        InitializeComponent();
        AddHandler(DragDrop.DropEvent, OnItemDropped, handledEventsToo: true);
        AddHandler(DragDrop.DragEnterEvent, OnItemEntered, handledEventsToo: true);
        AddHandler(DragDrop.DragOverEvent, OnItemOver, handledEventsToo: true);
        AddHandler(DragDrop.DragLeaveEvent, OnItemLeft, handledEventsToo: true);
    }

    public void SetDropOutlineVisible(bool isVisible)
    {
        DropOutlineIndicator.IsVisible = isVisible;
    }

    private void OnItemDropped(object? sender, DragEventArgs e)
    {
        DropOutlineIndicator.IsVisible = false;
        DropHereIndicator.IsVisible = false;

        if (DataContext is not CategoryViewModel categoryVm)
            return;

        if (!e.DataTransfer.Contains(DataFormat.File))
            return;

        var storageFiles = e.DataTransfer.TryGetFiles()?.ToList();
        if (storageFiles == null || storageFiles.Count == 0)
            return;

        var service = App.Services.GetRequiredService<ItemService>();

        foreach (var storageFile in storageFiles)
        {
            var filePath = storageFile.Path?.LocalPath;
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                continue;

            var itemName = Path.GetFileNameWithoutExtension(filePath);
            var iconPath = IconHelper.ExtractAndSave(filePath);
            var description = filePath; //By default, saving Path of the item in description, user editable 

            var item = service.Create(itemName, description, filePath, iconPath, categoryVm.Category.Id, false);
            categoryVm.ItemsInCategory.Add(new ItemViewModel(item, categoryVm, categoryVm.ShowNotification));
        }

        e.Handled = true;
    }

    private void OnItemEntered(object? sender, DragEventArgs e)
    {
        DropHereIndicator.IsVisible = e.DataTransfer.Contains(DataFormat.File);
    }

    private void OnItemOver(object? sender, DragEventArgs e)
    {
        DropHereIndicator.IsVisible = e.DataTransfer.Contains(DataFormat.File);
    }

    private void OnItemLeft(object? sender, DragEventArgs e)
    {
        DropHereIndicator.IsVisible = false;
    }
}
