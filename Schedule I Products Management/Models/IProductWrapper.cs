using System;
using System.Collections.ObjectModel;
using Schedule_I_Products_Management.Data;

namespace Schedule_I_Products_Management.Models;

public interface IProductWrapper
{
    public Guid Id { get; }
    public string Name { get; }
    public int Cost { get; }
    public int AskingPrice { get; }
    public int Profit { get; }
    public int Addictiveness { get; }
    public ProductCategory Category { get; }
    public bool IsMixed { get; }
    public ReadOnlyObservableCollection<ProductEffectWrapper> Effects { get; }
    public ReadOnlyObservableCollection<ProductRecipeWrapper> Recipes { get; }
}