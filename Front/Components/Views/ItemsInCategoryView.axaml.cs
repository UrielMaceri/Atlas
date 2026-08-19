using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.ComponentModel;

namespace Front;

public partial class ItemsInCategoryView : UserControl
{
    private ItemViewModel? _itemViewModel;
    private bool _dialogOpen;
    private bool _categoryDialogOpen;

    public ItemsInCategoryView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (_itemViewModel is not null)
        {
            _itemViewModel.PropertyChanged -= OnItemPropertyChanged;
            _itemViewModel.PropertyChanged -= OnCategoryPropertyChanged;
        }

        _itemViewModel = DataContext as ItemViewModel;
        if (_itemViewModel is not null)
        {
            _itemViewModel.PropertyChanged += OnItemPropertyChanged;
            _itemViewModel.PropertyChanged += OnCategoryPropertyChanged;
        }
    }

    private async void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(ItemViewModel.IsEditingName) ||
            _itemViewModel is null ||
            !_itemViewModel.IsEditingName ||
            _dialogOpen)
        {
            return;
        }

        var owner = TopLevel.GetTopLevel(this) as Window;
        if (owner is null)
            return;

        _dialogOpen = true;
        var dialog = new EditItemWindow
        {
            DataContext = _itemViewModel
        };

        await dialog.ShowDialog(owner);
        _dialogOpen = false;
    }

    private async void OnCategoryPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(ItemViewModel.IsEditingCategory) ||
            _itemViewModel is null ||
            !_itemViewModel.IsEditingCategory ||
            _categoryDialogOpen)
        {
            return;
        }

        var owner = TopLevel.GetTopLevel(this) as Window;
        if (owner is null)
            return;

        _categoryDialogOpen = true;
        var dialog = new ChangeCategoryWindow
        {
            DataContext = _itemViewModel
        };

        await dialog.ShowDialog(owner);
        _categoryDialogOpen = false;
    }
}
