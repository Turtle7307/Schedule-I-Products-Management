using System;

namespace Schedule_I_Products_Management.Data;

public interface IBuyable
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Cost { get; set; }
}