using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingletonBase<UIManager>
{
    public static BossHeathBar BossHeathBar => instance.bossHeathBar;
    public static CheckpointMenu CheckpointMenu => instance.checkpointMenu;
    public static TeleportMenu TeleportMenu => instance.teleportMenu;
    public static bool MenuShown => instance.pauseMenu.Shown || instance.checkpointMenu.Shown || instance.teleportMenu.Shown || instance.inventoryMenu.Shown;

    [Header("HUD")]
    // public StatusIconList playerHeath;
    // public StatusIconList playerMagic;
    public BossHeathBar bossHeathBar;
    public StatusHud playerStatusHud;
    
    [Header("Menus")]
    // public PauseMenu pauseMenu;
    public CheckpointMenu checkpointMenu;
    // public InventoryMenu inventoryMenu;
    public TeleportMenu teleportMenu;
    public PauseMenu pauseMenu;
    public InventoryMenu inventoryMenu;

    

    [Header("Input Icon")]
    public InputIconMap inputIconMap;

    public bool debugControlShown;
    public bool debugCheckpointMenuShown;
    public bool debugTeleportMenuShown;
    public bool debugPauseMenuShown;

    void Update()
    {
        if (debugControlShown)
        {
            checkpointMenu.Shown = debugCheckpointMenuShown;
            teleportMenu.Shown = debugTeleportMenuShown;
            pauseMenu.Shown = debugPauseMenuShown;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.Shown = !pauseMenu.Shown;
        }

        if (Input.GetKeyDown(KeyCode.P) && !pauseMenu.Shown)
        {
            inventoryMenu.Shown = !inventoryMenu.Shown;
        }
    }

    
}
