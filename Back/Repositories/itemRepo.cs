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
        return _context.Items.ToList();
    }
    public Item? GetById(int id)
    {
        return _context.Items.Find(id);
    }
    public List<Item> GetByName(string name)
    {
        return _context.Items
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
        var ItemToDelete = _context.Items.Find(id);

        if (ItemToDelete == null){
            throw new Exception($"Item {id} not found.");
        }
        else
        {
            _context.Items.Remove(ItemToDelete);
            _context.SaveChanges();
        }
    }
    public List<Item> GetByCategory(int categoryId)
    {
        return _context.Items
        .Where(i => i.CategoryId.Equals(categoryId))
        .ToList();
    }
    public List<Item> GetFavs()
    {
        return _context.Items
        .Where(i => i.IsFavorite.Equals(true))
        .ToList();
    }
    
    // ItemTags join table
    public List<Item> GetByTag(int tagId)
    {
        return _context.Tags
        .Where(t => t.Id == tagId)
        .SelectMany(i => i.Items)
        .ToList();
    }
    public List<Tag> GetTagsByItem(int itemId)
    {
        return _context.Items
        .Where(i => i.Id == itemId)
        .SelectMany(t => t.Tags)
        .ToList();
    }
    public void UpdateItemTags(Item item)
    {
        var ItemTags = _context.Items.Include(i =>i.Tags)
                                .SingleOrDefault(i => i.Id == item.Id);

        if (ItemTags == null)
            throw new Exception($"Item {item.Id} not found.");

        ItemTags.Tags.Clear();
        // Only saves in join table if there i
        if (item.Tags != null){
            foreach (var tag in item.Tags)
            {
                ItemTags.Tags.Add(tag);
            }
            _context.SaveChanges();
        } 
    }
}