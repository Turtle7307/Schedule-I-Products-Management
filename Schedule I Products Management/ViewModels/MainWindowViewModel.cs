using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using Schedule_I_Products_Management.Models;

namespace Schedule_I_Products_Management.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
#pragma warning disable CA1822 // Mark members as static
    private ObservableCollection<BaseProductWrapper> _baseProducts = new();
    private ObservableCollection<MixedProductWrapper> _mixedProducts = new();
    private ObservableCollection<MixableWrapper> _mixables = new();
    private MixedProductWrapper? _edit_selectedMixedProduct;
    
    public ObservableCollection<BaseProductWrapper> BaseProducts
    {
        get => _baseProducts;
        set
        {
            this.RaiseAndSetIfChanged(ref _baseProducts, value);
            this.RaisePropertyChanged(nameof(OverviewFilteredProducts));
        }
    }
    
    public ObservableCollection<MixedProductWrapper> MixedProducts
    {
        get => _mixedProducts;
        set
        {
            this.RaiseAndSetIfChanged(ref _mixedProducts, value);
            this.RaisePropertyChanged(nameof(OverviewFilteredProducts));
        }
    }

    public ObservableCollection<MixableWrapper> Mixables
    {
        get => _mixables;
        set
        {
            this.RaiseAndSetIfChanged(ref _mixables, value);
            this.RaisePropertyChanged(nameof(EditSelectedMixedProductMixablesReverse));
        }
    }

    public MixedProductWrapper? EditSelectedMixedProduct
    {
        get => _edit_selectedMixedProduct;
        set
        {
            this.RaiseAndSetIfChanged(ref _edit_selectedMixedProduct, value);
            this.RaisePropertyChanged(nameof(EditSelectedMixedProductMixables));
            this.RaisePropertyChanged(nameof(EditSelectedMixedProductMixablesReverse));
        }
    }

    public List<MixableWrapper> EditSelectedMixedProductMixables
    {
        get => _edit_selectedMixedProduct?.MixablesIds
            .Select(id => _mixables.FirstOrDefault(m => m.Id == id))
            .Where(m => m != null)
            .ToList()!;
        set
        {
            if (_edit_selectedMixedProduct == null)
                return;
            _edit_selectedMixedProduct.MixablesIds = value.Select(m => m.Id).ToList();
        }
    }
    
    public List<MixableWrapper> EditSelectedMixedProductMixablesReverse
    {
        get => _mixables.Where(mix => !(_edit_selectedMixedProduct?.MixablesIds.Any(m => m == mix.Id) ?? false)).ToList();
    }

    public List<IProductWrapperShowData> OverviewFilteredProducts
    {
        get
        {
            var products = _baseProducts.Select(IProductWrapperShowData (bp) => bp).ToList();
            products.AddRange(_mixedProducts.Select(IProductWrapperShowData (bp) => bp).ToList());
            return products;
        }
    }

#pragma warning restore CA1822 // Mark members as static
}