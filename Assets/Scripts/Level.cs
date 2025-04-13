using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public string areaName = "";
    public string id;
    public AudioClip bgm;
    public GameObject Spawners; // entity spawners
    public GameObject dynamicGeneratedPickupsRoot;


    void Start()
    {
        DataManager.instance.CurrentLevelData.open = true;
        DataManager.instance.SaveCurrentSlot();
        var spawners = GetComponentsInChildren<Spawner>(false);
        foreach (var spawner in spawners)
        {
            if (spawner is not BossSpawner)
                spawner.Spawn();
        }
        foreach (var item in DataManager.instance.saveSlot.dynamicPickupItems)
        {
            if (item.levelId == id)
            {
                PickupItem.DynamicGeneratePickupItem(item, PreloadResource.instance.pickupItemPrefab, dynamicGeneratedPickupsRoot);
            }
        }
    }

    public void DynamicGenerattePickupItem(InventoryItem item, Vector2 position, Vector2 velocity)
    {
        PickupItem.DynamicGeneratePickupItem(
            item: new() {
                levelId = id,
                instanceId = DataManager.NewInstanceId(),
                position = position,
                data = item,
            },
            prefab: PreloadResource.instance.pickupItemPrefab,
            parent: dynamicGeneratedPickupsRoot,
            velocity: velocity
        );
    }

    void OnDestroy()
    {
        for (int i = 0; i < dynamicGeneratedPickupsRoot.transform.childCount; i++)
        {
            if (dynamicGeneratedPickupsRoot.transform.GetChild(i).TryGetComponent<PickupItem>(out var instance))
            {
                instance.SaveDataIfDynamicGenerated();
            }
        }
    }

}
