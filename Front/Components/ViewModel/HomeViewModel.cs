using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
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

    public HomeViewModel()
    {
        Refresh();
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

    internal void RemoveCard(WorkspaceCardViewModel card) => Workspaces.Remove(card);
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

        DeleteCommand = new RelayCommand(() =>
        {
            var service = App.Services.GetRequiredService<WorkspaceService>();
            service.Delete(Workspace.Id);
            _parent.RemoveCard(this);           
        });
    }
}
