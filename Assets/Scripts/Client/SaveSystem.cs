using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    public static void SaveGame(PlayerData playerData)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = SaveDataPersistentPath();
        FileStream fs = new FileStream(path, FileMode.Create);
        var saveData = new SaveData(playerData);
        
        formatter.Serialize(fs, saveData);
        fs.Close();
    }

    public static SaveData LoadGame()
    {
        string path = SaveDataPersistentPath();

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fs = new FileStream(path, FileMode.Open);
            SaveData saveData = formatter.Deserialize(fs) as SaveData;
            fs.Close();

            return saveData;
        }

        return new SaveData();
    }

    private static string SaveDataPersistentPath()
    {
        return Application.persistentDataPath + "/save.yes";
    }
}
