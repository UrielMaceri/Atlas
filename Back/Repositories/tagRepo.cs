using System;
using System.Collections.Generic;
using Back.Classes;
using Back.Data;
using Back.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Back.Repositories;
public class TagRepo : ITag
{
    private readonly AtlasDbContext _context;
    public TagRepo(AtlasDbContext context)
    {
        _context = context;
    }

    public List<Tag> GetAll()
    {
        return _context.Tags.ToList();
    }
    public Tag? GetById(int id)
    {
        return _context.Tags.Find(id);
    }
    public List<Tag> GetByName(string name)
    {
        return _context.Tags
        .Where(t => t.Name.Contains(name))
        .ToList();
    }
    public void Insert(Tag tag)
    {
        _context.Tags.Add(tag);
        _context.SaveChanges();
    }
    public void Update(Tag tag)
    {
        _context.Tags.Update(tag);
        _context.SaveChanges();
    }
    public void Delete(int id)
    {
        var TagToDelete = _context.Tags.Find(id);

        if (TagToDelete == null){
            throw new Exception($"Tag {id} not found.");
        }
        else
        {
            _context.Tags.Remove(TagToDelete);
            _context.SaveChanges();
        }
    }
}