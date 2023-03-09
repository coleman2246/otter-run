using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PauseMenuUIManager : MonoBehaviour
{
    private UIDocument doc;
    private VisualElement root;
    [SerializeField] private Player player; 

    private Button continueButton;
    private Button restartButton;
    private Button quitButton;
    private SliderInt volumeSlider;
    private Settings settings;
    public bool isVisible = false; 


    // Start is called before the first frame update
    void Start()
    {
        doc = GetComponent<UIDocument>();
        root = doc.rootVisualElement;
        
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        settings = JSONLoadManager<Settings>.LoadFromJson(Settings.settingsJSON);
        continueButton = root.Q<Button>("ContinueButton");
        restartButton = root.Q<Button>("RestartButton");
        quitButton = root.Q<Button>("QuitButton");
        volumeSlider = root.Q<SliderInt>("VolumeSlider");
        volumeSlider.value =  settings.volumeLevel;
        
        SetVolume();

        
        continueButton.RegisterCallback<ClickEvent>(_ => ContinueLevel());
        restartButton.RegisterCallback<ClickEvent>(_ => RestartLevel());
        quitButton.RegisterCallback<ClickEvent>(_ => QuitLevel());
        volumeSlider.RegisterValueChangedCallback(VolumeChanged);
        

    }

    void VolumeChanged(ChangeEvent<int> value)
    {
        settings.volumeLevel = value.newValue;
        SetVolume();
        JSONSaveManager<Settings>.SaveToJson(Settings.settingsJSON, settings);
    }

    void SetVolume()
    {
        player.levelGen.audioAnal.audioSource.volume = (float)settings.volumeLevel / 100f;
    }


    void ContinueLevel()
    {
        player.isPaused = false;

    }

    void RestartLevel()
    {
        player.RestartLevel();
    }

    void QuitLevel()
    {

        SceneManager.LoadScene("Pick Level");
    }

    // Update is called once per frame
    void Update()
    {

        root.visible = isVisible;
        
        continueButton.visible = !(player.isEnded || player.isDead) && isVisible;
        isVisible = player.isPaused || player.isDead || player.isEnded;
    }

}
