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
    
    private readonly ObservableAsPropertyHelper<int> _cost;
    private ReadOnlyObservableCollection<MixableWrapper> _mixables;

    public MixedProductWrapper(MixedProduct mixedProduct)
    {
        _mixedProduct = mixedProduct;

        _cost = Observable.CombineLatest(
                MainWindow.ViewModel.Mixables.Connect()
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
        
        this.WhenAnyValue(x => x.MixablesIds)
            .Select(_ =>
                MainWindow.ViewModel.Mixables
                    .Connect()
                    .Filter(m => MixablesIds.Items.Contains(m.Id))
                    .Bind(out _mixables)
                    .AsObservableList())
            .Switch()
            .Subscribe();
    }

    public ProductCategory Category => BaseProduct.Category;
    public bool IsMixed => true;
    public int Cost => _cost.Value;
    public ReadOnlyObservableCollection<MixableWrapper> Mixables => _mixables;

    public Guid Id
    {
        get => _mixedProduct.Id;
        set
        {
            _mixedProduct.Id = value;
            this.RaisePropertyChanged();
        }
    }

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

    public SourceList<Guid> MixablesIds
    {
        get => _mixedProduct.MixablesIds;
        set
        {
            _mixedProduct.MixablesIds = value;
            this.RaisePropertyChanged();
        }
    }

    public static implicit operator MixedProductWrapper(MixedProduct mixedProduct) => new (mixedProduct);
    public static implicit operator MixedProduct(MixedProductWrapper mixedProductWrapper) => mixedProductWrapper._mixedProduct;

    public override string ToString()
    {
        return Name;
    }
}