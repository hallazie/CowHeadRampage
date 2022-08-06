using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SystemManager
{
    private static SystemManager _instance;
    public static SystemManager instance
    {
        get
        {
            return _instance;
        }
    }

    // public string savePath = Application.persistentDataPath + "/CHR-save.bin";
    public string savePath = Application.persistentDataPath + "/CHR-save.json";
    public int maxSceneReached = 0;

    public SystemManager()
    {
        _instance = this;
    }

    public void SaveSystemStatus(SaveLoadData data)
    {
        //BinaryFormatter formatter = new BinaryFormatter();
        //Stream stream = new FileStream(savePath, FileMode.Create);
        //formatter.Serialize(stream, data);
        //stream.Close();

        string jsonString = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, jsonString);

    }

    public SaveLoadData LoadSystemStatus()
    {
        if (File.Exists(savePath))
        {
            try
            {
                //BinaryFormatter formatter = new BinaryFormatter();
                //FileStream stream = new FileStream(savePath, FileMode.Open);
                //SaveLoadData data = (SaveLoadData)formatter.Deserialize(stream);
                //stream.Close();

                string jsonString = File.ReadAllText(savePath);
                SaveLoadData data = JsonUtility.FromJson<SaveLoadData>(jsonString);

                maxSceneReached = data.maxSceneReached;
                return data;
            }catch
            {
                Debug.LogError("Unable to read save file at: " + savePath);
                return null;
            }
        }
        else
        {
            Debug.LogError("Save file not found at: " + savePath);
            return null;
        }
    }

}
