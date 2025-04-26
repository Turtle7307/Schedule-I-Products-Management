using System;
using System.Collections.Generic;
using System.IO;
using Schedule_I_Products_Management.Data.Json;

namespace Schedule_I_Products_Management.Handlers;

public static class ReadSaveDataHandler
{
    private const string MainSaveFile = "Game.json";
    private const string ProductsDirectory = "Products";
    private const string ProductsFile = "Products.json";
    private const string MixedProductsDirectory = "CreatedProducts";
    
    public static void ReadSaveData(string path)
    {
        if (File.Exists(Path.Combine(path, MainSaveFile)))
            ReadSave(path);
        
        if (File.Exists(Path.Combine(path, "..", MainSaveFile)))
            ReadSave(Path.Combine(path, ".."));

        return;
        throw new ArgumentException("Path is not a valid save");
    }

    private static void ReadSave(string path)
    {
        var mainSaveData = JsonHandler.Read<Game>(Path.Combine(path, MainSaveFile));
        var productsData = JsonHandler.Read<Products>(Path.Combine(path, ProductsDirectory, ProductsFile));

        Dictionary<string, Product> products = new();
        if (Directory.Exists(Path.Combine(path, ProductsDirectory, MixedProductsDirectory)))
        {
            foreach (var file in Directory.GetFiles(Path.Combine(path, ProductsDirectory, MixedProductsDirectory)))
            {
                var product = JsonHandler.Read<Product>(file);
                if (product != null)
                    products.Add(product.ID, product);
            }
        }
    }
}