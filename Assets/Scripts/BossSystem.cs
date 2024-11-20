using System;
using UnityEngine;

public class BossSystem : MonoBehaviour
{
    public Entity boss;
    public bool bossFightStarted = false;


    public void StartBossFight(Entity boss)
    {
        if (!bossFightStarted && boss)
        {
            this.boss = boss;
            bossFightStarted = true;
            UIManager.BossHeathBar.valueBar.maxValue = boss.CharacterData.maxHeath;
            UIManager.BossHeathBar.valueBar.value = boss.CharacterData.heath;
            UIManager.BossHeathBar.Shown = true;
            boss.onDeath.AddListener(EndBossFight);
        }
    }

    public void EndBossFight()
    {
        if (bossFightStarted)
        {
            bossFightStarted = false;
            UIManager.BossHeathBar.Shown = false;
            if (boss != null)
                boss.onDeath.RemoveListener(EndBossFight);
            boss = null;
        }
    }

    void Update()
    {
        if (bossFightStarted && boss != null)
        {
            Debug.Log("Boss Health: " + boss.CharacterData.heath);
            Debug.Log("Bar value: " + UIManager.BossHeathBar.valueBar.value);
            UIManager.BossHeathBar.valueBar.value = boss.CharacterData.heath;
        }
    }
}