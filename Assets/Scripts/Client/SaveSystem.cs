using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    public static void SaveGame(SaveData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = SaveDataPersistentPath();
        FileStream fs = new FileStream(path, FileMode.Create);
        
        formatter.Serialize(fs, data);
        fs.Close();
    }

    public static void LoadGame(SaveData data)
    {
        string path = SaveDataPersistentPath();

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            data = formatter.Deserialize(stream) as SaveData;
            stream.Close();
        }
    }

    private static string SaveDataPersistentPath()
    {
        return Application.persistentDataPath + "/save.yes";
    }
}
