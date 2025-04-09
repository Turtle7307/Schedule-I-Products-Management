using System;
using DynamicData;

namespace Schedule_I_Products_Management.Data;

public class MixedProduct : IProduct
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BaseProductId { get; set; }
    public string Name { get; set; }
    public int AskingPrice { get; set; }
    public int Addictiveness { get; set; }
    public SourceList<Guid> MixablesIds { get; set; } = new();
}