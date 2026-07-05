using System;
using System.Collections.Generic;
using Back.Classes;
using Microsoft.EntityFrameworkCore;

namespace Back.Repositories.Interfaces;

public interface IWorkspace
{
    // Base 
    List<Workspace> GetAll();
    Workspace? GetById(int id);
    List<Workspace>? GetByName(string name);
    void Insert(Workspace workspace);
    void Update(Workspace workspace);
    void Delete(int id);
}

