using System;
using System.Collections.Generic;
using Back.Classes;
using Back.Repositories.Interfaces;

namespace Back.Services;

public class TagService
{
    private readonly ITag _tagRepository;

    public TagService(ITag tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public List<Tag> GetAll()
    {
        return _tagRepository.GetAll();
    }

    public Tag? GetById(int id)
    {
        if (id <= 0)
            throw new ArgumentException("The Tag id must be greater than zero.", nameof(id));

        return _tagRepository.GetById(id);
    }

    public List<Tag> Search(string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return _tagRepository.GetAll();

        return _tagRepository.GetByName(searchTerm.Trim()) ?? new List<Tag>();
    }

    public Tag Create(string name, string description, int workspaceId, string color)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("The Tag name is required.", nameof(name));

        if (workspaceId <= 0)
            throw new ArgumentException("The workspace id must be greater than zero.", nameof(workspaceId));

        var tag = new Tag
        {
            Name = name.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? string.Empty : description.Trim(),
            WorkspaceId = workspaceId,
            Color = color
        };

        _tagRepository.Insert(tag);
        return tag;
    }

    public void Update(int id, string name, string description, string color)
    {
        if (id <= 0)
            throw new ArgumentException("The Tag id must be greater than zero.", nameof(id));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("The Tag name is required.", nameof(name));

        var tag = _tagRepository.GetById(id);
        if (tag == null)
            throw new KeyNotFoundException($"Tag with id {id} was not found.");

        tag.Name = name.Trim();
        tag.Description = string.IsNullOrWhiteSpace(description) ? string.Empty : description.Trim();
        tag.Color = color;

        _tagRepository.Update(tag);
    }

    public void Delete(int id)
    {
        if (id <= 0)
            throw new ArgumentException("The Tag id must be greater than zero.", nameof(id));

        var tag = _tagRepository.GetById(id);
        if (tag == null)
            throw new KeyNotFoundException($"Tag with id {id} was not found.");

        if (tag.Items.Count > 0)
            throw new InvalidOperationException("You cannot delete a Tag that still contains items.");

        _tagRepository.Delete(id);
    }
}