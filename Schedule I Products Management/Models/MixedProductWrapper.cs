using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.Data;
using DynamicData;
using DynamicData.Aggregation;
using ReactiveUI;
using Schedule_I_Products_Management.Data;
using Schedule_I_Products_Management.Views;

namespace Schedule_I_Products_Management.Models;

public class MixedProductWrapper : ReactiveObject, IProductWrapper
{
    private MixedProduct _mixedProduct;
    private SourceList<Guid> _effectIds = new();
    private SourceList<ProductRecipeWrapper> _recipes = new();

    private int _cost;
    private int _profit;
    private ReadOnlyObservableCollection<ProductEffectWrapper> _effects;
    private ReadOnlyObservableCollection<ProductRecipeWrapper> _bindableRecipes;
    
    public ref SourceList<Guid> EffectIds => ref _effectIds;
    public ref SourceList<ProductRecipeWrapper> RecipesSourceList => ref _recipes;

    public MixedProductWrapper(MixedProduct mixedProduct)
    {
        _mixedProduct = mixedProduct;
        _effectIds.AddRange(mixedProduct.EffectIds);
        _recipes.AddRange(mixedProduct.ProductRecipes.Select(ProductRecipeWrapper (x) => x));

        // Cost
        _recipes.Connect()
            .AutoRefresh()
            .Minimum(x => x.BaseProduct?.Cost ?? 0 + x.AddedMixable?.Cost ?? 0)
            .Subscribe(cost =>
            {
                _cost = cost;
                this.RaisePropertyChanged(nameof(Cost));
            });
        
        // Effects
        _effectIds.Connect()
            .Transform(m => MainWindow.ViewModel.ProductEffects.Items.FirstOrDefault(eff => eff.Id == m,
                (ProductEffectWrapper) new ProductEffect{Name = "Not Found"}))
            .Bind(out _effects)
            .Subscribe();
        
        // Profit
        this.WhenAnyValue(x => x.Cost, x => x.AskingPrice)
            .StartWith((Cost, AskingPrice))
            .Select(tuple => tuple.Item2 - tuple.Item1)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(i =>
            {
                _profit = i;
                this.RaisePropertyChanged(nameof(Profit));
            });
        
        // Recipes
        _recipes.Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _bindableRecipes)
            .Subscribe();
    }

    public ProductCategory Category => BaseProduct.Category;
    public bool IsMixed => true;
    public int Cost => _cost;
    public int Profit => _profit;
    public ReadOnlyObservableCollection<ProductEffectWrapper> Effects => _effects;
    public ReadOnlyObservableCollection<ProductRecipeWrapper> Recipes => _bindableRecipes;
    public Guid Id => _mixedProduct.Id;

    public BaseProductWrapper? BaseProduct
    {
        get => MainWindow.ViewModel.BaseProducts.Items.FirstOrDefault(bas => bas.Id == _mixedProduct.BaseProductId);
        set
        {
            if (value == null)
                throw new DataValidationException("can not be null");
            _mixedProduct.BaseProductId = value.Id;
            this.RaisePropertyChanged();
        }
    }

    public string Name
    {
        get => _mixedProduct.Name;
        set
        {
            _mixedProduct.Name = value;
            this.RaisePropertyChanged();
        }
    }

    public int AskingPrice
    {
        get => _mixedProduct.AskingPrice;
        set
        {
            _mixedProduct.AskingPrice = value;
            this.RaisePropertyChanged();
        }
    }

    public int Addictiveness
    {
        get => _mixedProduct.Addictiveness;
        set
        {
            _mixedProduct.Addictiveness = value;
            this.RaisePropertyChanged();
        }
    }

    public static implicit operator MixedProductWrapper(MixedProduct mixedProduct) => new (mixedProduct);
    public static implicit operator MixedProduct(MixedProductWrapper mixedProductWrapper)
    {
        mixedProductWrapper._mixedProduct.EffectIds = mixedProductWrapper._effectIds.Items.ToList();
        mixedProductWrapper._mixedProduct.ProductRecipes =
            mixedProductWrapper._recipes.Items.Select(ProductRecipe (x) => x).ToList();
        return mixedProductWrapper._mixedProduct;
    }

    public override string ToString()
    {
        return Name;
    }
}