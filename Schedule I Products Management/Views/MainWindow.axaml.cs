using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DynamicData;
using Schedule_I_Products_Management.Data;
using Schedule_I_Products_Management.Handlers;
using Schedule_I_Products_Management.Models;
using Schedule_I_Products_Management.ViewModels;

namespace Schedule_I_Products_Management.Views;

public partial class MainWindow : Window
{
    public static ref MainWindowViewModel ViewModel => ref _viewModel;
    private static MainWindowViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        _viewModel = (MainWindowViewModel) DataContext;
        DataHandler.ReadData(ref _viewModel);
    }
    
    private void Button_save_OnClick(object? sender, RoutedEventArgs e)
    {
        DataHandler.WriteData(ref _viewModel);
    }

    private void Button_edit_buyable_add_OnClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(textBox_edit_buyable_name.Text) ||
            autoCompleteBox_edit_buyable_effect.SelectedItem is not ProductEffectWrapper)
            return;
        ViewModel.BaseProducts.Add(new BaseProduct
        {
            Name = textBox_edit_buyable_name.Text,
            Cost = (int) numericUpDown_edit_buyable_cost.Value!,
            AskingPrice = (int) numericUpDown_edit_buyable_askingPrice.Value!,
            Addictiveness = (int) numericUpDown_edit_buyable_addictiveness.Value!,
            EffectId = ((ProductEffectWrapper) autoCompleteBox_edit_buyable_effect.SelectedItem!).Id,
            Category = (ProductCategory) comboBox_edit_buyable_category.SelectedItem!
        });
    }

    private void Button_edit_buyable_delete_OnClick(object? sender, RoutedEventArgs e)
    {
        if (dataGrid_edit_buyable.SelectedItems.Count == 0)
            return;

        var selected = dataGrid_edit_buyable.SelectedItems.Cast<BaseProductWrapper>().ToList();

        ViewModel.MixedProducts.Edit(list => list.RemoveMany(ViewModel.MixedProducts.Items
            .Where(mp => selected.Select(x => x.Id).Contains(mp.BaseProduct?.Id ?? Guid.Empty))));
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
        ViewModel.Mixables.Edit(list => list.RemoveMany(selected));
    }

    private void Button_edit_effect_add_OnClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(textBox_edit_effect_name.Text))
            return;
        
        ViewModel.ProductEffects.Add(new ProductEffect
        {
            Name = textBox_edit_effect_name.Text
        });
    }
    
    private void Button_edit_effect_delete_OnClick(object? sender, RoutedEventArgs e)
    {
        if (dataGrid_edit_effect.SelectedItems.Count == 0)
            return;

        var selected = dataGrid_edit_effect.SelectedItems.Cast<ProductEffectWrapper>().ToList();
        
        //clean all mixedProducts of the deleted effects
        foreach (var mixedProduct in ViewModel.MixedProducts.Items.Where(x =>
                     x.EffectIds.Items.Any(id => selected.Select(sx => sx.Id).Contains(id))))
            foreach (var wrapper in selected.Where(wrapper => mixedProduct.EffectIds.Items.Contains(wrapper.Id)))
                mixedProduct.EffectIds.Remove(wrapper.Id);
        
        ViewModel.ProductEffects.Edit(list => list.RemoveMany(selected));
    }

    private void Button_edit_mixed_add_OnClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(textBox_edit_mixed_name.Text) ||
            comboBox_edit_mixed_baseProduct.SelectedItem == null)
            return;

        var baseProduct = (BaseProductWrapper) comboBox_edit_mixed_baseProduct.SelectedItem;
        var newProduct = new MixedProduct
        {
            Name = textBox_edit_mixed_name.Text,
            AskingPrice = (int)numericUpDown_edit_mixed_askingPrice.Value!,
            Addictiveness = (int)numericUpDown_edit_mixed_addictiveness.Value!,
            BaseProductId = baseProduct.Id
        };
        newProduct.EffectIds.AddRange(baseProduct.Effects.Select(x => x.Id));
        ViewModel.MixedProducts.Add(newProduct);
    }

    private void Button_edit_mixed_delete_OnClick(object? sender, RoutedEventArgs e)
    {
        if (dataGrid_edit_mixed.SelectedItems.Count == 0)
            return;

        var selected = dataGrid_edit_mixed.SelectedItems.Cast<MixedProductWrapper>().ToList();

        ViewModel.MixedProducts.Edit(list => list.RemoveMany(selected));
    }

    private void Button_edit_mixed_recipe_add_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.EditSelectedMixedProduct == null ||
            autoCompleteBox_edit_mixed_recipe_product.SelectedItem is not IProductWrapper productWrapper ||
            autoCompleteBox_edit_mixed_recipe_mixable.SelectedItem is not MixableWrapper mixableWrapper)
            return;
        
        ViewModel.EditSelectedMixedProduct.RecipesSourceList.Add(new ProductRecipe
        {
            BaseProductId = productWrapper.Id,
            MixableId = mixableWrapper.Id
        });
    }

    private void Button_edit_mixed_recipe_delete_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.EditSelectedMixedProduct == null || dataGrid_edit_recipes.SelectedItems.Count == 0)
            return;

        var selected = dataGrid_edit_recipes.SelectedItems.Cast<ProductRecipeWrapper>().ToList();

        ViewModel.EditSelectedMixedProduct.RecipesSourceList.Edit(list => list.RemoveMany(selected));
    }

    private void Button_edit_mixed_effect_add_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.EditSelectedMixedProduct == null || autoCompleteBox_edit_mixed_effects.SelectedItem is not ProductEffectWrapper effect)
            return;
        
        ViewModel.EditSelectedMixedProduct.EffectIds.Add(effect.Id);
    }
    
    private void Button_edit_mixed_effect_delete_OnClick(object? sender, RoutedEventArgs e)
    {
        if (dataGrid_edit_mixedEffects.SelectedItems.Count == 0 || ViewModel.EditSelectedMixedProduct == null)
            return;
        
        var selected = dataGrid_edit_mixedEffects.SelectedItems.Cast<ProductEffectWrapper>().ToList();
        
        ViewModel.EditSelectedMixedProduct.EffectIds.Edit(list => list.RemoveMany(selected.Select(m => m.Id)));
    }
}