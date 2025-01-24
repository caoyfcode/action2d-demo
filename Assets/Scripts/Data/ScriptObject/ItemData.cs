using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObject/ItemData", order = 3)]
public class ItemData : ScriptableObject
{
    [Serializable]
    public class Data
    {
        public string id;
        public string name;
        public Sprite icon;
        public string description;
    }

    public enum ItemType
    {
        Common,
        Skill,
        Ability,
    }

    public List<Data> itemList = new();
    public Dictionary<string, Data> itemMap = new();


    // Start is called before the first frame update
    void OnEnable()
    {
        itemMap.Clear();
        foreach (var item in itemList)
        {
            itemMap.Add(item.id, item);
        }
    }

    public Sprite GetItemSprite(string id)
    {
        return itemMap.ContainsKey(id) ? itemMap[id].icon : null;
    }

    public Data GetItemData(string id)
    {
        return itemMap.ContainsKey(id) ? itemMap[id] : null;
    }

}
