using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using Schedule_I_Products_Management.Models;

namespace Schedule_I_Products_Management.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
#pragma warning disable CA1822 // Mark members as static
    private SourceList<BaseProductWrapper> _baseProducts = new();
    private SourceList<MixedProductWrapper> _mixedProducts = new();
    private SourceList<MixableWrapper> _mixables = new();
    private MixedProductWrapper? _edit_selectedMixedProduct;

    private ReadOnlyObservableCollection<BaseProductWrapper> _readOnlyBaseProducts;
    private ReadOnlyObservableCollection<MixedProductWrapper> _readOnlyMixedProducts;
    private ReadOnlyObservableCollection<MixableWrapper> _readOnlyMixables;
    private ReadOnlyObservableCollection<MixableWrapper> _editSelectedMixables;
    private ReadOnlyObservableCollection<MixableWrapper> _editSelectedMixablesReverse;
    private ReadOnlyObservableCollection<IProductWrapperShowData> _overviewFilteredProducts;
    
    public SourceList<BaseProductWrapper> BaseProducts => _baseProducts;
    public SourceList<MixedProductWrapper> MixedProducts => _mixedProducts;
    public SourceList<MixableWrapper> Mixables => _mixables;
    
    public ReadOnlyObservableCollection<BaseProductWrapper> BindableBaseProducts => _readOnlyBaseProducts;
    public ReadOnlyObservableCollection<MixedProductWrapper> BindableMixedProducts => _readOnlyMixedProducts;
    public ReadOnlyObservableCollection<MixableWrapper> BindableMixables => _readOnlyMixables;
    public ReadOnlyObservableCollection<MixableWrapper> EditSelectedMixedProductMixables => _editSelectedMixables;
    public ReadOnlyObservableCollection<MixableWrapper> EditSelectedMixedProductMixablesReverse => _editSelectedMixablesReverse;
    public ReadOnlyObservableCollection<IProductWrapperShowData> OverviewFilteredProducts => _overviewFilteredProducts;

    public MainWindowViewModel()
    {
        // BindableBaseProducts
        _baseProducts.Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _readOnlyBaseProducts)
            .Subscribe();
        
        // BindableMixables
        _mixables.Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _readOnlyMixables)
            .Subscribe();
        
        // BindableMixedProducts
        _mixedProducts.Connect()
            .AutoRefreshOnObservable(mpw =>
                mpw.WhenAnyValue(x => x.BaseProduct)
                    .SelectMany(bp =>
                        bp.WhenAnyValue(x => x.Name, x => x.Category)
                    )
            )
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _readOnlyMixedProducts)
            .Subscribe();

        // EditSelectedMixedProductMixables
        this.WhenAnyValue(x => x.EditSelectedMixedProduct)
            .Select(mpw =>
            {
                if (mpw == null) return Observable.Empty<IChangeSet<MixableWrapper>>();

                return mpw.MixablesIds.Connect()
                    .Transform(id => _mixables.Items.FirstOrDefault(m => m.Id == id))
                    .Filter(m => m != null);
            })
            .Switch()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _editSelectedMixables)
            .Subscribe();

        // EditSelectedMixedProductMixablesReverse
        this.WhenAnyValue(x => x.EditSelectedMixedProduct)
            .Select(mpw =>
            {
                if (mpw == null)
                    return Observable.Empty<IChangeSet<MixableWrapper>>();

                var trigger = mpw.MixablesIds.Connect()
                    .ToCollection()
                    .Select(_ => Unit.Default);

                return _mixables.Connect()
                    .AutoRefresh()
                    .AutoRefreshOnObservable(_ => trigger)
                    .Filter(m => !mpw.MixablesIds.Items.Contains(m.Id));
            })
            .Switch()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _editSelectedMixablesReverse)
            .Subscribe();

        // OverviewFilteredProducts
        _baseProducts.Connect()
            .AutoRefresh()
            .Transform(bp => (IProductWrapperShowData)bp)
            .Merge(_mixedProducts.Connect()
                .AutoRefresh()
                .Transform(mp => (IProductWrapperShowData)mp))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _overviewFilteredProducts)
            .Subscribe();
    }

    public MixedProductWrapper? EditSelectedMixedProduct
    {
        get => _edit_selectedMixedProduct;
        set => this.RaiseAndSetIfChanged(ref _edit_selectedMixedProduct, value);
    }
    
#pragma warning restore CA1822 // Mark members as static
}