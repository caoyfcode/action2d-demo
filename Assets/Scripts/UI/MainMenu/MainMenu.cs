using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : SingletonBase<MainMenu>
{
    public MainList mainList;
    public SettingsMenu settingsMenu;



    void OnDestroy()
    {
        instance = null;
    }
}
