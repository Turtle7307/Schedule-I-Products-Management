using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;
using Schedule_I_Products_Management.Data;
using Schedule_I_Products_Management.Views;

namespace Schedule_I_Products_Management.Models;

public class MixedProductWrapper : ReactiveObject, IProductWrapperShowData
{
    private MixedProduct _mixedProduct;

    public MixedProductWrapper(MixedProduct mixedProduct)
    {
        _mixedProduct = mixedProduct;
        foreach(var mixable in MainWindow.ViewModel.Mixables)
            mixable.WhenAnyValue(x => x.Cost)
                .ToProperty(this, x => x.Cost);
    }

    public ProductCategory Category => BaseProduct.Category;
    public bool IsMixed => true;
    public int Cost => BaseProduct.Cost + MainWindow.ViewModel.Mixables
                           .Where(m => MixablesIds.Contains(m.Id))
                           .Sum(m => m.Cost);
    public List<MixableWrapper> Mixables =>
        MainWindow.ViewModel.Mixables.Where(m => MixablesIds.Contains(m.Id)).ToList();

    public Guid Id
    {
        get => _mixedProduct.Id;
        set
        {
            _mixedProduct.Id = value;
            this.RaisePropertyChanged();
        }
    }

    public BaseProduct BaseProduct
    {
        get => MainWindow.ViewModel.BaseProducts.First(mix => mix.Id == _mixedProduct.BaseProductId);
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

    public List<Guid> MixablesIds
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