using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Back.Classes;
using Back.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Front;

public class HomeViewModel : ReactiveObject
{
    public ObservableCollection<WorkspaceCardViewModel> Workspaces { get; } = new();

    /// <summary>Set by MainWindowViewModel so cards can open a tab.</summary>
    public Action<Workspace>? OpenWorkspaceAction { get; set; }

    /// <summary>Set by MainWindowViewModel to display update status.</summary>
    public Action<string>? NotificationAction { get; set; }

    /// <summary>Set by HomeView so deletion can be confirmed before it is performed.</summary>
    public Func<WorkspaceCardViewModel, Task>? DeleteConfirmationAction { get; set; }

    private bool _isUpdateAvailable;
    public bool IsUpdateAvailable
    {
        get => _isUpdateAvailable;
        private set
        {
            this.RaiseAndSetIfChanged(ref _isUpdateAvailable, value);
            this.RaisePropertyChanged(nameof(IsUpdateButtonVisible));
        }
    }

    private bool _isUpdateBusy;
    public bool IsUpdateBusy
    {
        get => _isUpdateBusy;
        private set => this.RaiseAndSetIfChanged(ref _isUpdateBusy, value);
    }

    private string _updateStatusMessage = string.Empty;
    public string UpdateStatusMessage
    {
        get => _updateStatusMessage;
        private set
        {
            this.RaiseAndSetIfChanged(ref _updateStatusMessage, value);
            this.RaisePropertyChanged(nameof(HasUpdateStatusMessage));
        }
    }

    public bool IsDevelopmentVersion => Version == "dev";
    public bool IsUpdateButtonVisible => IsDevelopmentVersion || IsUpdateAvailable;
    public bool HasUpdateStatusMessage => !string.IsNullOrEmpty(UpdateStatusMessage);

    public ICommand UpdateCommand { get; }

    public HomeViewModel()
    {
        UpdateCommand = new RelayCommand(() => _ = UpdateAsync());
        Refresh();
        _ = CheckForUpdatesOnStartupAsync();
    }

    public void Refresh()
    {
        var service = App.Services.GetRequiredService<WorkspaceService>();
        var all = service.GetAll();
        Workspaces.Clear();
        foreach (var ws in all)
            Workspaces.Add(new WorkspaceCardViewModel(ws, this));
    }

    internal void RequestOpen(Workspace workspace) => OpenWorkspaceAction?.Invoke(workspace);

    internal Task RequestDelete(WorkspaceCardViewModel card) =>
        DeleteConfirmationAction?.Invoke(card) ?? Task.CompletedTask;

    internal void RemoveCard(WorkspaceCardViewModel card) => Workspaces.Remove(card);

    private async Task CheckForUpdatesOnStartupAsync()
    {
        IsUpdateBusy = true;
        UpdateStatusMessage = "Checking for updates...";
        try
        {
            var update = await Updater.Check();
            IsUpdateAvailable = update is not null;
            UpdateStatusMessage = IsUpdateAvailable
                ? "There are updates pending, please update."
                : string.Empty;
        }
        catch (Exception ex)
        {
            UpdateStatusMessage = $"Could not check for updates: {ex.Message}";
        }
        finally
        {
            IsUpdateBusy = false;
        }
    }

    private async Task UpdateAsync()
    {
        if (IsUpdateBusy)
            return;

        IsUpdateBusy = true;
        try
        {
            UpdateStatusMessage = "Checking for updates...";
            var update = await Updater.Check();
            if (update is null)
            {
                IsUpdateAvailable = false;
                UpdateStatusMessage = string.Empty;
                NotificationAction?.Invoke("Atlas is up to date.");
                return;
            }

            IsUpdateAvailable = true;
            UpdateStatusMessage = "There are updates pending, please update.";
            var version = update.TargetFullRelease.Version;
            NotificationAction?.Invoke($"Downloading update {version}...");
            await Updater.DownloadAndRestartAsync(update);
        }
        catch (Exception ex)
        {
            UpdateStatusMessage = $"Update failed: {ex.Message}";
            NotificationAction?.Invoke($"Update failed: {ex.Message}");
        }
        finally
        {
            IsUpdateBusy = false;
        }
    }
    
    private static UpdateService Updater = new();
    public string Version { get;} = Updater.GetVersion();
} 

// ─────────────────────────────────────────────────────────────────────────────

public class WorkspaceCardViewModel : ReactiveObject
{
    private readonly HomeViewModel _parent;
    private string _name;
    private string _description;
    private bool _isEditing;
    private string _editingName = string.Empty;
    private string _editingDescription = string.Empty;
    

    public Workspace Workspace { get; }

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

    public bool IsEditing
    {
        get => _isEditing;
        set => this.RaiseAndSetIfChanged(ref _isEditing, value);
    }

    public string EditingName
    {
        get => _editingName;
        set => this.RaiseAndSetIfChanged(ref _editingName, value);
    }

    public string EditingDescription
    {
        get => _editingDescription;
        set => this.RaiseAndSetIfChanged(ref _editingDescription, value);
    }

    public ICommand OpenCommand { get; }
    public ICommand BeginEditCommand { get; }
    public ICommand CommitEditCommand { get; }
    public ICommand CancelEditCommand { get; }
    public ICommand DeleteCommand { get; }

    public WorkspaceCardViewModel(Workspace workspace, HomeViewModel parent)
    {
        Workspace = workspace;
        _parent = parent;
        _name = workspace.Name;
        _description = workspace.Description;

        OpenCommand = new RelayCommand(() => _parent.RequestOpen(Workspace));

        BeginEditCommand = new RelayCommand(() =>
        {
            EditingName = Name;
            EditingDescription = Description;
            IsEditing = true;
        });

        CommitEditCommand = new RelayCommand(() =>
        {
            var trimmedName = EditingName.Trim();
            if (string.IsNullOrWhiteSpace(trimmedName)) trimmedName = Name;
            var trimmedDesc = EditingDescription.Trim();

            var service = App.Services.GetRequiredService<WorkspaceService>();
            service.Update(Workspace.Id, trimmedName, trimmedDesc);

            Workspace.Name = trimmedName;
            Workspace.Description = trimmedDesc;
            Name = trimmedName;
            Description = trimmedDesc;
            IsEditing = false;
        });

        CancelEditCommand = new RelayCommand(() => IsEditing = false);

        DeleteCommand = new RelayCommand(() => _ = _parent.RequestDelete(this));
    }

    internal void DeleteConfirmed()
    {
        var service = App.Services.GetRequiredService<WorkspaceService>();
        service.Delete(Workspace.Id);
        _parent.RemoveCard(this);
        _parent.Refresh();
    }
}
