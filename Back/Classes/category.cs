using System.Collections.Generic;
namespace Back.Classes;
public class Category : BaseEntity
{
    public int WorkspaceId { get; set; }
    public Workspace? Workspace { get; set; }
    public List<Item> Items { get; set; } = new();
}