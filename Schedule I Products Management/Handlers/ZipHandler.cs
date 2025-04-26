using System;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;

namespace Schedule_I_Products_Management.Handlers;

public static class ZipHandler
{
    public static void WriteJsonToZip(object serialize, string zipPath, string internalPath, bool addRootFolderToPath = false, bool overrideEntry = true)
    {
        var zip = new ZipArchive(new FileStream(zipPath, FileMode.OpenOrCreate, FileAccess.ReadWrite), ZipArchiveMode.Update);
        var rootFolder = addRootFolderToPath ? zip.Entries[0].FullName.Split("/")[0] : "";

        var existingEntry = zip.GetEntry(rootFolder + internalPath);
        if(existingEntry != null)
        {
            if(overrideEntry)
            {
                existingEntry.Delete();
            }
            else
            {
                throw new Exception("Can not Create entry as a entry already Exists!");
            }
        }

        var entry = zip.CreateEntry(rootFolder + internalPath);
        var sw = new StreamWriter(entry.Open());
        var serializer = new JsonSerializer
        {
            Formatting = Formatting.Indented
        };
        serializer.Serialize(new JsonTextWriter(sw), serialize);
        sw.Flush();
        sw.Close();
        zip.Dispose();
    }
    
    public static T? ReadJsonFromZip<T>(string zipPath, string internalPath, bool addRootFolderToPath = false)
    {
        var file = File.OpenRead(zipPath);
        var zip = new ZipArchive(file);
        var rootFolder = addRootFolderToPath ? zip.Entries[0].FullName.Split("/")[0] : "";

        var infoFile = zip.GetEntry(rootFolder + internalPath);
        if (infoFile == null) return default;
        
        var sr = new StreamReader(infoFile.Open());
        var serializer = new JsonSerializer();
        var obj = serializer.Deserialize<T>(new JsonTextReader(sr));
        sr.Close();
        file.Close();
        return obj;

    }
}