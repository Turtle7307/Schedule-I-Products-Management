using System.Collections.Generic;
using Schedule_I_Products_Management.Data;

namespace Schedule_I_Products_Management.Models;

public interface IProductWrapperShowData
{
    public string Name { get; }
    public int Cost { get; }
    public int AskingPrice { get; }
    public int Addictiveness { get; }
    public ProductCategory Category { get; }
    public bool IsMixed { get; }
    public List<MixableWrapper> Mixables { get; }
}