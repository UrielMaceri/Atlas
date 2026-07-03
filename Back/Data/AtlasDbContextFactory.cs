using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Back.Data;

public class AtlasDbContextFactory : IDesignTimeDbContextFactory<AtlasDbContext>
{
    public AtlasDbContext CreateDbContext(string[] args)
    {
        string AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string AtlasFolder = Path.Combine(AppDataFolder, "AtlasOrganizer");
        Directory.CreateDirectory(AtlasFolder);

        string DbPath = Path.Combine(AtlasFolder, "atlas.db");

        var OptionsBuilder = new DbContextOptionsBuilder<AtlasDbContext>();
        OptionsBuilder.UseSqlite($"Data Source={DbPath}");

        return new AtlasDbContext(OptionsBuilder.Options);
    }
}
// While in Back/ 
// dotnet ef migrations add InitialCreate -  if this is the first time
// dotnet ef migrations add "NewName" -  if this is a schema update
// dotnet ef database update - to create/update the sqlite file at the default path ('AppDataFolder'/AtlasOrganizer)
// dotnet ef database drop - if the current history or database needs to be reset

