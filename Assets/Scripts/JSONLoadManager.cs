using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.IO;
using System;

using UnityEngine;

public class JSONLoadManager<T> 
{
    public static T LoadFromJson(string file)
    {
        if(!JSONManager.verifySavePathFile(file))
        {
            Debug.Log("Trying to read file that does not exist"); 
        }

        string json = File.ReadAllText( JSONManager.savePath + file);
        T obj = JsonUtility.FromJson<T>(json);
        return obj;
    }
}
