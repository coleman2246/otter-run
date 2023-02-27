using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighscoreManager : MonoBehaviour
{

    public static string highScoreJson = "highScores.json";
    public static SerializableDictionary<string, float> scores = InitScores();

    public static SerializableDictionary<string, float> InitScores()
    {
        if(JSONManager.verifySavePathFile(highScoreJson))
        {
            return JSONLoadManager<SerializableDictionary<string, float>>.LoadFromJson(highScoreJson);
        }

        return new SerializableDictionary<string,float>();
    
    }

    static public float GetScore(string hash)
    {
        
        if(scores.ContainsKey(hash))
        {
            return scores[hash];
        }

        else
        {
            scores.Add(hash,0f);

            if(JSONManager.verifySavePathFile(highScoreJson))
            {
                JSONSaveManager<SerializableDictionary<string,float>>.SaveToJson(highScoreJson, scores);
            }

        }

        return 0f;
    }

    static public bool UpdateHighScore(string hash, float newScore)
    {
        float oldScore = GetScore(hash);

        if(newScore > oldScore)
        {
            scores[hash] = newScore;
            foreach(string key in scores.Keys)
            {
                Debug.Log($"Writing: {key}");
            }
            JSONSaveManager<SerializableDictionary<string,float>>.SaveToJson(highScoreJson, scores);
            return true;
        }
        return false;

    }

}
