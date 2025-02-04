using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointMenu : Menu
{

    public ItemBox heal;
    public ItemBox teleport;
    public ItemBox upgrade;
    public ItemBox leave;
    private ItemBox GetItem(int index) => index switch
    {
        0 => heal,
        1 => teleport,
        2 => upgrade,
        3 => leave,
        _ => null
    };
    public int selected = -1;

    void Update()
    {
        UpdateSelection();
        HandleInput();
    }

    void UpdateSelection()
    {
        bool up = Input.GetKeyDown(KeyCode.W);
        bool down = Input.GetKeyDown(KeyCode.S);
        int prevSelected = selected;
        if (down && !up)
        {
            if (selected < 3)
            {
                selected++;
            }
            else
            {
                selected = 0;
            }
        }
        else if (up && !down)
        {
            if (selected > 0)
            {
                selected--;
            }
            else
            {
                selected = 3;
            }
        }
        if (prevSelected != selected)
        {
            if (prevSelected >= 0 && prevSelected < 4)
            {
                GetItem(prevSelected).selected = false;
            }
            if (selected >= 0 && selected < 4)
            {
                GetItem(selected).selected = true;
            }
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            switch (selected)
            {
                case 0:
                    Heal();
                    break;
                case 1:
                    Teleport();
                    break;
                case 2:
                    Upgrade();
                    break;
                case 3:
                    Leave();
                    break;
            }
        }
    }

    public void Heal()
    {
        WorldManager.Player.Heal();
        Shown = false;
    }

    public void Teleport()
    {
        Shown = false;
        UIManager.TeleportMenu.Shown = true;
    }

    public void Upgrade()
    {
        // nothing
    }

    public void Leave()
    {
        Shown = false;
    }

    
}
