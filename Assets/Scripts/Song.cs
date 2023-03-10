using System;
using System.Linq;
using System.Security.Cryptography;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;
using System.Threading;
using UnityEngine;


[System.Serializable]
public class Song : ScriptableObject
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
    public static Song passedSongInstance; // used so we can pass between scenes
    [System.NonSerialized]
    public AudioClip clip;


    [System.NonSerialized]
    FileInfo fileInfo;

    public void SetupSong(FileInfo file)
    {
        this.fileInfo = file;

        this.path = file.FullName;
        this.fileType = file.Extension;
        this.songName = file.Name;

    }


    public int HashToInt()
    {
        string half = this.md5Hash.Substring(0, this.md5Hash.Length / 2);

        string firstQuarter = half.Substring(0, half.Length / 2);
        string secQuarter = half.Substring(half.Length / 2);

        int firstQuarterNumber = Convert.ToInt32(firstQuarter, 16);
        int secQuarterNumber = Convert.ToInt32(secQuarter, 16);

        return firstQuarterNumber ^ secQuarterNumber;

    }

    public void SaveInstance()
    {
        passedSongInstance = this;
    }

    public void CleanInstance()
    {
        passedSongInstance = null;
    }




    public static Song SongFromString(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"File: {path} does not exist");
        }

        FileInfo file = new FileInfo(path);
        Song newSong = ScriptableObject.CreateInstance<Song>();
        newSong.SetupSong(file);
        return newSong;
    }

    private static float LookUpHighScore(string hash)
    {
        return 0f;
    }


    public void ToAudioClip()
    {

        // this function was giving a lot of errors so I mixed some code from a few forum posts.
        // https://forum.unity.com/threads/load-mp3-from-user-file-at-runtime-windows.589138/
        // https://forum.unity.com/threads/load-mp3-files-saved-locally-to-audioclip.851434/
        // this should be done async, but since it will only be reading mp3s locally should not be too much of an issue

        string url = $"file://{fileInfo.FullName}";
        using (var web = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
        {
            web.SendWebRequest();

            while (!web.isDone) ;

            if (!web.isNetworkError && !web.isHttpError)
            {
                clip = DownloadHandlerAudioClip.GetContent(web);
            }
        }

    }

    public static List<Song> GetAllSongs(DirectoryInfo dir)
    {
        List<Song> songs = new List<Song>();

        FileInfo[] songFiles = dir.GetFiles("*.mp3");


        foreach (FileInfo currSong in songFiles)
        {
            Song newSong = ScriptableObject.CreateInstance<Song>();
            newSong.SetupSong(currSong);

            songs.Add(newSong);

        }

        // hashing is slow :(, lets throw more threads at it
        Parallel.ForEach(songs, currSong =>
        {
            currSong.SetHash();
        });

        return songs;

    }

    public void SetHash()
    {
        this.md5Hash = CalculateMD5(this.fileInfo);
        this.highScore = HighscoreManager.GetScore(this.md5Hash);
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
