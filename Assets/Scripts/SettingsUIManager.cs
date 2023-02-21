using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsUIManager : MonoBehaviour
{
    [SerializeField] private Settings settings;
    void Awake()
    {
        if(JSONManager.verifySavePathFile(Settings.settingsJSON))
        {
            settings = JSONLoadManager<Settings>.LoadJFromJson(Settings.settingsJSON);
        }
        else 
        {
            settings = Settings.getDefaults();
        }

    }

}
