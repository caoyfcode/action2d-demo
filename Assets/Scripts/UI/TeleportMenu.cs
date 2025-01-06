using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeleportMenu : Menu
{
    public GameObject itemPrefab;

    public GameObject listNode;
    public List<Vector2Int> levelIds;
    public Map map;
    
    public int selectedId = -1;

    public void Close()
    {
        Shown = false;
    }

    void Start()
    {
        if (listNode.transform.childCount == 0)
        {
            foreach (var checkpoint in Game.instance.levelData.checkpointList)
            {
                var obj = Instantiate(itemPrefab, listNode.transform);
                var textComp = obj.GetComponentInChildren<Text>();
                textComp.text = checkpoint.name;
                levelIds.Add(checkpoint.index);
            }
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
        if (Input.GetKeyDown(KeyCode.K))
        {
            // TODO Teleport to selectedId
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            Shown = false;
        }
    }

    void UpdateSelection()
    {
        bool up = Input.GetKeyDown(KeyCode.W);
        bool down = Input.GetKeyDown(KeyCode.S);
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
                listNode.transform.GetChild(prevSelected).GetComponent<ItemBox>().selected = false;
            }
            if (selectedId >= 0 && selectedId < listNode.transform.childCount)
            {
                listNode.transform.GetChild(selectedId).GetComponent<ItemBox>().selected = true;
            }
        }
    }

    void HandleMapPosition()
    {
        if (selectedId < 0 || selectedId >= listNode.transform.childCount)
        {
            return;
        }
        map.MoveTo(Game.instance.levelData.GetCheckpointPosition(levelIds[selectedId]));
    }
}
