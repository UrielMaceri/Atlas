using System;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace Back.Data;

public static class DbBootstrapper
{
    public static AtlasDbContext CreateAndMigrate()
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
        var db = new AtlasDbContext(options);
        db.Database.Migrate(); 
        return db;
    }
}