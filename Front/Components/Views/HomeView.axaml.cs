using Avalonia.Controls;

namespace Front;

public partial class HomeView : UserControl
{
    public HomeView()
    {
        InitializeComponent();
        // DataContext is set externally by MainWindowViewModel via HomeTabSentinel
    }
}
