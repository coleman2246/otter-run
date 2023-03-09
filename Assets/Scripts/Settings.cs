using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public enum WorldTheme
{
    Industrial
}

[System.Serializable]
public enum Character
{
    Otter,
    Ball
}

[System.Serializable]
public class Settings 
{
    public string songDirectoryPath;
    public WorldTheme selectedTheme;
    public Character selectedCharacter;
    public int volumeLevel;

    [System.NonSerialized]
    public DirectoryInfo songDir; 
    [System.NonSerialized]
    public static string settingsJSON = "settings.json";
    [System.NonSerialized]
    public bool pathNull;
    [System.NonSerialized]

    public static Dictionary<string,Character> stringToChar = new Dictionary<string,Character>(){
            {"Ball" , Character.Ball},
            {"Otter" , Character.Otter}
    };

    public static Dictionary<string,Character> characterMap = new Dictionary<string,Character>(){
            {"Ball" , Character.Ball},
            {"Otter" , Character.Otter}
    };





    public Settings(string songDirectoryPath, WorldTheme selectedTheme, Character selectedCharacter)
    {
        this.songDirectoryPath = songDirectoryPath;
        this.selectedTheme = selectedTheme;
        this.selectedCharacter = selectedCharacter;
        
    }


    public void SetupComplexObjects()
    {
        this.pathNull = this.songDirectoryPath == "";

        if(!this.pathNull)
        {
            this.songDir = new DirectoryInfo(this.songDirectoryPath);
        }
    }

    public static Settings getDefaults()
    {
        return new Settings("", WorldTheme.Industrial, Character.Otter);
    }
    
}
