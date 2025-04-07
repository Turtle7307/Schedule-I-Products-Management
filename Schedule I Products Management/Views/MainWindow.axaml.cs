using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ReactiveUI;
using Schedule_I_Products_Management.Data;
using Schedule_I_Products_Management.Models;
using Schedule_I_Products_Management.ViewModels;

namespace Schedule_I_Products_Management.Views;

public partial class MainWindow : Window
{
    private const string DataZipFile = "data.zip";
    private const string BaseProductsJsonFile = "baseProducts.json";
    private const string MixablesJsonFile = "mixablesProducts.json";
    private const string MixedProductsJsonFile = "mixedProducts.json";
    
    public static MainWindowViewModel ViewModel => _viewModel;
    private static MainWindowViewModel _viewModel;
    
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        _viewModel = (MainWindowViewModel) DataContext;
        if (!File.Exists(DataZipFile)) return;
        
        _viewModel.BaseProducts = new ObservableCollection<BaseProductWrapper>(
            ZipHandler.ReadJsonFromZip<BaseProduct[]>(DataZipFile, BaseProductsJsonFile)!.Select(x =>
                new BaseProductWrapper(x)));
        _viewModel.Mixables = new ObservableCollection<MixableWrapper>(
            ZipHandler.ReadJsonFromZip<Mixable[]>(DataZipFile, MixablesJsonFile)!
                .Select(x => new MixableWrapper(x)));
        _viewModel.MixedProducts = new ObservableCollection<MixedProductWrapper>(
            ZipHandler.ReadJsonFromZip<MixedProduct[]>(DataZipFile, MixedProductsJsonFile)!
                .Select(x => new MixedProductWrapper(x)));
    }
    
    private void Button_save_OnClick(object? sender, RoutedEventArgs e)
    {
        ZipHandler.WriteJsonToZip(_viewModel.BaseProducts.Select(x => (BaseProduct)x).ToArray(), DataZipFile,
            BaseProductsJsonFile);
        ZipHandler.WriteJsonToZip(_viewModel.Mixables.Select(x => (Mixable)x).ToArray(), DataZipFile, MixablesJsonFile);
        ZipHandler.WriteJsonToZip(_viewModel.MixedProducts.Select(x => (MixedProduct)x).ToArray(), DataZipFile,
            MixedProductsJsonFile);
    }

    private void Button_edit_buyable_add_OnClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(textBox_edit_buyable_name.Text))
            return;
        _viewModel.BaseProducts.Add(new BaseProduct
        {
            Name = textBox_edit_buyable_name.Text,
            Cost = (int) numericUpDown_edit_buyable_cost.Value!,
            AskingPrice = (int) numericUpDown_edit_buyable_askingPrice.Value!,
            Addictiveness = (int) numericUpDown_edit_buyable_addictiveness.Value!,
            Category = (ProductCategory) comboBox_edit_buyable_category.SelectionBoxItem!
        });
    }

    private void Button_edit_buyable_delete_OnClick(object? sender, RoutedEventArgs e)
    {
        if (dataGrid_edit_buyable.SelectedItems.Count == 0)
            return;

        var selected = dataGrid_edit_buyable.SelectedItems.Cast<BaseProductWrapper>().ToList();

        _viewModel.BaseProducts = new ObservableCollection<BaseProductWrapper>(_viewModel.BaseProducts
            .Where(bp => selected.All(sbp => sbp.Id != bp.Id))
            .ToList());
    }

    private void Button_edit_mixable_add_OnClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(textBox_edit_mixable_name.Text))
            return;
        _viewModel.Mixables.Add(new Mixable
        {
            Name = textBox_edit_mixable_name.Text,
            Cost = (int) numericUpDown_edit_mixable_price.Value!
        });
        _viewModel.RaisePropertyChanged(nameof(_viewModel.EditSelectedMixedProductMixablesReverse));
    }

    private void Button_edit_mixable_delete_OnClick(object? sender, RoutedEventArgs e)
    {
        if (dataGrid_edit_mixable.SelectedItems.Count == 0)
            return;

        var selected = dataGrid_edit_mixable.SelectedItems.Cast<MixableWrapper>().ToList();

        _viewModel.Mixables = new ObservableCollection<MixableWrapper>(_viewModel.Mixables
            .Where(mix => selected.All(smix => smix.Id != mix.Id))
            .ToList());

        //clean all mixedProducts of the deleted mixables
        foreach (var mixedProduct in _viewModel.MixedProducts.Where(x =>
                     x.MixablesIds.Any(id => selected.Select(sx => sx.Id).Contains(id))))
            foreach (var wrapper in selected.Where(wrapper => mixedProduct.MixablesIds.Contains(wrapper.Id)))
                mixedProduct.MixablesIds.Remove(wrapper.Id);
        
        _viewModel.RaisePropertyChanged(nameof(_viewModel.EditSelectedMixedProductMixables));
        _viewModel.RaisePropertyChanged(nameof(_viewModel.EditSelectedMixedProductMixablesReverse));
    }

    private void Button_edit_mixed_add_OnClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(textBox_edit_mixed_name.Text) || comboBox_edit_mixed_baseProduct.SelectedItem is not BaseProductWrapper baseProduct)
            return;
        _viewModel.MixedProducts.Add(new MixedProduct
        {
            Name = textBox_edit_mixed_name.Text,
            AskingPrice = (int) numericUpDown_edit_mixed_askingPrice.Value!,
            Addictiveness = (int) numericUpDown_edit_mixed_addictiveness.Value!,
            BaseProductId = baseProduct.Id
        });
    }

    private void Button_edit_mixed_delete_OnClick(object? sender, RoutedEventArgs e)
    {
        if (dataGrid_edit_mixed.SelectedItems.Count == 0)
            return;

        var selected = dataGrid_edit_mixed.SelectedItems.Cast<MixedProductWrapper>().ToList();

        _viewModel.MixedProducts = new ObservableCollection<MixedProductWrapper>(_viewModel.MixedProducts
            .Where(mix => selected.All(smix => smix.Id != mix.Id))
            .ToList());
    }

    private void Button_edit_ingredients_add_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_viewModel.EditSelectedMixedProduct == null || comboBox_edit_ingredients_mixable.SelectionBoxItem is not MixableWrapper mixable)
            return;
        
        _viewModel.EditSelectedMixedProduct.MixablesIds.Add(mixable.Id);
        _viewModel.RaisePropertyChanged(nameof(_viewModel.EditSelectedMixedProductMixables));
        _viewModel.RaisePropertyChanged(nameof(_viewModel.EditSelectedMixedProductMixablesReverse));
    }

    private void Button_edit_ingredients_delete_OnClick(object? sender, RoutedEventArgs e)
    {
        if (dataGrid_edit_ingredients.SelectedItems.Count == 0 || _viewModel.EditSelectedMixedProduct == null)
            return;
        
        var selected = dataGrid_edit_mixed.SelectedItems.Cast<MixableWrapper>().ToList();
        
        _viewModel.EditSelectedMixedProduct.MixablesIds = _viewModel.EditSelectedMixedProduct.MixablesIds
            .Where(mix => selected.All(smix => smix.Id != mix))
            .ToList();
        _viewModel.RaisePropertyChanged(nameof(_viewModel.EditSelectedMixedProductMixables));
        _viewModel.RaisePropertyChanged(nameof(_viewModel.EditSelectedMixedProductMixablesReverse));
    }
}