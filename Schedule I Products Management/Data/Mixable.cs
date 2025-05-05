using System;
using System.Collections.Generic;

namespace Schedule_I_Products_Management.Data;

public class Mixable
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public int Cost { get; set; }

    public static readonly Dictionary<string, Mixable> DefaultTranslationTableMixables = new()
    {
        { "cuke", new() { Name = "Cuke", Cost = 2} },
        { "banana", new() { Name = "Banana", Cost = 2} },
        { "paracetamol", new() { Name = "Paracetamol", Cost = 3} },
        { "donut", new() { Name = "Donut", Cost = 3} },
        { "viagra", new() { Name = "Viagra", Cost = 4} },
        { "mouthwash", new() { Name = "Mouth Wash", Cost = 4} },
        { "flumedicine", new() { Name = "Flu Medicine", Cost = 5} },
        { "gasoline", new() { Name = "Gasoline", Cost = 5} },
        { "energydrink", new() { Name = "Energy Drink", Cost = 6} },
        { "motoroil", new() { Name = "Motor Oil", Cost = 6} }, //TODO: check if this is correct
        { "megabean", new() { Name = "Mega Bean", Cost = 7} }, //TODO: check if this is correct
        { "chili", new() { Name = "Chili", Cost = 7} }, //TODO: check if this is correct
        { "battery", new() { Name = "Battery", Cost = 8} }, //TODO: check if this is correct
        { "iodine", new() { Name = "Iodine", Cost = 8} }, //TODO: check if this is correct
        { "addy", new() { Name = "Addy", Cost = 9} },
        { "horsesemen", new() { Name = "Horse Semen", Cost = 9 }}
    };
}