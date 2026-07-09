using System;
using System.Collections.Generic;
using Back.Classes;
using Back.Repositories.Interfaces;

namespace Back.Services;

public class WorkspaceService
{
    private readonly IWorkspace _workspaceRepository;

    public WorkspaceService(IWorkspace workspaceRepository)
    {
        _workspaceRepository = workspaceRepository;
    }

    public List<Workspace> GetAll()
    {
        return _workspaceRepository.GetAll();
    }

    public Workspace? GetById(int id)
    {
        if (id <= 0)
            throw new ArgumentException("The Workspace id must be greater than zero.", nameof(id));

        return _workspaceRepository.GetById(id);
    }

    public List<Workspace> Search(string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return _workspaceRepository.GetAll();

        return _workspaceRepository.GetByName(searchTerm.Trim()) ?? new List<Workspace>();
    }

    public Workspace Create(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("The Workspace name is required.", nameof(name));

        var workspace = new Workspace
        {
            Name = name.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? string.Empty : description.Trim()
        };

        _workspaceRepository.Insert(workspace);
        return workspace;
    }

    public void Update(int id, string name, string description)
    {
        if (id <= 0)
            throw new ArgumentException("The Workspace id must be greater than zero.", nameof(id));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("The Workspace name is required.", nameof(name));

        var workspace = _workspaceRepository.GetById(id);
        if (workspace == null)
            throw new KeyNotFoundException($"Workspace with id {id} was not found.");

        workspace.Name = name.Trim();
        workspace.Description = string.IsNullOrWhiteSpace(description) ? string.Empty : description.Trim();

        _workspaceRepository.Update(workspace);
    }

    public void Delete(int id)
    {
        if (id <= 0)
            throw new ArgumentException("The Workspace id must be greater than zero.", nameof(id));

        var workspace = _workspaceRepository.GetById(id);
        if (workspace == null)
            throw new KeyNotFoundException($"Workspace with id {id} was not found.");

        _workspaceRepository.Delete(id);
    }
}