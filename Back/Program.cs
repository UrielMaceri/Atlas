using System;
using Back.Classes;
using Back.Data;
using Microsoft.EntityFrameworkCore;
namespace Back.Main;

class MainApp
{
    static void Main(string[] args)
    {
        //Searching for the local appdata folder and creates the db there 
        string AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string AtlasFolder = Path.Combine(AppDataFolder, "AtlasOrganizer");
        Directory.CreateDirectory(AtlasFolder); //If exists, does nothing
        string DbPath = Path.Combine(AtlasFolder, "atlas.db");
        
        //Creates database if not exists
        var options = new DbContextOptionsBuilder<AtlasDbContext>()
        .UseSqlite($"Data Source={DbPath}")
        .Options;
        using var db = new AtlasDbContext(options);

        
    }
}