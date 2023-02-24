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

    public bool isVisible = false; 


    // Start is called before the first frame update
    void Start()
    {
        doc = GetComponent<UIDocument>();
        root = doc.rootVisualElement;
        
        continueButton = root.Q<Button>("ContinueButton");
        restartButton = root.Q<Button>("RestartButton");
        quitButton = root.Q<Button>("QuitButton");
        
        continueButton.RegisterCallback<ClickEvent>(_ => ContinueLevel());
        restartButton.RegisterCallback<ClickEvent>(_ => RestartLevel());
        quitButton.RegisterCallback<ClickEvent>(_ => QuitLevel());
    }


    void ContinueLevel()
    {

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
