using System;
namespace Back.Classes;
public abstract class BaseEntity
{
    public int Id {get; set;} = 0;
    public string Name {get; set;} = string.Empty;
    public string Description {get; set;} = string.Empty;

}