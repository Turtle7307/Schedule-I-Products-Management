using System;

namespace Schedule_I_Products_Management.Data;

public class BaseProduct
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public int Cost { get; set; }
    public int AskingPrice { get; set; }
    public int Addictiveness { get; set; }
    public Guid EffectId { get; set; }
    public ProductCategory Category { get; set; }
    
    public override string ToString()
    {
        return Name;
    }
}