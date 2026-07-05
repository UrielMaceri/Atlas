using System;
using System.Collections.Generic;
using Back.Classes;
using Microsoft.EntityFrameworkCore;

namespace Back.Repositories.Interfaces;

public interface IItem
{
    // Base 
    List<Item> GetAll();
    Item? GetById(int id);
    List<Item>? GetByName(string name);
    void Insert(Item item);
    void Update(Item item);
    void Delete(int id);

    // User
    List<Item>? GetByCategory(int categoryId);
    List<Item>? GetFavs();
    
    List<Item>? GetByTag(int tagId); // join table
    List<Tag> GetTagsByItem(int itemId); // join table
    void UpdateItemTags(Item item); // join table


    /* This logic should go in itemService object 

    void AddTagToItem(int itemId, int tagId); // join table
    void RemoveTagFromItem(int itemId, int tagId); // join table
    
    then just call itemRepo.UpdateItemTags instead of regular Update */
}

