using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelPickerItem : VisualElement
{
    Song song;

    public LevelPickerItem(Song song)
    {
        this.song = song;

          // Create the song information label
        Label songInfoLabel = new Label($"Highscore: {song.highScore}");
        songInfoLabel.style.marginLeft = 10;

        // Create the play button
        Button playButton = new Button();
        playButton.text = $"{song.songName}";
        playButton.style.marginLeft = 10;
        playButton.style.marginTop = 20;

        playButton.RegisterCallback<ClickEvent>(_ => PlayLevel());

        // Add the song information label and play button to the custom visual element
        Add(playButton);
        Add(songInfoLabel);
    }

    void PlayLevel()
    {
        song.ToAudioClip();
        song.SaveInstance();
        SceneManager.LoadScene("Level");
    }

    
    
}
