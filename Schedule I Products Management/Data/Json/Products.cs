using System.Collections.Generic;

namespace Schedule_I_Products_Management.Data.Json;

public class Products
{
    public string GameVersion { get; set; }
    public List<string> DiscoveredProducts { get; set; }
    public List<string> ListedProducts { get; set; }
    public List<MixRecipe> MixRecipes { get; set; }
    public List<ProductPrice> ProductPrices { get; set; }
    public List<string> FavoritedProducts { get; set; }
}