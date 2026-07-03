using System;
namespace Back.Classes;
public class Tag : BaseEntity
{
    public string Color {get; set;} = string.Empty;
    public int Workspace {get; set;} = 0;
}