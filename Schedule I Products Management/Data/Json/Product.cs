using System.Collections.Generic;

namespace Schedule_I_Products_Management.Data.Json;

public class Product
{
    public string GameVersion { get; set; }
    public string Name { get; set; }
    public string ID { get; set; }
    public List<string> Properties { get; set; }
}