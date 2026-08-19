using Avalonia.Controls;
using System.ComponentModel;

namespace Front;

public partial class EditItemWindow : Window
{
    public EditItemWindow()
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
        if (e.PropertyName == nameof(ItemViewModel.IsEditingName) &&
            DataContext is ItemViewModel item &&
            !item.IsEditingName)
        {
            Close();
        }
    }
}