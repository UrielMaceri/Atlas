using Avalonia.Controls;
using System.ComponentModel;

namespace Front;

public partial class ChangeCategoryWindow : Window
{
    public ChangeCategoryWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, System.EventArgs e)
    {
        if (DataContext is ItemViewModel item)
            item.PropertyChanged += OnItemPropertyChanged;
    }

    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ItemViewModel.IsEditingCategory) &&
            DataContext is ItemViewModel item &&
            !item.IsEditingCategory)
        {
            Close();
        }
    }
    private void OnCategorySelected(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is ItemViewModel item &&
            item.SelectedCategory is not null)
        {
            item.ChangeCategoryCommit.Execute(null);
        }
    }
}