using System;
using System.Collections.Generic;
namespace Back.Classes;
public class Category : BaseEntity
{
    public int Workspace {get; set;} = 0;
    public List<Item> Items {get; set;} = new();

}