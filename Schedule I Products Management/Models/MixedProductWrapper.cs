using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;
using Schedule_I_Products_Management.Data;
using Schedule_I_Products_Management.Views;

namespace Schedule_I_Products_Management.Models;

public class MixedProductWrapper(MixedProduct mixedProduct) : ReactiveObject
{
    private MixedProduct _mixedProduct = mixedProduct;
    
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