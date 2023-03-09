using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;

public class LevelUIManager : MonoBehaviour
{
    private UIDocument doc;
    private VisualElement root;
    [SerializeField] private  Player player;

    ProgressBar progressBar;
    Label scoreLabel;
    Label multiLabel;
    Label highScoreLabel;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        doc = GetComponent<UIDocument>();
        root = doc.rootVisualElement;

        progressBar = root.Q<ProgressBar>("LevelProgressBar");


        scoreLabel = root.Q<Label>("LevelScoreLabel");
        multiLabel = root.Q<Label>("LevelMultiplyLabel");
        highScoreLabel = root.Q<Label>("LevelHighScoreLabel");

       
    }

    // Update is called once per frame
    void Update()
    {
        progressBar.value = player.progress;
        progressBar.title = $"{player.progress:F0}";
        scoreLabel.text = $"Score: {Mathf.FloorToInt(player.score)}";
        multiLabel.text = $"Multiplier: {player.multiplier:F2}";

        if(Song.passedSongInstance != null)
        {
            float score = HighscoreManager.GetScore(Song.passedSongInstance.md5Hash);
            if(player.score > score)
            {
                score = player.score;
            }

            highScoreLabel.text = $"High Score: {Mathf.FloorToInt(score)}";
        }
    }
}
