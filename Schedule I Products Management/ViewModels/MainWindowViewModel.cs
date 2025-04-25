using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using AvaloniaGraphControl;
using DynamicData;
using DynamicData.Alias;
using ReactiveUI;
using Schedule_I_Products_Management.Models;

namespace Schedule_I_Products_Management.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
#pragma warning disable CA1822 // Mark members as static
    private SourceList<BaseProductWrapper> _baseProducts = new();
    private SourceList<MixedProductWrapper> _mixedProducts = new();
    private SourceList<MixableWrapper> _mixables = new();
    private SourceList<ProductEffectWrapper> _productEffects = new();
    private List<MixableWrapper> _missingMixables = new();
    private MixedProductWrapper? _edit_selectedMixedProduct;
    private Graph _missingGraph = new();
    private int _uniqueMixedPerBaseProduct;
    
    private Guid lastUsedGraphBaseId;
    private List<Guid> lastUsedGraphMixablesIds;
    private ProductNode lastUsedGraphStartNode;

    private ReadOnlyObservableCollection<BaseProductWrapper> _readOnlyBaseProducts;
    private ReadOnlyObservableCollection<MixedProductWrapper> _readOnlyMixedProducts;
    private ReadOnlyObservableCollection<MixableWrapper> _readOnlyMixables;
    private ReadOnlyObservableCollection<ProductEffectWrapper> _readOnlyProductEffects;
    private ReadOnlyObservableCollection<IProductWrapper> _readOnlyProducts;
    private ReadOnlyObservableCollection<ProductEffectWrapper> _editSelectedEffects;
    private ReadOnlyObservableCollection<ProductEffectWrapper> _editSelectedEffectsReverse;
    private ReadOnlyObservableCollection<IProductWrapper> _overviewFilteredProducts;
    private ReadOnlyObservableCollection<IProductWrapper> _selectedMixedMatchingProducts;
    private BaseProductWrapper _missingSelectedBaseProduct;

    public ref SourceList<BaseProductWrapper> BaseProducts => ref _baseProducts;
    public ref SourceList<MixedProductWrapper> MixedProducts => ref _mixedProducts;
    public ref SourceList<MixableWrapper> Mixables => ref _mixables;
    public ref SourceList<ProductEffectWrapper> ProductEffects => ref _productEffects;
    
    public ReadOnlyObservableCollection<BaseProductWrapper> BindableBaseProducts => _readOnlyBaseProducts;
    public ReadOnlyObservableCollection<MixedProductWrapper> BindableMixedProducts => _readOnlyMixedProducts;
    public ReadOnlyObservableCollection<MixableWrapper> BindableMixables => _readOnlyMixables;
    public ReadOnlyObservableCollection<ProductEffectWrapper> BindableProductEffects => _readOnlyProductEffects;
    public ReadOnlyObservableCollection<IProductWrapper> BindableProducts => _readOnlyProducts;
    public ReadOnlyObservableCollection<ProductEffectWrapper> EditSelectedMixedProductEffects => _editSelectedEffects;
    public ReadOnlyObservableCollection<ProductEffectWrapper> EditSelectedMixedProducteffectsReverse => _editSelectedEffectsReverse;
    public ReadOnlyObservableCollection<IProductWrapper> OverviewFilteredProducts => _overviewFilteredProducts;
    public ReadOnlyObservableCollection<IProductWrapper> EditSelectedMixedProductMatchingProducts => _selectedMixedMatchingProducts;

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
        
        // BindableProductEffects
        _productEffects.Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _readOnlyProductEffects)
            .Subscribe();
        
        // EditSelectedMixedProductEffects
        this.WhenAnyValue(x => x.EditSelectedMixedProduct)
            .Select(mpw =>
            {
                if (mpw == null) return Observable.Empty<IChangeSet<ProductEffectWrapper>>();

                return mpw.EffectIds.Connect()
                    .Transform(id => _productEffects.Items.FirstOrDefault(m => m.Id == id))
                    .Filter(m => m != null);
            })
            .Switch()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _editSelectedEffects)
            .Subscribe();

        // EditSelectedMixedProductEffectsReverse
        this.WhenAnyValue(x => x.EditSelectedMixedProduct)
            .Select(mpw =>
            {
                if (mpw == null)
                    return Observable.Empty<IChangeSet<ProductEffectWrapper>>();

                var trigger = mpw.EffectIds.Connect()
                    .ToCollection()
                    .Select(_ => Unit.Default);

                return _productEffects.Connect()
                    .AutoRefresh()
                    .AutoRefreshOnObservable(_ => trigger)
                    .Filter(m => !mpw.EffectIds.Items.Contains(m.Id));
            })
            .Switch()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _editSelectedEffectsReverse)
            .Subscribe();
        
        // EditSelectedMixedProductMatchingProducts
        this.WhenAnyValue(x => x.EditSelectedMixedProduct)
            .Select(mpw =>
            {
                if(mpw == null)
                    return Observable.Empty<IChangeSet<IProductWrapper>>();
                
                var trigger = mpw.RecipesSourceList.Connect()
                    .ToCollection()
                    .Select(_ => Unit.Default);
                
                return _mixedProducts.Connect()
                    .AutoRefresh()
                    .AutoRefreshOnObservable(_ => trigger)
                    .Where(m => m.Id != mpw.Id
                                && (m.BaseProduct?.Id ?? Guid.Empty) == (mpw.BaseProduct?.Id ?? Guid.Empty)
                                && !mpw.Recipes.Select(r => r.BaseProduct?.Id ?? Guid.Empty).Contains(m.Id))
                    .Transform(IProductWrapper (m) => m)
                    .MergeChangeSets(_baseProducts.Connect()
                        .Filter(bp => bp.Id == mpw.BaseProduct.Id)
                        .Transform(IProductWrapper (bp) => bp));
            })
            .Switch()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _selectedMixedMatchingProducts)
            .Subscribe();

        // OverviewFilteredProducts
        _baseProducts.Connect()
            .Transform(IProductWrapper (bp) => bp)
            .MergeChangeSets(_mixedProducts.Connect()
                .Transform(IProductWrapper (mp) => mp))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _overviewFilteredProducts)
            .Subscribe();
        
        // BindableProducts
        _baseProducts.Connect()
            .AutoRefresh()
            .Transform(IProductWrapper (bp) => bp)
            .MergeChangeSets(_mixedProducts.Connect()
                .AutoRefresh()
                .Transform(IProductWrapper (mp) => mp))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _readOnlyProducts)
            .Subscribe();
        
        // UniqueMixedPerBaseProduct
        _mixables.Connect()
            .Subscribe(_ => UniqueMixedPerBaseProduct = GetUniqueNodeCount());
        
        // MissingGraph
        /*
        _mixables.Connect()
            .AutoRefresh(x => x.Name)
            .Select(_ => Unit.Default)
            .Merge(_mixedProducts.Connect()
                .Select(_ => Unit.Default))
            .Merge(_mixedProducts.Connect()
                .Transform(m => m.MixablesIds.Connect()
                    .ToCollection()
                    .Select(_ => Unit.Default))
                .MergeMany(x => x))
            .Subscribe(_ => MissingGraph = GenerateGraph(lastUsedGraphBaseId, lastUsedGraphMixablesIds, lastUsedGraphStartNode));
            */
    }
    
    public MixedProductWrapper? EditSelectedMixedProduct
    {
        get => _edit_selectedMixedProduct;
        set => this.RaiseAndSetIfChanged(ref _edit_selectedMixedProduct, value);
    }

    public BaseProductWrapper MissingSelectedBaseProduct
    {
        get => _missingSelectedBaseProduct;
        set
        {
            this.RaiseAndSetIfChanged(ref _missingSelectedBaseProduct, value);
            MissingGraph = GenerateGraph(value);
        }
    }

    public List<MixableWrapper> MissingMixables
    {
        get => _missingMixables;
        set => this.RaiseAndSetIfChanged(ref _missingMixables, value);
    }

    public Graph MissingGraph
    {
        get => _missingGraph;
        set => this.RaiseAndSetIfChanged(ref _missingGraph, value);
    }

    public int UniqueMixedPerBaseProduct
    {
        get => _uniqueMixedPerBaseProduct;
        set => this.RaiseAndSetIfChanged(ref _uniqueMixedPerBaseProduct, value);
    }

    private Graph GenerateGraph(IProductWrapper? startProduct)
    {
        if (startProduct == null)
            return new Graph { Orientation = Graph.Orientations.Horizontal};
        
        var startIsMixed = startProduct is MixedProductWrapper;
        var baseProductId = startIsMixed
            ? ((MixedProductWrapper)startProduct).BaseProduct.Id
            : ((BaseProductWrapper)startProduct).Id;

        var startMixableIds = new List<Guid>(); /*startIsMixed
            ? ((MixedProductWrapper)startProduct).MixablesIds.Items.ToList()
            : [];*/

        return GenerateGraph(baseProductId, startMixableIds, new ProductNode(startProduct));
    }

    private Graph GenerateGraph(Guid baseProductId, List<Guid> startMixableIds, ProductNode startNode)
    {
        lastUsedGraphBaseId = baseProductId;
        lastUsedGraphMixablesIds = startMixableIds;
        lastUsedGraphStartNode = startNode;

        
        var graph = new Graph
        {
            Orientation = Graph.Orientations.Horizontal
        };
        
        if (baseProductId == Guid.Empty || startMixableIds == null || startNode == null)
            return graph;
        /*
        var foundRelatedMixed = _mixedProducts.Items
            .Where(x => x.BaseProduct.Id == baseProductId && startMixableIds.All(x.MixablesIds.Items.Contains))
            .Select(IProductWrapper (p) => p)
            .ToList();
        foundRelatedMixed.Add(_baseProducts.Items.First(p => p.Id == baseProductId));

        var stack = new Stack<(ProductNode node, List<Guid> mixables)>();
        stack.Push((startNode, startMixableIds));

        while (stack.Count > 0)
        {
            var (parent, parentMixables) = stack.Pop();

            // Limit total mixables (2 before + 2 after)
            if (parentMixables.Count >= startMixableIds.Count + 2 || parentMixables.Count <= startMixableIds.Count - 2)
                continue;

            foreach (var mixable in _mixables.Items)
            {
                //add
                if (parentMixables.Count >= startMixableIds.Count && !parentMixables.Contains(mixable.Id))
                {
                    var nextMixables = new List<Guid>(parentMixables) { mixable.Id };
                    
                    var mixedProduct = foundRelatedMixed.FirstOrDefault(
                        x => x.Mixables.Count == nextMixables.Count &&
                             x.Mixables.Select(m => m.Id).All(nextMixables.Contains)
                    );
                    var mixedNode = new ProductNode(mixedProduct);
                    mixedNode.OnClick = () => MissingGraph = GenerateGraph(baseProductId, nextMixables, mixedNode);

                    graph.Edges.Add(new Edge(parent, mixedNode, mixable.Name));
                    stack.Push((mixedNode, nextMixables));
                }
                
                //remove
                if (parentMixables.Count <= startMixableIds.Count && parentMixables.Contains(mixable.Id))
                {
                    var nextMixables = new List<Guid>(parentMixables);
                    nextMixables.Remove(mixable.Id);
                    
                    var mixedProduct = foundRelatedMixed.FirstOrDefault(
                        x => x.Mixables.Count == nextMixables.Count &&
                             x.Mixables.Select(m => m.Id).All(nextMixables.Contains)
                    );
                    var mixedNode = new ProductNode(mixedProduct);
                    mixedNode.OnClick = () => MissingGraph = GenerateGraph(baseProductId, nextMixables, mixedNode);

                    graph.Edges.Add(new Edge(mixedNode, parent, mixable.Name));
                    stack.Push((mixedNode, nextMixables));
                }
            }
        }

        MissingMixables = _mixables.Items.Where(m => startMixableIds.Contains(m.Id)).ToList();*/
        return graph;
    }
    
    public int GetUniqueNodeCount()
    {
        var foundUniqueNodes = new HashSet<string>();
        var stack = new Stack<List<Guid>>();
        stack.Push([]);

        while (stack.Count > 0)
        {
            var currentMixables = stack.Pop();
            // Create a unique, sorted string key for the combination
            var key = string.Join("|", currentMixables.OrderBy(g => g));
            if(foundUniqueNodes.Contains(key))
                continue;
            foundUniqueNodes.Add(key);

            // Add new combinations by adding each unused mixable
            foreach (var mixable in _mixables.Items)
            {
                if (currentMixables.Contains(mixable.Id)) continue;
                // Only create a new list when actually adding a mixable
                var next = new List<Guid>(currentMixables) { mixable.Id };
                stack.Push(next);
            }
        }

        return foundUniqueNodes.Count;
    }
    
#pragma warning restore CA1822 // Mark members as static
}