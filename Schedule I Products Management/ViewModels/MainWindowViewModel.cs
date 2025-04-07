using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using Schedule_I_Products_Management.Data;

namespace Schedule_I_Products_Management.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
#pragma warning disable CA1822 // Mark members as static
    private ObservableCollection<BaseProduct> _baseProducts = new();
    private ObservableCollection<MixedProduct> _mixedProducts = new();
    private ObservableCollection<Mixable> _mixables = new();
    private MixedProduct? _edit_selectedMixedProduct;
    
    public ObservableCollection<BaseProduct> BaseProducts
    {
        get => _baseProducts;
        set
        {
            this.RaiseAndSetIfChanged(ref _baseProducts, value);
        }
    }
    
    public ObservableCollection<MixedProduct> MixedProducts
    {
        get => _mixedProducts;
        set
        {
            this.RaiseAndSetIfChanged(ref _mixedProducts, value);
        }
    }

    public ObservableCollection<Mixable> Mixables
    {
        get => _mixables;
        set
        {
            this.RaiseAndSetIfChanged(ref _mixables, value);
        }
    }

    public MixedProduct? EditSelectedMixedProduct
    {
        get => _edit_selectedMixedProduct;
        set
        {
            this.RaiseAndSetIfChanged(ref _edit_selectedMixedProduct, value);
            this.RaisePropertyChanged(nameof(EditSelectedMixedProductMixables));
        }
    }

    public List<Mixable> EditSelectedMixedProductMixables
    {
        get => _edit_selectedMixedProduct.MixablesIds
            .Select(id => _mixables.FirstOrDefault(m => m.Id == id))
            .Where(m => m != null)
            .ToList();
        set
        {
            _edit_selectedMixedProduct.MixablesIds = value.Select(m => m.Id).ToList();
        }
    }

#pragma warning restore CA1822 // Mark members as static
}