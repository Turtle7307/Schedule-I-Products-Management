using System.IO;
using System.Linq;
using Schedule_I_Products_Management.Data;
using Schedule_I_Products_Management.Models;
using Schedule_I_Products_Management.ViewModels;

namespace Schedule_I_Products_Management.Handlers;

public static class DataHandler
{
    private const string DataZipFile = "data.zip";
    private const string BaseProductsJsonFile = "baseProducts.json";
    private const string MixedProductsJsonFile = "mixedProducts.json";
    private const string MixablesJsonFile = "mixables.json";
    private const string ProductEffectsJsonFile = "effects.json";
    private const string SavesJsonFile = "saves.json";

    public static void ReadData(ref MainWindowViewModel viewModel)
    {
        if (!File.Exists(DataZipFile))
        {
            viewModel.ProductEffects.Edit(list =>
                list.AddRange(
                    ProductEffect.DefaultTranslationTableEffects.Values.Select(x => new ProductEffectWrapper(x))));
            viewModel.Mixables.Edit(list =>
                list.AddRange(Mixable.DefaultTranslationTableMixables.Values.Select(x => new MixableWrapper(x))));
            return;
        }
        
        viewModel.ProductEffects.Edit(list =>
            list.AddRange(
                ZipHandler.ReadJsonFromZip<ProductEffect[]>(DataZipFile, ProductEffectsJsonFile)!
                    .Select(x => new ProductEffectWrapper(x))));
        viewModel.Mixables.Edit(list =>
            list.AddRange(
                ZipHandler.ReadJsonFromZip<Mixable[]>(DataZipFile, MixablesJsonFile)!
                    .Select(x => new MixableWrapper(x))));
        viewModel.BaseProducts.Edit(list =>
            list.AddRange(
                ZipHandler.ReadJsonFromZip<BaseProduct[]>(DataZipFile, BaseProductsJsonFile)!.
                    Select(x => new BaseProductWrapper(x))));
        viewModel.MixedProducts.Edit(list =>
            list.AddRange(
                ZipHandler.ReadJsonFromZip<MixedProduct[]>(DataZipFile, MixedProductsJsonFile)!
                    .Select(x => new MixedProductWrapper(x))));
    }

    public static void WriteData(ref MainWindowViewModel viewModel)
    {
        ZipHandler.WriteJsonToZip(viewModel.ProductEffects.Items.Select(ProductEffect (x) => x).ToArray(), DataZipFile,
            ProductEffectsJsonFile);
        ZipHandler.WriteJsonToZip(viewModel.Mixables.Items.Select(Mixable (x) => x).ToArray(), DataZipFile,
            MixablesJsonFile);
        ZipHandler.WriteJsonToZip(viewModel.BaseProducts.Items.Select(BaseProduct (x) => x).ToArray(), DataZipFile,
            BaseProductsJsonFile);
        ZipHandler.WriteJsonToZip(viewModel.MixedProducts.Items.Select(MixedProduct (x) => x).ToArray(), DataZipFile,
            MixedProductsJsonFile);
    }
}