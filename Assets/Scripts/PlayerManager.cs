using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    [SerializeField] private static string prefabFolder = "Assets/Prefabs/Players";
    //public static Dictionary<Character,GameObject> players = GetPlayersPrefabs();
    public Settings settings;

    public static Dictionary<Character,GameObject> GetPlayersPrefabs()
    {

        Dictionary<Character,GameObject> map = new Dictionary<Character,GameObject>();

        foreach(string id in AssetDatabase.FindAssets("t:Prefab", new[] { PlayerManager.prefabFolder }))
        {
            string path = AssetDatabase.GUIDToAssetPath(id);

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if(Settings.characterMap.ContainsKey(prefab.name))
            {
                SpriteRenderer renderer = prefab.GetComponent<SpriteRenderer>();

                if(renderer == null)
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
        if(JSONManager.verifySavePathFile(Settings.settingsJSON))
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
