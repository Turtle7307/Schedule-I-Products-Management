using System.Collections.ObjectModel;
using ReactiveUI;
using Schedule_I_Products_Management.Data;

namespace Schedule_I_Products_Management.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
#pragma warning disable CA1822 // Mark members as static
    private ObservableCollection<BaseProduct> _baseProducts = new();
    private ObservableCollection<MixedProduct> _mixedProducts = new();
    private ObservableCollection<Mixable> _mixables = new();
    
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
    
#pragma warning restore CA1822 // Mark members as static
}