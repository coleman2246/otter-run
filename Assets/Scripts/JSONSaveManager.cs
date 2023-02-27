using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization;

public class JSONSaveManager<T> 
{

    public static void SaveToJson(string filename, T classToWrite)
    {
        string json = JsonUtility.ToJson(classToWrite);
        File.WriteAllText(JSONManager.savePath + filename, json);
    }
}
