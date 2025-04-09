using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DynamicData;
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
    
    public static MainWindowViewModel ViewModel { get; private set; }

    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        ViewModel = (MainWindowViewModel) DataContext;
        if (!File.Exists(DataZipFile)) return;

        ViewModel.BaseProducts.Edit(list =>
            list.AddRange(
                ZipHandler.ReadJsonFromZip<BaseProduct[]>(DataZipFile, BaseProductsJsonFile)!.
                    Select(x => new BaseProductWrapper(x))));
        ViewModel.Mixables.Edit(list =>
            list.AddRange(
                ZipHandler.ReadJsonFromZip<Mixable[]>(DataZipFile, MixablesJsonFile)!
                    .Select(x => new MixableWrapper(x))));
        ViewModel.MixedProducts.Edit(list =>
            list.AddRange(
                ZipHandler.ReadJsonFromZip<MixedProduct[]>(DataZipFile, MixedProductsJsonFile)!
                    .Select(x => new MixedProductWrapper(x))));
    }
    
    private void Button_save_OnClick(object? sender, RoutedEventArgs e)
    {
        ZipHandler.WriteJsonToZip(ViewModel.BaseProducts.Items.Select(BaseProduct (x) => x).ToArray(), DataZipFile,
            BaseProductsJsonFile);
        ZipHandler.WriteJsonToZip(ViewModel.Mixables.Items.Select(Mixable (x) => x).ToArray(), DataZipFile,
            MixablesJsonFile);
        ZipHandler.WriteJsonToZip(ViewModel.MixedProducts.Items.Select(MixedProduct (x) => x).ToArray(), DataZipFile,
            MixedProductsJsonFile);
    }

    private void Button_edit_buyable_add_OnClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(textBox_edit_buyable_name.Text))
            return;
        ViewModel.BaseProducts.Add(new BaseProduct
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

        ViewModel.MixedProducts.Edit(list => list.RemoveMany(ViewModel.MixedProducts.Items
            .Where(mp => selected.Select(x => x.Id).Contains(mp.BaseProduct.Id))));
        ViewModel.BaseProducts.Edit(list => list.RemoveMany(selected));
    }

    private void Button_edit_mixable_add_OnClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(textBox_edit_mixable_name.Text))
            return;
        ViewModel.Mixables.Add(new Mixable
        {
            Name = textBox_edit_mixable_name.Text,
            Cost = (int) numericUpDown_edit_mixable_price.Value!
        });
    }

    private void Button_edit_mixable_delete_OnClick(object? sender, RoutedEventArgs e)
    {
        if (dataGrid_edit_mixable.SelectedItems.Count == 0)
            return;

        var selected = dataGrid_edit_mixable.SelectedItems.Cast<MixableWrapper>().ToList();

        //clean all mixedProducts of the deleted mixables
        foreach (var mixedProduct in ViewModel.MixedProducts.Items.Where(x =>
                     x.MixablesIds.Items.Any(id => selected.Select(sx => sx.Id).Contains(id))))
            foreach (var wrapper in selected.Where(wrapper => mixedProduct.MixablesIds.Items.Contains(wrapper.Id)))
                mixedProduct.MixablesIds.Remove(wrapper.Id);
        
        ViewModel.Mixables.Edit(list => list.RemoveMany(selected));
    }

    private void Button_edit_mixed_add_OnClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(textBox_edit_mixed_name.Text) || comboBox_edit_mixed_baseProduct.SelectedItem is not BaseProductWrapper baseProduct)
            return;
        ViewModel.MixedProducts.Add(new MixedProduct
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

        ViewModel.MixedProducts.Edit(list => list.RemoveMany(selected));
    }

    private void Button_edit_ingredients_add_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.EditSelectedMixedProduct == null || comboBox_edit_ingredients_mixable.SelectionBoxItem is not MixableWrapper mixable)
            return;
        
        ViewModel.EditSelectedMixedProduct.MixablesIds.Add(mixable.Id);
    }

    private void Button_edit_ingredients_delete_OnClick(object? sender, RoutedEventArgs e)
    {
        if (dataGrid_edit_ingredients.SelectedItems.Count == 0 || ViewModel.EditSelectedMixedProduct == null)
            return;
        
        var selected = dataGrid_edit_ingredients.SelectedItems.Cast<MixableWrapper>().ToList();
        
        ViewModel.EditSelectedMixedProduct.MixablesIds.Edit(list => list.RemoveMany(selected.Select(m => m.Id)));
    }
}