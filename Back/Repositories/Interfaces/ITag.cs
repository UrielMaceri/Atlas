using System;
using System.Collections.Generic;
using Back.Classes;
using Microsoft.EntityFrameworkCore;

namespace Back.Repositories.Interfaces;

public interface ITag
{
    // Base 
    List<Tag> GetAll();
    Tag? GetById(int id);
    List<Tag>? GetByName(string name);
    void Insert(Tag tag);
    void Update(Tag tag);
    void Delete(int id);
}

