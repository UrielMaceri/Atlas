using System;
using System.Windows.Input;
using Back.Classes;
using Back.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Front;

public class WorkspaceTabViewModel : ReactiveObject
{
    private string _name;
    private bool _isSelected;
    private bool _isEditingName;
    private string _editingName = string.Empty;
    private bool _isNew;

    public Workspace Workspace { get; }

    public string Name
    {
        get => _name;
        private set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => this.RaiseAndSetIfChanged(ref _isSelected, value);
    }

    public bool IsEditingName
    {
        get => _isEditingName;
        set => this.RaiseAndSetIfChanged(ref _isEditingName, value);
    }

    public string EditingName
    {
        get => _editingName;
        set => this.RaiseAndSetIfChanged(ref _editingName, value);
    }

    public bool IsNew
    {
        get => _isNew;
        set => this.RaiseAndSetIfChanged(ref _isNew, value);
    }

    // Wired by MainWindowViewModel after construction
    public Action<WorkspaceTabViewModel>? RequestClose { get; set; }
    public ICommand? SelectCommand { get; set; }

    public ICommand CloseCommand { get; }
    public ICommand BeginRenameCommand { get; }
    public ICommand CommitRenameCommand { get; }
    public ICommand CancelRenameCommand { get; }

    public WorkspaceTabViewModel(Workspace workspace)
    {
        Workspace = workspace;
        _name = workspace.Name;

        CloseCommand = new RelayCommand(() => RequestClose?.Invoke(this));

        BeginRenameCommand = new RelayCommand(() =>
        {
            EditingName = Name;
            IsEditingName = true;
        });

        CommitRenameCommand = new RelayCommand(() =>
        {
            var trimmed = EditingName.Trim();
            if (string.IsNullOrWhiteSpace(trimmed)) trimmed = Name;

            var service = App.Services.GetRequiredService<WorkspaceService>();
            service.Update(Workspace.Id, trimmed, Workspace.Description);
            Workspace.Name = trimmed;
            Name = trimmed;
            IsEditingName = false;
            IsNew = false;
        });

        CancelRenameCommand = new RelayCommand(() =>
        {
            IsEditingName = false;
            if (IsNew)
            {
                var service = App.Services.GetRequiredService<WorkspaceService>();
                service.Delete(Workspace.Id);
                CloseCommand.Execute(null);
            }
            
        });
    }
}
