using System.IO;
using Newtonsoft.Json;

namespace Schedule_I_Products_Management.Handlers;

public static class JsonHandler
{
    public static void Write(string path, object serialize)
    {
        var sw = new StreamWriter(path);
        Write(sw, serialize);
        sw.Close();
    }

    public static void Write(StreamWriter sw, object serialize)
    {
        var serializer = new JsonSerializer
        {
            Formatting = Formatting.Indented
        };
        serializer.Serialize(new JsonTextWriter(sw), serialize);
        sw.Flush();
    }

    public static T? Read<T>(string path)
    {
        var sr = new StreamReader(path);
        var obj = Read<T>(sr);
        sr.Close();
        return obj;
    }

    public static T? Read<T>(StreamReader sr)
    {
        var serializer = new JsonSerializer();
        var obj = serializer.Deserialize<T>(new JsonTextReader(sr));
        return obj;
    }
}