using System;
using System.Collections.Generic;
using ReactiveUI;
using Schedule_I_Products_Management.Data;

namespace Schedule_I_Products_Management.Models;

public class BaseProductWrapper(BaseProduct baseProduct) : ReactiveObject, IProductWrapperShowData
{
    private BaseProduct _baseProduct = baseProduct;
    public bool IsMixed => false;
    public List<MixableWrapper> Mixables => [];

    public Guid Id
    {
        get => _baseProduct.Id;
        set
        {
            _baseProduct.Id = value;
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