using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelPickerUIManager : MonoBehaviour
{
    private Settings settings;

    private UIDocument doc;
    private VisualElement root;
    private ScrollView songListView;
    private VisualElement con;


    void Start()
    {
        doc = GetComponent<UIDocument>();
        root = doc.rootVisualElement;
     

        if(JSONManager.verifySavePathFile(Settings.settingsJSON))
        {
            settings = JSONLoadManager<Settings>.LoadFromJson(Settings.settingsJSON);
        }
        else 
        {
            settings = Settings.getDefaults();
        }

        settings.SetupComplexObjects();

        songListView= new ScrollView();
        con = root.Q<VisualElement>("SongListContainer");


        if(settings.pathNull)
        {
            NoSongs();
        }
        else
        {
            GenerateLevelPickerList();
        }

    }

    
    
    void GenerateLevelPickerList()
    {
        List<Song> songs = Song.GetAllSongs(settings.songDir);
        List<LevelPickerItem> items = new List<LevelPickerItem>();

        foreach(Song song in songs)
        {
            LevelPickerItem item = new LevelPickerItem(song);
            items.Add(item);
            songListView.Add(item);
        }
        con.Add(songListView);
    }


    void NoSongs()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Cancel"))
        {
            SceneManager.LoadScene("MainMenu");
        }       
    }
}
