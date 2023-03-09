using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine;

public class NoSongsElement : VisualElement
{
    public NoSongsElement()
    {


        Button messageButton = new Button();
        messageButton.text = $"No Songs. Click Here To Navigate To Settings.";

    


        messageButton.style.position = Position.Absolute;

        messageButton.style.width = Screen.width * 0.5f;
        messageButton.style.height = Screen.height * 0.3f;

        messageButton.style.left = Screen.width * 0.25f;


        messageButton.style.top = Screen.height * 0.25f;

        /*
        messageButton.style.left = 0;
        messageButton.style.right = 0;
        messageButton.style.top = 0;
        messageButton.style.bottom = 0;
        */


        messageButton.RegisterCallback<ClickEvent>(_ => NavigateToSettings());

        Add(messageButton);


    }


    void NavigateToSettings()
    {
        SceneManager.LoadScene("Settings");
    }
    
}
