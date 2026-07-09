using System;
using System.Collections.Generic;
using Back.Classes;
using Back.Repositories.Interfaces;

namespace Back.Services;

public class CategoryService
{
    private readonly ICategory _categoryRepository;

    public CategoryService(ICategory categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public List<Category> GetAll()
    {
        return _categoryRepository.GetAll();
    }

    public Category? GetById(int id)
    {
        if (id <= 0)
            throw new ArgumentException("The category id must be greater than zero.", nameof(id));

        return _categoryRepository.GetById(id);
    }

    public List<Category> Search(string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return _categoryRepository.GetAll();

        return _categoryRepository.GetByName(searchTerm.Trim()) ?? new List<Category>();
    }

    public Category Create(string name, string description, int workspaceId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("The category name is required.", nameof(name));

        if (workspaceId <= 0)
            throw new ArgumentException("The workspace id must be greater than zero.", nameof(workspaceId));

        var category = new Category
        {
            Name = name.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? string.Empty : description.Trim(),
            WorkspaceId = workspaceId
        };

        _categoryRepository.Insert(category);
        return category;
    }

    public void Update(int id, string name, string description)
    {
        if (id <= 0)
            throw new ArgumentException("The category id must be greater than zero.", nameof(id));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("The category name is required.", nameof(name));

        var category = _categoryRepository.GetById(id);
        if (category == null)
            throw new KeyNotFoundException($"Category with id {id} was not found.");

        category.Name = name.Trim();
        category.Description = string.IsNullOrWhiteSpace(description) ? string.Empty : description.Trim();

        _categoryRepository.Update(category);
    }

    public void Delete(int id)
    {
        if (id <= 0)
            throw new ArgumentException("The category id must be greater than zero.", nameof(id));

        var category = _categoryRepository.GetById(id);
        if (category == null)
            throw new KeyNotFoundException($"Category with id {id} was not found.");

        if (category.Items.Count > 0)
            throw new InvalidOperationException("You cannot delete a category that still contains items.");

        _categoryRepository.Delete(id);
    }

    public List<Category> GetByWorkspace(int workspaceId)
    {
        if (workspaceId <= 0)
            throw new ArgumentException("The workspace id must be greater than zero.", nameof(workspaceId));

        return _categoryRepository.GetByWorkspace(workspaceId) ?? new List<Category>();
    }
}