using System.Collections.ObjectModel;
using Back.Classes;
using Back.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Front;

public class HomeViewModel
{
    public ObservableCollection<Workspace> Workspaces { get; } = new();

    public HomeViewModel()
    {
        LoadWorkspaces();
    }

    private void LoadWorkspaces()
    {
        var service = App.Services.GetRequiredService<WorkspaceService>();
        var all = service.GetAll();
        Workspaces.Clear();
        foreach (var ws in all)
            Workspaces.Add(ws);
    }
}
