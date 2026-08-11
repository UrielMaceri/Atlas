using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia.Metadata;
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
    private bool _isCreating;
    private string _newCategoryName = string.Empty;
    private string _newCategoryDescription = string.Empty;

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
    
    public bool IsCreating
    {
        get => _isCreating;
        set => this.RaiseAndSetIfChanged(ref _isCreating, value);
    }

    public string NewCategoryName
    {
        get => _newCategoryName;
        set => this.RaiseAndSetIfChanged(ref _newCategoryName, value);
    }

    public string NewCategoryDescription
    {
        get => _newCategoryDescription;
        set => this.RaiseAndSetIfChanged(ref _newCategoryDescription, value);
    }

    // Wired by MainWindowViewModel after construction
    public Action<WorkspaceTabViewModel>? RequestClose { get; set; }
    public ICommand? SelectCommand { get; set; }
    public ICommand CloseCommand { get; }
    public ICommand BeginRenameCommand { get; }
    public ICommand CommitRenameCommand { get; }
    public ICommand CancelRenameCommand { get; }
    public ICommand AddNewCategory { get; }
    public ICommand CommitCreateCategoryCommand { get; }
    public ICommand CancelCreateCategoryCommand { get; }

    public Workspace Workspace { get; }
    public ObservableCollection<CategoryViewModel> Categories { get; } = new();
    public ObservableCollection<TagViewModel> Tags { get; } = new();

    public ObservableCollection<CategoryViewModel> LoadCategories(int workspaceId)
    {
        var service = App.Services.GetRequiredService<CategoryService>();

        var CategoriesInWorkspace = service.GetByWorkspace(workspaceId);

        Categories.Clear();
        foreach (var cat in CategoriesInWorkspace)
            Categories.Add(new CategoryViewModel(cat, this));
        return Categories;
    }

    public ObservableCollection<TagViewModel> LoadTags(int workspaceId)
    {
        var service = App.Services.GetRequiredService<TagService>();

        var TagsInWorkspace = service.GetByWorkspace(workspaceId);

        Tags.Clear();
        foreach (var tag in TagsInWorkspace)
            Tags.Add(new TagViewModel(tag, this));
        return Tags;
    }

    public WorkspaceTabViewModel(Workspace workspace)
    {
        Workspace = workspace;
        _name = workspace.Name;

        LoadCategories(Workspace.Id);
        LoadTags(Workspace.Id);

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

        AddNewCategory = new RelayCommand(() =>
        {
            NewCategoryName = string.Empty;
            NewCategoryDescription = string.Empty;
            IsCreating = true;
        });

        CommitCreateCategoryCommand = new RelayCommand(() =>
        {
            var name = NewCategoryName?.Trim();
            if (string.IsNullOrWhiteSpace(name))
                name = "New Category";

            var service = App.Services.GetRequiredService<CategoryService>();
            service.Create(name, NewCategoryDescription?.Trim() ?? string.Empty, workspace.Id);
            LoadCategories(workspace.Id);

            NewCategoryName = string.Empty;
            NewCategoryDescription = string.Empty;
            IsCreating = false;
        });

        CancelCreateCategoryCommand = new RelayCommand(() =>
        {
            IsCreating = false;
            NewCategoryName = string.Empty;
            NewCategoryDescription = string.Empty;
        });
    }
}
