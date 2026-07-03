using System.Collections.Generic;
namespace Back.Classes;
public class Tag : BaseEntity
{
    public string Color { get; set; } = string.Empty;
    public int WorkspaceId { get; set; }
    public Workspace? Workspace { get; set; }
    public List<Item> Items { get; set; } = new();
}