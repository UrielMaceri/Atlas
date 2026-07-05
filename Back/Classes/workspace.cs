using System.Collections.Generic;
namespace Back.Classes;
public class Workspace : BaseEntity
{
    public List<Category>? Categories { get; set; } = null;
    public List<Tag>? Tags { get; set; } = null;
}