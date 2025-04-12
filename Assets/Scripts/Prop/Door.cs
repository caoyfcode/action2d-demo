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
        var found = DataManager.instance.CurrentLevelData.doors.Find(x => x.id == id);
        if (found == null)
        {
            IsOpen = sr.sprite == openSprite; // 利用默认状态来初始化
            DataManager.instance.CurrentLevelData.doors.Add(new(){
                id = id,
                open = IsOpen,
            });
            DataManager.instance.SaveCurrentSlot();
        }
        else
        {
            IsOpen = found.open;
        }
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
                    var found = DataManager.instance.CurrentLevelData.doors.Find(x => x.id == id);
                    if (found != null)
                    {
                        found.open = true;
                    }
                    else // 估计这里不会发生
                    {
                        DataManager.instance.CurrentLevelData.doors.Add(new(){
                            id = id,
                            open = true,
                        });
                    }
                    DataManager.instance.SaveCurrentSlot();
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


[System.Serializable]
public class LevelDoorSaveData
{
    public string id;
    public bool open;
}
