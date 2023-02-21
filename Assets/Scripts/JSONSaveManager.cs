using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization;

public class JSONSaveManager<T> where T : ISerializable 
{

    public static void SaveToJson(string filename, T classToWrite)
    {
        // will only create file if does not exist
        JSONManager.createFile(filename);
        string json = JsonUtility.ToJson(classToWrite);
        File.WriteAllText(JSONManager.savePath + filename, json);
    }
}
