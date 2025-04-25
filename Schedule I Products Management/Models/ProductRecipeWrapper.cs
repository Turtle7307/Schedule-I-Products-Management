using System;
using System.Linq;
using Avalonia.Data;
using ReactiveUI;
using Schedule_I_Products_Management.Data;
using Schedule_I_Products_Management.Views;

namespace Schedule_I_Products_Management.Models;

public class ProductRecipeWrapper : ReactiveObject
{
    private ProductRecipe _productRecipe;

    public ProductRecipeWrapper(ProductRecipe productRecipe)
    {
        _productRecipe = productRecipe;
    }
    
    public Guid Id => _productRecipe.Id;

    public IProductWrapper? BaseProduct
    {
        get => MainWindow.ViewModel.BindableProducts.FirstOrDefault(p => p.Id == _productRecipe.BaseProductId);
        set
        {
            if (value == null)
                throw new DataValidationException("can not set null");
            _productRecipe.BaseProductId = value.Id;
            this.RaisePropertyChanged();
            this.RaisePropertyChanged(nameof(AsString));
        }
    }

    public MixableWrapper? AddedMixable
    {
        get => MainWindow.ViewModel.Mixables.Items.FirstOrDefault(m => m.Id == _productRecipe.MixableId);
        set {
            if (value == null)
                throw new DataValidationException("can not set null");
            _productRecipe.MixableId = value.Id;
            this.RaisePropertyChanged();
            this.RaisePropertyChanged(nameof(AsString));
        }
    }

    public string AsString => $"{BaseProduct?.Name ?? "Not Found"} + {AddedMixable?.Name ?? "NotFound"}";
    
    public static implicit operator ProductRecipeWrapper(ProductRecipe recipe) => new(recipe);
    public static implicit operator ProductRecipe(ProductRecipeWrapper recipeWrapper) => recipeWrapper._productRecipe;
}