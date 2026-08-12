using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Back.Classes;
using Back.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Avalonia.Threading;
using System;

namespace Front;

public class MainWindowViewModel : ReactiveObject
{
    private bool _homeIsSelected = true;
    private object? _selectedTab;

    private string _notificationMessage = string.Empty;
    private bool _isNotificationVisible;

    public string NotificationMessage
    {
        get => _notificationMessage;
        private set => this.RaiseAndSetIfChanged(ref _notificationMessage, value);
    }

    public bool IsNotificationVisible
    {
        get => _isNotificationVisible;
        private set => this.RaiseAndSetIfChanged(ref _isNotificationVisible, value);
    }


    public HomeTabSentinel HomeTab { get; }

    /// <summary>Open workspace tabs (does not include Home).</summary>
    public ObservableCollection<WorkspaceTabViewModel> WorkspaceTabs { get; } = new();

    public bool HomeIsSelected
    {
        get => _homeIsSelected;
        private set => this.RaiseAndSetIfChanged(ref _homeIsSelected, value);
    }

    /// <summary>Either HomeTabSentinel or a WorkspaceTabViewModel — used by IsTypeConverter.</summary>
    public object? SelectedTab
    {
        get => _selectedTab;
        private set => this.RaiseAndSetIfChanged(ref _selectedTab, value);
    }

    public ICommand SelectHomeCommand { get; }
    public ICommand AddNewWorkspaceCommand { get; }

    public MainWindowViewModel()
    {
        var service = App.Services.GetRequiredService<WorkspaceService>();
        service.WorkspaceDeleted += OnWorkspaceDeleted;
        HomeTab = new HomeTabSentinel();
        HomeTab.ViewModel.OpenWorkspaceAction = OpenWorkspace;

        SelectHomeCommand    = new RelayCommand(SelectHome);
        AddNewWorkspaceCommand = new RelayCommand(AddNewWorkspace);

        SelectHome();
    }

    // ── Selection ────────────────────────────────────────────────

    private void SelectHome()
    {
        SelectedTab = HomeTab;
        HomeIsSelected = true;
        HomeTab.ViewModel.Refresh();
        foreach (var t in WorkspaceTabs) t.IsSelected = false;
    }

    private void SelectTab(WorkspaceTabViewModel tab)
    {
        SelectedTab = tab;
        HomeIsSelected = false;
        foreach (var t in WorkspaceTabs) t.IsSelected = (t == tab);
    }

    // ── Operations ───────────────────────────────────────────────

    public void OpenWorkspace(Workspace workspace)
    {
        // Bring existing tab to front instead of duplicating
        var existing = WorkspaceTabs.FirstOrDefault(t => t.Workspace.Id == workspace.Id);
        if (existing != null)
        {
            SelectTab(existing);
            return;
        }

        var tab = BuildTab(workspace);
        WorkspaceTabs.Add(tab);
        SelectTab(tab);
    }

    public void ShowNotification(string message)
    {
        NotificationMessage = message;
        IsNotificationVisible = true;

        DispatcherTimer.Run(() =>
        {
            IsNotificationVisible = false;
            return false; // stop timer
        }, TimeSpan.FromSeconds(5));
    }

    private void AddNewWorkspace()
    {
        var service = App.Services.GetRequiredService<WorkspaceService>();
        var workspace = service.Create("New Workspace", string.Empty);

        var tab = BuildTab(workspace);
        tab.IsNew = true;
        WorkspaceTabs.Add(tab);
        SelectTab(tab);

        // Start rename immediately so the user can set a real name
        tab.BeginRenameCommand.Execute(null);

        // Refresh the Home card list
        HomeTab.ViewModel.Refresh();
    }

    private void CloseTab(WorkspaceTabViewModel tab)
    {
        var idx = WorkspaceTabs.IndexOf(tab);
        WorkspaceTabs.Remove(tab);

        HomeTab.ViewModel.Refresh();

        if (SelectedTab == tab)
        {
            if (WorkspaceTabs.Count > 0)
                SelectTab(WorkspaceTabs[System.Math.Max(0, idx - 1)]);
            else
                SelectHome();
        }
    }

    private WorkspaceTabViewModel BuildTab(Workspace workspace)
    {
        var tab = new WorkspaceTabViewModel(workspace, ShowNotification);
        tab.RequestClose  = CloseTab;
        tab.SelectCommand = new RelayCommand(() => SelectTab(tab));
        return tab;
    }
    
    private void OnWorkspaceDeleted(int id)
{
    var tab = WorkspaceTabs.FirstOrDefault(t => t.Workspace.Id == id);
    if (tab != null)
        CloseTab(tab);
}
}

/// <summary>Marker for the static Home tab.</summary>
public class HomeTabSentinel
{
    public HomeViewModel ViewModel { get; } = new HomeViewModel();
}
