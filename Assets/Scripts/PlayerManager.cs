using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    [SerializeField] private static string prefabFolder = "Prefabs/Players";
    //public static Dictionary<Character,GameObject> players = GetPlayersPrefabs();
    public Settings settings;

    public static Dictionary<Character, GameObject> GetPlayersPrefabs()
    {

        Dictionary<Character, GameObject> map = new Dictionary<Character, GameObject>();

        foreach (GameObject prefab in Resources.LoadAll<GameObject>(PlayerManager.prefabFolder))
        {
            if (Settings.characterMap.ContainsKey(prefab.name))
            {
                SpriteRenderer renderer = prefab.GetComponent<SpriteRenderer>();

                if (renderer == null)
                {
                    Debug.LogError($"Prefab Does not have a SpriteRenderer: {prefab.name}");
                    continue;
                }
                Character currentChar = Settings.characterMap[prefab.name];
                map[currentChar] = prefab;
            }
            else
            {
                Debug.LogError($"Unregistered Prefab {prefab.name}");
            }
        }

        return map;
    }

    void Awake()
    {
        if (JSONManager.verifySavePathFile(Settings.settingsJSON))
        {
            settings = JSONLoadManager<Settings>.LoadFromJson(Settings.settingsJSON);
        }
        else
        {
            settings = Settings.getDefaults();
        }

        Instantiate(GetPlayersPrefabs()[settings.selectedCharacter]);
    }
}
