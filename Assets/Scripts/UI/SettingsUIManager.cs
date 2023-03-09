using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using SFB;

public class SettingsUIManager : MonoBehaviour
{
    private UIDocument doc;
    private VisualElement root;


    private Settings settings;
    private Button setMusicPath;
    private SliderInt volumeSlider;
    private Foldout characterFoldOut;



    void Awake()
    {
        if(JSONManager.verifySavePathFile(Settings.settingsJSON))
        {
            settings = JSONLoadManager<Settings>.LoadFromJson(Settings.settingsJSON);
        }
        else 
        {
            settings = Settings.getDefaults();
        }

        doc = GetComponent<UIDocument>();
        root = doc.rootVisualElement;
        
        setMusicPath = root.Q<Button>("SetMusicPathButton");
        volumeSlider = root.Q<SliderInt>("VolumeSlider");
        PopulateFoldOut();

        volumeSlider.value =  settings.volumeLevel;

        setMusicPath.RegisterCallback<ClickEvent>(_ => PickPath());
        volumeSlider.RegisterValueChangedCallback(VolumeChanged);

    }


    void PopulateFoldOut()
    {

        characterFoldOut = root.Q<Foldout>("CharacterFoldOut");


        foreach(KeyValuePair<Character,GameObject> kv in PlayerManager.GetPlayersPrefabs())
        {

            Character currentChar = kv.Key;
            GameObject prefab = kv.Value;

            


            SpriteRenderer renderer = prefab.GetComponent<SpriteRenderer>();
            Texture2D texture = renderer.sprite.texture;
            Image icon = new Image();

            icon.style.paddingTop = 10;

            if(currentChar == settings.selectedCharacter)
            {
                Color borderCol = new Color(0.71f, 0.353f, 0.067f, 1f);

                icon.style.borderBottomColor = borderCol;
                icon.style.borderBottomWidth = 3;
                icon.style.borderTopColor = borderCol;
                icon.style.borderTopWidth = 3;
                icon.style.borderLeftColor = borderCol;
                icon.style.borderLeftWidth = 3;
                icon.style.borderRightColor = borderCol;
                icon.style.borderRightWidth = 3;
            }

            icon.image = texture;


            icon.RegisterCallback<ClickEvent>(evt =>
            {
                settings.selectedCharacter = currentChar;
                JSONSaveManager<Settings>.SaveToJson(Settings.settingsJSON, settings);
                characterFoldOut.Clear();
                PopulateFoldOut();
            });

            characterFoldOut.Add(icon);
        }

    }

    void VolumeChanged(ChangeEvent<int> value)
    {
        settings.volumeLevel = value.newValue;
        JSONSaveManager<Settings>.SaveToJson(Settings.settingsJSON, settings);
    }

    void PickPath()
    {
        string startingDir = "";

        if(!settings.pathNull)
        {
            startingDir = settings.songDirectoryPath;
        }



        // https://github.com/gkngkc/UnityStandaloneFileBrowser/blob/master/Assets/StandaloneFileBrowser/StandaloneFileBrowser.cs#L87
        string[] paths = StandaloneFileBrowser.OpenFolderPanel("Select Music Folder", startingDir, false);


        if (paths.Length > 0)
        {

            Debug.Log(paths[0]);
            settings = new Settings(paths[0], settings.selectedTheme, settings.selectedCharacter);
            JSONSaveManager<Settings>.SaveToJson(Settings.settingsJSON,settings);
        }

    }

    void Update()
    {
        if(Input.GetButton("Cancel"))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
