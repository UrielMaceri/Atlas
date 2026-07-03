using System;
using System.Collections.Generic;
using Back.Classes;
using Microsoft.EntityFrameworkCore;

namespace Back.Data;

public class AtlasDbContext : DbContext
{
    public DbSet<Workspace> Workspaces { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Item> Items { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;

    //Constructor 
    public AtlasDbContext(DbContextOptions<AtlasDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Workspace>(entity =>
        {
            entity.ToTable("Workspace");
            entity.HasKey(workspace=> workspace.Id);

            entity.HasMany(workspace=> workspace.Categories)
                .WithOne(category=> category.Workspace)
                .HasForeignKey(category=> category.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(workspace=> workspace.Tags)
                .WithOne(tag => tag.Workspace)
                .HasForeignKey(tag => tag.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");
            entity.HasKey(category=> category.Id);

            entity.HasMany(category=> category.Items)
                .WithOne(item=> item.Category)
                .HasForeignKey(item=> item.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.ToTable("Item");
            entity.HasKey(item=> item.Id);

            entity.HasMany(item=> item.Tags)
                .WithMany(tag => tag.Items)
                .UsingEntity<Dictionary<string, object>>(
                    "ItemTags",
                    rTag => rTag.HasOne<Tag>().WithMany().HasForeignKey("TagId"),
                    lItem => lItem.HasOne<Item>().WithMany().HasForeignKey("ItemId"),
                    join =>
                    {
                        join.ToTable("ItemTags");
                        join.HasKey("ItemId", "TagId");
                    });
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("Tag");
            entity.HasKey(tag => tag.Id);
        });
    }
}