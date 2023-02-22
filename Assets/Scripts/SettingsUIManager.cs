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


    [SerializeField] private Settings settings;
    private Button setMusicPath;

    void Awake()
    {
        if(JSONManager.verifySavePathFile(Settings.settingsJSON))
        {
            settings = JSONLoadManager<Settings>.LoadJFromJson(Settings.settingsJSON);
        }
        else 
        {
            settings = Settings.getDefaults();
        }

        doc = GetComponent<UIDocument>();
        root = doc.rootVisualElement;
        
        setMusicPath = root.Q<Button>("SetMusicPathButton");



        setMusicPath.RegisterCallback<ClickEvent>(_ => PickPath());
    }

    void PickPath()
    {
        string startingDir = "";

        if(!settings.pathNull)
        {
            startingDir = settings.songDirectoryPath;
        }


        Debug.Log(startingDir);

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
