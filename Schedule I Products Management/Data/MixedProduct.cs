using System;
using System.Collections.Generic;

namespace Schedule_I_Products_Management.Data;

public class MixedProduct
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BaseProductId { get; set; }
    public string Name { get; set; }
    public int AskingPrice { get; set; }
    public int Addictiveness { get; set; }
    public List<Guid> MixablesIds { get; set; } = new();
    public List<Guid> EffectIds { get; set; } = new();
}