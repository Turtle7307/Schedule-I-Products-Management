using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using Schedule_I_Products_Management.Data;
using Schedule_I_Products_Management.Views;

namespace Schedule_I_Products_Management.Models;

public class MixedProductWrapper : ReactiveObject, IProductWrapperShowData
{
    private MixedProduct _mixedProduct;
    private SourceList<Guid> _mixablesIds = new();
    private SourceList<Guid> _effectIds = new();
    
    private readonly ObservableAsPropertyHelper<int> _cost;
    private int _profit;
    private ReadOnlyObservableCollection<MixableWrapper> _mixables;
    private ReadOnlyObservableCollection<ProductEffectWrapper> _effects;
    
    public ref SourceList<Guid> EffectIds => ref _effectIds;
    public ref SourceList<Guid> MixablesIds => ref _mixablesIds;

    public MixedProductWrapper(MixedProduct mixedProduct)
    {
        _mixedProduct = mixedProduct;
        _mixablesIds.AddRange(mixedProduct.MixablesIds);
        _effectIds.AddRange(mixedProduct.EffectIds);

        // Cost
        _cost = Observable.CombineLatest(
                MainWindow.ViewModel.Mixables.Connect()
                    .AutoRefresh()
                    .ToCollection()
                    .StartWith(MainWindow.ViewModel.Mixables.Items.ToList()),
                MixablesIds.Connect()
                    .ToCollection()
                    .StartWith(MixablesIds.Items.ToList()),
                (mixables, ids) =>
                    BaseProduct.Cost + mixables
                        .Where(m => ids.Contains(m.Id))
                        .Sum(m => m.Cost)
            )
            .ToProperty(this, x => x.Cost, out _cost);
        
        // Mixables
        _mixablesIds.Connect()
            .Transform(m => MainWindow.ViewModel.Mixables.Items.First(mix => mix.Id == m))
            .Bind(out _mixables)
            .Subscribe();
        
        // Effects
        _effectIds.Connect()
            .Transform(m => MainWindow.ViewModel.ProductEffects.Items.First(eff => eff.Id == m))
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
    }

    public ProductCategory Category => BaseProduct.Category;
    public bool IsMixed => true;
    public int Cost => _cost.Value;
    public int Profit => _profit;
    public ReadOnlyObservableCollection<MixableWrapper> Mixables => _mixables;
    public ReadOnlyObservableCollection<ProductEffectWrapper> Effects => _effects;
    public Guid Id => _mixedProduct.Id;

    public BaseProductWrapper BaseProduct
    {
        get => MainWindow.ViewModel.BaseProducts.Items.First(mix => mix.Id == _mixedProduct.BaseProductId);
        set
        {
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
        mixedProductWrapper._mixedProduct.MixablesIds = mixedProductWrapper._mixablesIds.Items.ToList();
        mixedProductWrapper._mixedProduct.EffectIds = mixedProductWrapper._effectIds.Items.ToList();
        return mixedProductWrapper._mixedProduct;
    }

    public override string ToString()
    {
        return Name;
    }
}