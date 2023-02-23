using System;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


[System.Serializable]
public class Song : MonoBehaviour
{
    public string md5Hash;
    public float highScore;


    [System.NonSerialized]
    public string path;
    [System.NonSerialized]
    public string fileType;
    [System.NonSerialized]
    public string songName;

    
    [System.NonSerialized]
    FileInfo fileInfo;

    public Song(FileInfo file)
    {
        this.fileInfo = file;

        this.path = file.FullName;
        this.fileType = file.Extension;
        this.songName = file.Name;

        this.md5Hash = CalculateMD5(file);
        this.highScore = LookUpHighScore(this.md5Hash);

    }

    public static Song SongFromString(string path)
    {
        if(!File.Exists(path))
        {
            throw new FileNotFoundException($"File: {path} does not exist");
        }

        FileInfo file = new FileInfo(path);
        return new Song(file);


    }

    private static float LookUpHighScore(string hash)
    {
        return 0f;
    }

    public static List<Song> GetAllSongs(DirectoryInfo dir)
    {
        List<Song> songs = new List<Song>();


        FileInfo[] songFiles = dir.GetFiles("*.mp3");

        foreach(FileInfo currSong in songFiles)
        {
            songs.Add(new Song(currSong));
        }

        return songs;

    }

    // taken from:
    //https://stackoverflow.com/questions/10520048/calculate-md5-checksum-for-a-file
    static string CalculateMD5(FileInfo file)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(file.FullName))
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }

    
}
