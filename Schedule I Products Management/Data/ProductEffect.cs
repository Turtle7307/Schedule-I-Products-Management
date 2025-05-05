using System;
using System.Collections.Generic;

namespace Schedule_I_Products_Management.Data;

public class ProductEffect
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }

    public static readonly Dictionary<string, ProductEffect> DefaultTranslationTableEffects = new()
    {
        { "antigravity", new ProductEffect { Name = "Anti-Gravity"} },
        { "athletic", new ProductEffect { Name = "Athletic"} },
        { "balding", new ProductEffect { Name = "Balding"} },
        { "brighteyed", new ProductEffect { Name = "Bright-Eyed"} },
        { "calming", new ProductEffect { Name = "Calming"} },
        { "caloriedense", new ProductEffect { Name = "Calorie-Dense"} },
        { "cyclopean", new ProductEffect { Name = "Cyclopean"} },
        { "disorienting", new ProductEffect { Name = "Disorienting"} },
        { "electrifying", new ProductEffect { Name = "Electrifying"} },
        { "energizing", new ProductEffect { Name = "Energizing"} },
        { "Euphoric", new ProductEffect { Name = "Euphoric"} },
        { "explosive", new ProductEffect { Name = "Explosive"} },
        { "Focused", new ProductEffect { Name = "Focused"} },
        { "foggy", new ProductEffect { Name = "Foggy"} },
        { "gingeritis", new ProductEffect { Name = "Gingeritis"} },
        { "glowie", new ProductEffect { Name = "Glowing"} },
        { "jennerising", new ProductEffect { Name = "Jennerising"} },
        { "laxative", new ProductEffect { Name = "Laxative"} },
        { "lethal", new ProductEffect { Name = "Lethal"} },
        { "giraffying", new ProductEffect { Name = "Long Faced"} },
        { "munchies", new ProductEffect { Name = "Munchies"} },
        { "paranoia", new ProductEffect { Name = "Paranoia"} },
        { "refreshing", new ProductEffect { Name = "Refreshing"} },
        { "schizophrenic", new ProductEffect { Name = "Schizophrenic"} },
        { "sedating", new ProductEffect { Name = "Sedating"} },
        { "seizure", new ProductEffect { Name = "Seizure-Inducing"} },
        { "shrinking", new ProductEffect { Name = "Shrinking"} },
        { "slippery", new ProductEffect { Name = "Slippery"} },
        { "smelly", new ProductEffect { Name = "Smelly"} },
        { "sneaky", new ProductEffect { Name = "Sneaky"} },
        { "spicy", new ProductEffect { Name = "Spicy"} },
        { "thoughtprovoking", new ProductEffect { Name = "Thought-Provoking"} },
        { "toxic", new ProductEffect { Name = "Toxic"} },
        { "tropicthunder", new ProductEffect { Name = "Tropic Thunder"} },
        { "zombifying", new ProductEffect { Name = "Zombifying" }}
    };
}