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

    // Start is called before the first frame update
    void Start()
    {
        doc = GetComponent<UIDocument>();
        root = doc.rootVisualElement;

        progressBar = root.Q<ProgressBar>("LevelProgressBar");
        scoreLabel = root.Q<Label>("LevelScoreLabel");
        multiLabel = root.Q<Label>("LevelMultiplyLabel");

        
    }

    // Update is called once per frame
    void Update()
    {
        progressBar.value = player.progress;
        scoreLabel.text = $"Score: {Mathf.FloorToInt(player.score)}";
        multiLabel.text = $"Multiplier: {player.multiplier:F2}";
    }
}
