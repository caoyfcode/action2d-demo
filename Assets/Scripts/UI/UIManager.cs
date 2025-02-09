using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingletonBase<UIManager>
{
    public static BossHeathBar BossHeathBar => instance.bossHeathBar;
    public static CheckpointMenu CheckpointMenu => instance.checkpointMenu;
    public static TeleportMenu TeleportMenu => instance.teleportMenu;
    public static bool MenuShown => instance.pauseMenu.Shown || instance.checkpointMenu.Shown || instance.teleportMenu.Shown || instance.inventoryMenu.Shown || instance.settingsMenu.Shown;

    [Header("HUD")]
    // public StatusIconList playerHeath;
    // public StatusIconList playerMagic;
    public BossHeathBar bossHeathBar;
    public StatusHud playerStatusHud;
    public PickupHud pickupHud;
    
    [Header("Menus")]
    // public PauseMenu pauseMenu;
    public CheckpointMenu checkpointMenu;
    // public InventoryMenu inventoryMenu;
    public TeleportMenu teleportMenu;
    public PauseMenu pauseMenu;
    public InventoryMenu inventoryMenu;
    public SettingsMenu settingsMenu;

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

        /// TODO 添加 UI Shown Selector 枚举, 用 None, ... 来表示这些互斥的 UI 显示状态
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!MenuShown)
            {
                pauseMenu.Shown = true;
                SoundManager.instance.PlayUIConfirm();
            }
            else if (pauseMenu.Shown)
            {
                pauseMenu.Shown = false;
                SoundManager.instance.PlayUIConfirm();
            }
            else if (checkpointMenu.Shown)
            {
                checkpointMenu.Shown = false;
                SoundManager.instance.PlayUIConfirm();
            }
            else if (teleportMenu.Shown)
            {
                teleportMenu.Shown = false;
                SoundManager.instance.PlayUIConfirm();
            }
            else if (inventoryMenu.Shown)
            {
                inventoryMenu.Shown = false;
                SoundManager.instance.PlayUIConfirm();
            }
            else if (settingsMenu.Shown)
            {
                settingsMenu.Shown = false;
                SoundManager.instance.PlayUIConfirm();
            }
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            if (!MenuShown)
            {
                inventoryMenu.Shown = true;
                SoundManager.instance.PlayUIConfirm();
            }
            else if (inventoryMenu.Shown)
            {
                inventoryMenu.Shown = false;
                SoundManager.instance.PlayUIConfirm();
            }
        }
    }

    
}
