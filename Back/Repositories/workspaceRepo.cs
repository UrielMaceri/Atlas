using System;
using System.Collections.Generic;
using Back.Classes;
using Back.Data;
using Back.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Back.Repositories;
public class WorkspaceRepo : IWorkspace
{
    private readonly AtlasDbContext _context;
    public WorkspaceRepo(AtlasDbContext context)
    {
        _context = context;
    }

    public List<Workspace> GetAll()
    {
        return _context.Workspaces.ToList();
    }
    public Workspace? GetById(int id)
    {
        return _context.Workspaces.Find(id);
    }
    public List<Workspace> GetByName(string name)
    {
        return _context.Workspaces
        .Where(w => w.Name.Contains(name))
        .ToList();
    }
    public void Insert(Workspace workspace)
    {
        _context.Workspaces.Add(workspace);
        _context.SaveChanges();
    }
    public void Update(Workspace workspace)
    {
        _context.Workspaces.Update(workspace);
        _context.SaveChanges();
    }
    public void Delete(int id)
    {
        var workspaceToDelete = _context.Workspaces.Find(id);

        if (workspaceToDelete == null){
            throw new Exception($"Workspace {id} not found.");
        }
        else
        {
            _context.Workspaces.Remove(workspaceToDelete);
            _context.SaveChanges();
        }
    }
}