using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
    public int unitSize = Mathf.RoundToInt(LevelUnit.unitTime * Player.movementSpeed); // each unit should last unitTime
    public List<GameObject> bands = new List<GameObject>();
    public int numberOfUnits = 0;
    public List<GameObject> propMap;
    public Vector2 startLocation;
    public Vector2 endLocation;

    private  List<LevelUnit> units;
    public AudioAnalysis audioAnal;
    
    void Awake()
    {

        audioAnal = GetComponent<AudioAnalysis>(); 
        numberOfUnits = Mathf.CeilToInt((float)audioAnal.audioLen / (float)LevelUnit.unitTime);
        units = new List<LevelUnit>();

        GenerateLevel();
    }

    void GenerateLevel()
    {
        int currentStartX = 0;
        int currentStartY = 0;

        startLocation = new Vector2(currentStartX,currentStartY);

        for(int i = 0; i < numberOfUnits; i++)
        {
            audioAnal.GenerateAnalysis(i);

            LevelUnit newUnit = new LevelUnit(currentStartX,currentStartY,propMap,audioAnal, i+1 >= numberOfUnits);
            newUnit.GenerateUnit();

            currentStartX = newUnit.worldEndX;
            currentStartY = newUnit.worldEndY;

            units.Add(newUnit);
        }

        endLocation = new Vector2(currentStartX,currentStartY);


        audioAnal.CleanUpDFT();
    }
}
