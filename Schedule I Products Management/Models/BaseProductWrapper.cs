using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using ReactiveUI;
using Schedule_I_Products_Management.Data;
using Schedule_I_Products_Management.Views;

namespace Schedule_I_Products_Management.Models;

public class BaseProductWrapper : ReactiveObject, IProductWrapperShowData
{
    private BaseProduct _baseProduct;
    
    private readonly ObservableAsPropertyHelper<int> _profit;

    public BaseProductWrapper(BaseProduct baseProduct)
    {
        _baseProduct = baseProduct;
        
        _profit = this.WhenAnyValue(x => x.Cost, x => x.AskingPrice)
            .StartWith((Cost, AskingPrice))
            .Select(tuple => tuple.Item2 - tuple.Item1)
            .ToProperty(this, x => x.AskingPrice);
    }

    public bool IsMixed => false;
    public int Profit => _profit.Value;
    public ReadOnlyObservableCollection<MixableWrapper> Mixables => new([]);
    public Guid Id => _baseProduct.Id;

    public ProductEffectWrapper ProductEffect
    {
        get => MainWindow.ViewModel.ProductEffects.Items.FirstOrDefault(mix => mix.Id == _baseProduct.EffectId,
            new ProductEffectWrapper(new ProductEffect { Name = "None" }));
        set
        {
            _baseProduct.EffectId = value.Id;
            this.RaisePropertyChanged();
        }
    }
    
    public string Name
    {
        get => _baseProduct.Name;
        set
        {
            _baseProduct.Name = value;
            this.RaisePropertyChanged();
        }
    }
    
    public int Cost
    {
        get => _baseProduct.Cost;
        set
        {
            _baseProduct.Cost = value;
            this.RaisePropertyChanged();
        }
    }
    
    public int AskingPrice
    {
        get => _baseProduct.AskingPrice;
        set
        {
            _baseProduct.AskingPrice = value;
            this.RaisePropertyChanged();
        }
    }
    
    public int Addictiveness
    {
        get => _baseProduct.Addictiveness;
        set
        {
            _baseProduct.Addictiveness = value;
            this.RaisePropertyChanged();
        }
    }
    
    public ProductCategory Category
    {
        get => _baseProduct.Category;
        set
        {
            _baseProduct.Category = value;
            this.RaisePropertyChanged();
        }
    }
    
    public static implicit operator BaseProductWrapper(BaseProduct baseProduct) => new(baseProduct);
    public static implicit operator BaseProduct(BaseProductWrapper wrapper) => wrapper._baseProduct;

    public override string ToString()
    {
        return Name;
    }
}