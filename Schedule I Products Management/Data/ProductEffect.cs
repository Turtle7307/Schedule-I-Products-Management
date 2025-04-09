using System;

namespace Schedule_I_Products_Management.Data;

public class ProductEffect
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
}