using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PickupItem : Interactive
{
    const string DYNAMIC_GENERATED_ID = "111";
    // id 用于非动态生成的物体(用于存档中保存), instanceId 用于动态生成的物体(用于区分不同物体与存档中保存)
    public int instanceId; // unique id for each interactive object, 在场景中手动配置
    public InventoryItem item;
    public bool dynamicGenerated = false; // 表示固定放置的物品还是动态生成的物品
    Vector2 velocity = Vector2.zero;


    protected override void Start()
    {
        base.Start();
        
        if (!dynamicGenerated)// non dynamic generated, 若已经 interacted 则消耗对象
        {
            var found = DataManager.instance.CurrentLevelData.pickupItems.Find(x => x.id == id);
            if (found == null)
            {
                DataManager.instance.CurrentLevelData.pickupItems.Add(new() {
                    id = id,
                    picked = false,
                });
                DataManager.instance.SaveCurrentSlot();
            }
            else
            {
                if (found.picked)
                    DestroyImmediate(gameObject);
            }
        }
       
    }

    void Update()
    {
        if (playerIn && !UIManager.MenuShown)
        {
            HintShown = true;
            if (GameInputManager.MoveTrigger.y != 0)
            {
                HintShown = false;
                hint.gameObject.SetActive(false);
                // Show Pick Message
                UIManager.instance.pickupHud.AddItem(new InventoryItem() {
                    id = item.id,
                    quantity = item.quantity,
                });
                WorldManager.instance.InventoryAddItem(new InventoryItem() {
                    id = item.id,
                    quantity = item.quantity,
                });
                UpdateDataOnPickup();
                DestroyImmediate(gameObject);
            }
        }
        else
        {
            HintShown = false;
        }
    }

    void FixedUpdate()
    {
        velocity.y -= 30f * Time.fixedDeltaTime; // 默认 gravity==30
        if (velocity.magnitude > 10)
            velocity = velocity.normalized * 10;
        if (velocity.x > 0)
        {
            if (Physics2D.Raycast(transform.position, Vector2.right, 0.1f, LayerMask.GetMask("Environment")))
            {
                velocity.x = 0;
            }
        }
        else if (velocity.x < 0)
        {
            if (Physics2D.Raycast(transform.position, Vector2.left, 0.1f, LayerMask.GetMask("Environment")))
            {
                velocity.x = 0;
            }
        }
        if (velocity.y < 0)
        {
            if (Physics2D.Raycast(transform.position, Vector2.down, 0.1f, LayerMask.GetMask("Environment")))
            {
                velocity.y = 0;
            }
        }
        else if (velocity.y > 0)
        {
            if (Physics2D.Raycast(transform.position, Vector2.up, 0.1f, LayerMask.GetMask("Environment")))
            {
                velocity.y = 0;
            }
        }
        if (velocity.magnitude > 0.1f)
        {
            transform.position = transform.position + (Vector3)velocity * Time.fixedDeltaTime;
        }
        else
        {
            velocity = Vector2.zero;
        }
    }

    void UpdateDataOnPickup()
    {
        if (!dynamicGenerated)
        {
            var found = DataManager.instance.CurrentLevelData.pickupItems.Find(x => x.id == id);
            if (found != null)
            {
                found.picked = true;
            }
            else // 这个条件预计不会达到
            {
                DataManager.instance.CurrentLevelData.pickupItems.Add(new() {
                    id = id,
                    picked = false,
                });
            }
        }
        else
        {
            var found = DataManager.instance.saveSlot.dynamicPickupItems.Find(x => x.instanceId == instanceId);
            if (found != null)
            {
                DataManager.instance.saveSlot.dynamicPickupItems.Remove(found);
            }
        }
        DataManager.instance.SaveCurrentSlot();
    }

    // 当新动态生成的物体被创建时，或是在 Level Destory 时，或是游戏关闭时调用
    public void SaveDataIfDynamicGenerated()
    {
        if (!dynamicGenerated)
            return;
        var found = DataManager.instance.saveSlot.dynamicPickupItems.Find(x => x.instanceId == instanceId);
        if (found != null)
        {
            found.position = transform.position;
            found.data = new InventoryItem() {
                id = item.id,
                quantity = item.quantity,
            };
        }
        else
        {
            DataManager.instance.saveSlot.dynamicPickupItems.Add(new DynamicPickupItemSaveData() {
                levelId = WorldManager.instance.currentLevel.id,
                instanceId = instanceId,
                position = transform.position,
                data = new InventoryItem() {
                    id = item.id,
                    quantity = item.quantity,
                }
            });
        }
        DataManager.instance.SaveCurrentSlot();
    }

    // 用于在加载关卡式生成或是在游戏内生成
    public static void DynamicGeneratePickupItem(DynamicPickupItemSaveData item, GameObject prefab, GameObject parent=null, Vector2 position=default, Vector2 velocity=default)
    {
        GameObject obj = Instantiate(prefab, position, Quaternion.identity, parent.transform);
        var pickup = obj.GetComponent<PickupItem>();
        pickup.dynamicGenerated = true;
        pickup.id = DYNAMIC_GENERATED_ID;
        pickup.instanceId = item.instanceId;
        pickup.item.id = item.data.id;
        pickup.item.quantity = item.data.quantity;
        obj.transform.position = position;
        pickup.velocity = velocity;
        pickup.SaveDataIfDynamicGenerated(); // 照理来说需要区分是游戏内生成还是加载后生成的，如果是加载后生成的就不用，但是做了也没关系
    }
}

[System.Serializable]
public class DynamicPickupItemSaveData
{
    public string levelId;
    public int instanceId;
    public Vector2 position;
    public InventoryItem data;
}

[System.Serializable]
public class LevelPickupItemSaveData
{
    public string id;
    public bool picked;
}
