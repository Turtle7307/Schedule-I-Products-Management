using System;
using System.Collections.Generic;

namespace Schedule_I_Products_Management.Data;

public class MixedProduct
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public int AskingPrice { get; set; }
    public int Addictiveness { get; set; }
    public Guid? BaseProductId { get; set; }
    public List<Guid> EffectIds { get; set; } = new();
    public List<ProductRecipe> ProductRecipes { get; set; } = new();
}