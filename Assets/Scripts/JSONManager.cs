using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JSONManager : MonoBehaviour
{
    public static string savePath = Application.persistentDataPath + "/savePath/";

    // note that all files passed are expected to be in savePath dir
    public static bool verifySavePathFile(string file)
    {
        if(!File.Exists(JSONManager.savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        return File.Exists(JSONManager.savePath + file);
    }

    public static void createFile(string file)
    {
        if(!verifySavePathFile(file))
        {
            Directory.CreateDirectory(JSONManager.savePath + file);
        }
    }
    
}
