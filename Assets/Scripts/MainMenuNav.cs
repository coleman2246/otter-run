using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuNav : MonoBehaviour
{
    private UIDocument doc;
    private VisualElement root;

    private Button playButton;
    private Button settingsButton;
    private Button quitButton;

    void Start()
    {
        doc = GetComponent<UIDocument>();
        root = doc.rootVisualElement;
        
        playButton = root.Q<Button>("PlayButton");
        settingsButton = root.Q<Button>("SettingsButton");
        quitButton = root.Q<Button>("QuitButton");

        playButton.RegisterCallback<ClickEvent>(_ => PlayButtonCallback());
        settingsButton.RegisterCallback<ClickEvent>( _ => SettingsButtonCallback());
        quitButton.RegisterCallback<ClickEvent>(_ => QuitButtonCallback());

    }

    void PlayButtonCallback()
    {
        SceneManager.LoadScene("Pick Level");
    }

    void SettingsButtonCallback()
    {
        SceneManager.LoadScene("Settings");
    }

    void QuitButtonCallback()
    {
        Application.Quit();
    }

}
