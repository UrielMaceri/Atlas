using System;
using System.Collections.Generic;
using Back.Classes;
using Microsoft.EntityFrameworkCore;

namespace Back.Repositories.Interfaces;

public interface ICategory
{   
    // Base 
    List<Category> GetAll();
    Category? GetById(int id);
    List<Category>? GetByName(string name);
    void Insert(Category category);
    void Update(Category category);
    void Delete(int id);

    // User
    List<Category>? GetByWorkspace(int workspaceId);
}

