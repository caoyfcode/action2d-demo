using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObject/ItemData", order = 3)]
public class ItemData : ScriptableObject
{
    public const string HeroKnightItemId = "1000";
    public const string ArcaneArcherItemId = "1001";
    public const string FireKnightItemId = "1002";
    public const string GolemItemId = "1003";
    public static HashSet<string> AttackItemIds = new HashSet<string>() { HeroKnightItemId, ArcaneArcherItemId, FireKnightItemId, GolemItemId }; // TODO  也许将 Attack 改名为 Skill 更合适 

    public const string DashItemId = "2000";
    public const string DoubleJumpItemId = "2001";
    public const string WallSlideItemId = "2002";
    public static HashSet<string> AbilityItemIds = new HashSet<string>() { DashItemId, DoubleJumpItemId, WallSlideItemId };

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

[System.Serializable]
public class InventoryItem
{
    public string id;
    public int quantity;
}
