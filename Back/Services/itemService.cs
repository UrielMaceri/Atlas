using System;
using System.Collections.Generic;
using Back.Classes;
using Back.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Back.Services;

public class ItemService
{
    private readonly IItem _itemRepository;
    private readonly ITag _tagRepository;

    public ItemService(IItem itemRepository, ITag tagRepository)
    {
        _itemRepository = itemRepository;
        _tagRepository = tagRepository;
    }

    public List<Item> GetAll()
    {
        return _itemRepository.GetAll();
    }

    public Item? GetById(int id)
    {
        if (id <= 0)
            throw new ArgumentException("The Item id must be greater than zero.", nameof(id));

        return _itemRepository.GetById(id);
    }

    public List<Item> Search(string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return _itemRepository.GetAll();

        return _itemRepository.GetByName(searchTerm.Trim()) ?? new List<Item>();
    }

    public Item Create(string name, string description, string path, string iconPath, int categoryId, bool isFavorite)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("The Item name is required.", nameof(name));

        if (categoryId <= 0)
            throw new ArgumentException("The category id must be greater than zero.", nameof(categoryId));

        var item = new Item
        {
            Name = name.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? string.Empty : description.Trim(),
            Path = string.IsNullOrWhiteSpace(path) ? string.Empty : path.Trim(),
            IconPath = string.IsNullOrWhiteSpace(iconPath) ? string.Empty : iconPath.Trim(),
            CategoryId = categoryId,
            IsFavorite = isFavorite
        };

        _itemRepository.Insert(item);
        return item;
    }

    public void Update(int id, string name, string description, string path, string iconPath, int categoryId, bool isFavorite)
    {
        if (id <= 0)
            throw new ArgumentException("The Item id must be greater than zero.", nameof(id));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("The Item name is required.", nameof(name));

        var item = _itemRepository.GetById(id);
        if (item == null)
            throw new KeyNotFoundException($"Item with id {id} was not found.");

        item.Name = name.Trim();
        item.Description = string.IsNullOrWhiteSpace(description) ? string.Empty : description.Trim();
        item.Path = string.IsNullOrWhiteSpace(path) ? string.Empty : path.Trim();
        item.IconPath = string.IsNullOrWhiteSpace(iconPath) ? string.Empty : iconPath.Trim();
        item.CategoryId = categoryId;
        item.IsFavorite = isFavorite;

        _itemRepository.Update(item);
    }

    public void Delete(int id)
    {
        if (id <= 0)
            throw new ArgumentException("The Item id must be greater than zero.", nameof(id));

        var item = _itemRepository.GetById(id);
        if (item == null)
            throw new KeyNotFoundException($"Item with id {id} was not found.");

        _itemRepository.Delete(id);
    }

    public List<Item> GetByCategory(int categoryId)
    {
        if (categoryId <= 0)
            throw new ArgumentException("The category id must be greater than zero.", nameof(categoryId));

        return _itemRepository.GetByCategory(categoryId) ?? new List<Item>();
    }

    public List<Item> GetFavs()
    {
            return _itemRepository.GetFavs() ?? new List<Item>();
    }


    // ------------ JOIN TABLE METHODS ------------     
    public List<Item> GetByTag(int tagId)
    {
        if (tagId <= 0)
            throw new ArgumentException("The tag id must be greater than zero.", nameof(tagId));

        return _itemRepository.GetByTag(tagId) ?? new List<Item>();
    }
    public List<Tag> GetTagsByItem(int itemId)
    {
        if (itemId <= 0)
            throw new ArgumentException("The item id must be greater than zero.", nameof(itemId));

        var foundItem = _itemRepository.GetById(itemId);

        if (foundItem == null)
            throw new ArgumentException("This item does not exist in the database.",nameof(itemId));
        
        if (foundItem.Tags.Count <= 0)
            return new List<Tag>();
        
        List<Tag> returnTags =  foundItem.Tags;
        return returnTags;
    }

    public void AddTagToItem(int itemId, int tagId)
    {
        if (itemId <= 0)
            throw new ArgumentException("The item id must be greater than zero.", nameof(itemId));
        if (tagId <= 0)
            throw new ArgumentException("The tag id must be greater than zero.", nameof(tagId));

        var updateItem = _itemRepository.GetById(itemId);
        var tag = _tagRepository.GetById(tagId);
        
        if (updateItem == null)
             throw new ArgumentException("This item does not exist in the database.",nameof(itemId));
        if (tag == null)
             throw new ArgumentException("Selected tag does not exist in the database.",nameof(itemId));

        updateItem.Tags.Add(tag);
        _itemRepository.UpdateItemTags(updateItem);
    }
    public void RemoveTagFromItem(int itemId, int tagId)
    {
        if (itemId <= 0)
            throw new ArgumentException("The item id must be greater than zero.", nameof(itemId));
        if (tagId <= 0)
            throw new ArgumentException("The tag id must be greater than zero.", nameof(tagId));

        var updateItem = _itemRepository.GetById(itemId);
        var removingTag = _tagRepository.GetById(tagId);
        
        if (updateItem == null)
             throw new ArgumentException("This item does not exist in the database.",nameof(itemId));
        if (removingTag == null)
             throw new ArgumentException("Selected tag does not exist in the database.",nameof(itemId));

        updateItem.Tags.RemoveAll(t => t.Id == removingTag.Id);
        _itemRepository.UpdateItemTags(updateItem);
    }
}