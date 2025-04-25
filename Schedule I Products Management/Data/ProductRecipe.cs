using System;

namespace Schedule_I_Products_Management.Data;

public class ProductRecipe
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BaseProductId { get; set; }
    public Guid MixableId { get; set; }
}