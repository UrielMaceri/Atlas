using System;
using System.Collections.Generic;
using Back.Classes;
using Back.Data;
using Back.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Back.Repositories;
public class CategoryRepo : ICategory
{
    private readonly AtlasDbContext _context;
    public CategoryRepo(AtlasDbContext context)
    {
        _context = context;
    }

    public List<Category> GetAll()
    {
        return _context.Categories
            .Include(c => c.Items)
            .ToList();
    }

    public Category? GetById(int id)
    {
        return _context.Categories
            .Include(c => c.Items)
            .FirstOrDefault(c => c.Id == id);
    }

    public List<Category> GetByName(string name)
    {
        return _context.Categories
            .Include(c => c.Items)
            .Where(c => c.Name.Contains(name))
            .ToList();
    }

    public void Insert(Category category)
    {
        _context.Categories.Add(category);
        _context.SaveChanges();
    }

    public void Update(Category category)
    {
        _context.Categories.Update(category);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var categoryToDelete = _context.Categories.Find(id);

        if (categoryToDelete == null)
        {
            throw new Exception($"Category {id} not found.");
        }

        _context.Categories.Remove(categoryToDelete);
        _context.SaveChanges();
    }

    public List<Category> GetByWorkspace(int workspaceId)
    {
        return _context.Categories
            .Include(c => c.Items)
            .Where(c => c.WorkspaceId == workspaceId)
            .ToList();
    }
}