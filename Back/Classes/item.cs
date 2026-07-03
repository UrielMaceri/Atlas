using System.Collections.Generic;
namespace Back.Classes;
public class Item : BaseEntity
{
    public string Path { get; set; } = string.Empty;
    public string IconPath { get; set; } = string.Empty;
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
    public List<Tag> Tags { get; set; } = new();
    public bool IsFavorite { get; set; } = false;
}