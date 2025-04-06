using System;

namespace Schedule_I_Products_Management.Data;

public interface IProduct
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int AskingPrice { get; set; }
    public int Addictiveness { get; set; }
    public ProductCategory Category { get; set; }
}