using System;
using System.Collections.Generic;
using Back.Classes;
using Back.Data;
using Back.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Back.Repositories;
public class ItemRepo : IItem
{
    private readonly AtlasDbContext _context;
    public ItemRepo(AtlasDbContext context)
    {
        _context = context;
    }

    public List<Item> GetAll()
    {
        return _context.Items
            .Include(i => i.Category)
            .Include(i => i.Tags)
            .ToList();
    }

    public Item? GetById(int id)
    {
        return _context.Items
            .Include(i => i.Category)
            .Include(i => i.Tags)
            .FirstOrDefault(i => i.Id == id);
    }

    public List<Item> GetByName(string name)
    {
        return _context.Items
            .Include(i => i.Category)
            .Include(i => i.Tags)
            .Where(i => i.Name.Contains(name))
            .ToList();
    }

    public void Insert(Item item)
    {
        _context.Items.Add(item);
        _context.SaveChanges();
    }

    public void Update(Item item)
    {
        _context.Items.Update(item);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var itemToDelete = _context.Items.Find(id);

        if (itemToDelete == null)
        {
            throw new Exception($"Item {id} not found.");
        }

        _context.Items.Remove(itemToDelete);
        _context.SaveChanges();
    }

    public List<Item> GetByCategory(int categoryId)
    {
        return _context.Items
            .Include(i => i.Category)
            .Include(i => i.Tags)
            .Where(i => i.CategoryId == categoryId)
            .ToList();
    }

    public List<Item> GetFavs()
    {
        return _context.Items
            .Include(i => i.Category)
            .Include(i => i.Tags)
            .Where(i => i.IsFavorite)
            .ToList();
    }

    public List<Item> GetByTag(int tagId)
    {
        return _context.Items
            .Include(i => i.Category)
            .Include(i => i.Tags)
            .Where(i => i.Tags.Any(t => t.Id == tagId))
            .ToList();
    }

    public List<Tag> GetTagsByItem(int itemId)
    {
        return _context.Items
            .Include(i => i.Tags)
            .Where(i => i.Id == itemId)
            .SelectMany(i => i.Tags)
            .ToList();
    }

    public void UpdateItemTags(Item item)
    {
        var itemTags = _context.Items
            .Include(i => i.Tags)
            .SingleOrDefault(i => i.Id == item.Id);

        if (itemTags == null)
            throw new Exception($"Item {item.Id} not found.");

        itemTags.Tags.Clear();

        if (item.Tags != null)
        {
            foreach (var tag in item.Tags)
            {
                itemTags.Tags.Add(tag);
            }

            _context.SaveChanges();
        }
    }
}