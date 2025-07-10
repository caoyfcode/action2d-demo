using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoSingletonBase<UIManager>
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
    // 部分 ShownUI 是 Menu, 只有一下的是 Menu, Menu 的开启必须利用 UIManager 的函数
    public CheckpointMenu checkpointMenu;
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

        // 直接利用按键打开的 Menu, 在 UIManager 中控制开启
        if (!MenuShown)
        {
            if (GameInputManager.Pause)
            {
                OpenMenu(pauseMenu);
            }
            else if (GameInputManager.Inventory)
            {
                OpenMenu(inventoryMenu);
            }
        }
    }

    public void OpenMenu(ShownUI menu)
    {
        if (!MenuShown)
        {
            if (menu == teleportMenu || menu == checkpointMenu || menu == pauseMenu || menu == inventoryMenu || menu == settingsMenu)
            {
                menu.Shown = true;
                SoundManager.instance.PlayUIConfirm();
            }
        }
    }
}
