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
    Santa,
    Ball
}

[System.Serializable]
public class Settings 
{
    public string songDirectoryPath;
    public WorldTheme selectedTheme;
    public Character selectedCharacter;

    [System.NonSerialized]
    public DirectoryInfo songDir; 
    [System.NonSerialized]
    public static string settingsJSON = "settings.json";
    [System.NonSerialized]
    public bool pathNull;



    public Settings(string songDirectoryPath, WorldTheme selectedTheme, Character selectedCharacter)
    {
        this.songDirectoryPath = songDirectoryPath;
        this.selectedTheme = selectedTheme;
        this.selectedCharacter = selectedCharacter;
        this.pathNull = this.songDirectoryPath == "";

        if(!this.pathNull)
        {
            this.songDir = new DirectoryInfo(this.songDirectoryPath);
        }
    }

    public static Settings getDefaults()
    {
        return new Settings("", WorldTheme.Industrial, Character.Santa);
    }
    
}
