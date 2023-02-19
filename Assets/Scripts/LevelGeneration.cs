using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
    public int unitSize = Mathf.RoundToInt(LevelUnit.unitTime * Player.movementSpeed); // each unit should last unitTime
    private  List<LevelUnit> units;
    private AudioAnalysis audioAnal;
    public List<GameObject> bands = new List<GameObject>();
    public int numberOfUnits = 0;
    public List<GameObject> propMap;
    
    void Start()
    {

        audioAnal = GetComponent<AudioAnalysis>(); 
        numberOfUnits = Mathf.CeilToInt((float)audioAnal.audioLen / (float)LevelUnit.unitTime);
        units = new List<LevelUnit>();

        //InvokeRepeating("SlowUpdate", 0.0f, 1f/10f);
        GenerateLevel();
        //audioAnal.audioLen;
         
        
    }

    void SlowUpdate()
    {
        int i = 0;
        foreach(SoundBands band in SoundBands.GetValues(typeof(SoundBands)))
        {
            //Debug.Log($"Band: {band} {binnedPowerLevelIncreases[band]}");
            bands[i].transform.localScale = new Vector3(1,audioAnal.binnedPowerLevelIncreases[band] / 1000,1);
            i += 1;
        }

        
    }

    void GenerateLevel()
    {
        int currentStartX = 0;
        int currentStartY = 0;
        for(int i = 0; i < numberOfUnits; i++)
        {
            LevelUnit newUnit = new LevelUnit(currentStartX,currentStartY,propMap);
            newUnit.GenerateUnit();

            currentStartX = newUnit.endX;
            currentStartY = newUnit.endY;

            units.Add(newUnit);
        }

    }
}
