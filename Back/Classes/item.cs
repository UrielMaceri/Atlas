using System;
using System.Reflection.Metadata;
namespace Back.Classes;
public class Item : BaseEntity
{
    public string Path {get; set;} = string.Empty;
    public string IconPath {get; set;} = string.Empty; 
    public List<Tag> Tags {get; set;} = new();
    public int? Category {get; set;} = 0; //Setting 0 as a default category means it's "uncategorized" 
    public bool IsFavorite {get; set;} = false;
}