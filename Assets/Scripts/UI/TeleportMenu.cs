using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeleportMenu : ShownUI
{
    public GameObject itemPrefab;

    public GameObject listNode;
    public List<string> levelIds;
    public Map map;
    
    public int selectedId = -1;

    public void Close()
    {
        Shown = false;
    }

    // 每当 SetActive 或是初始时
    void Awake()
    {
        if (listNode.transform.childCount > 0)
        {
            InitSelection();
        }
    }

    // 第一次打卡时初始化列表
    void Start()
    {
        if (listNode.transform.childCount == 0)
        {
            foreach (var checkpoint in WorldManager.instance.levelData.checkpointList)
            {
                var obj = Instantiate(itemPrefab, listNode.transform);
                var textComp = obj.GetComponentInChildren<Text>();
                textComp.text = checkpoint.name;
                levelIds.Add(checkpoint.levelId);
            }
            InitSelection();
        }
    }

    void Update()
    {
        UpdateSelection();
        HandleInput();
        HandleMapPosition();
    }

    void HandleInput()
    {
        if (GameInputManager.UiConfirm)
        {
            var targetLevelId = levelIds[selectedId];
            if (targetLevelId != WorldManager.CurrentLevel.id)
            {
                // WorldManager.Teleport(targetLevelId, WorldManager.instance.levelData.GetCheckpointPosition(targetLevelId));
                WorldManager.TeleportCheckpoint(targetLevelId);
            }
            Shown = false;
            SoundManager.instance.PlayUIConfirm();
        }
        else if (GameInputManager.UiCancel)
        {
            Shown = false;
            SoundManager.instance.PlayUIConfirm();
        }
    }

    void InitSelection()
    {
        int prevSelected = selectedId;
        selectedId = levelIds.IndexOf(WorldManager.CurrentLevel.id);
        if (prevSelected != selectedId)
        {
            if (prevSelected >= 0 && prevSelected < listNode.transform.childCount)
            {
                listNode.transform.GetChild(prevSelected).GetComponent<SelectItem>().SetSelectedInstantly(false);
            }
            if (selectedId >= 0 && selectedId < listNode.transform.childCount)
            {
                listNode.transform.GetChild(selectedId).GetComponent<SelectItem>().SetSelectedInstantly(true);
            }
        }
    }

    void UpdateSelection()
    {
        bool up = GameInputManager.MoveTrigger.y > 0;
        bool down = GameInputManager.MoveTrigger.y < 0;
        int prevSelected = selectedId;
        if (down && !up)
        {
            if (selectedId < listNode.transform.childCount - 1)
            {
                selectedId += 1;
            }
            else
            {
                selectedId = 0;
            }
        }
        else if (up && !down)
        {
            if (selectedId > 0)
            {
                selectedId -= 1;
            }
            else
            {
                selectedId = listNode.transform.childCount - 1;
            }
        }
        if (prevSelected != selectedId)
        {
            if (prevSelected >= 0 && prevSelected < listNode.transform.childCount)
            {
                listNode.transform.GetChild(prevSelected).GetComponent<SelectItem>().selected = false;
            }
            if (selectedId >= 0 && selectedId < listNode.transform.childCount)
            {
                listNode.transform.GetChild(selectedId).GetComponent<SelectItem>().selected = true;
                SoundManager.instance.PlayUISelection();
            }
        }
    }

    void HandleMapPosition()
    {
        if (selectedId < 0 || selectedId >= listNode.transform.childCount)
        {
            return;
        }
        map.MoveTo(WorldManager.instance.levelData.GetCheckpointPosition(levelIds[selectedId]));
    }
}
