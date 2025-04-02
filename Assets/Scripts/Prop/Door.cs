using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactive
{
    public SpriteRenderer sr;
    public Sprite openSprite;
    public Sprite closeSprite;
    public GameObject doorCollider;
    public bool IsOpen 
    {
        get => sr.sprite == openSprite;
        set
        {
            if (value)
            {
                sr.sprite = openSprite;
                doorCollider.SetActive(false);
            }
            else
            {
                sr.sprite = closeSprite;
                doorCollider.SetActive(true);
            }
        }
    }

    protected override void Start()
    {
        base.Start();
        // TODO 从存档查询状态
        HintShown = false;
        IsOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOpen && playerIn && !UIManager.MenuShown)
        {
            if (WorldManager.Player.isOnGround)
            {
                HintShown = true;
                if (GameInputManager.MoveTrigger.y != 0)
                {
                    HintShown = false;
                    IsOpen = true;
                }
            }
            else
            {
                HintShown = false;
            }
        }
        else
        {
            HintShown = false;
        }
    }
}
