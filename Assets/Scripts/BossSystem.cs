using System;
using UnityEngine;

public class BossSystem : MonoBehaviour
{
    public BossHeathBar bossHeathBar;
    public Entity boss;
    public bool bossFightStarted = false;


    public void StartBossFight(Entity boss)
    {
        if (!bossFightStarted && boss)
        {
            this.boss = boss;
            bossFightStarted = true;
            bossHeathBar.valueBar.maxValue = boss.CharacterData.maxHeath;
            bossHeathBar.valueBar.value = boss.CharacterData.heath;
            bossHeathBar.Shown = true;
            boss.onDeath.AddListener(EndBossFight);
        }
    }

    public void EndBossFight()
    {
        if (bossFightStarted)
        {
            bossFightStarted = false;
            bossHeathBar.Shown = false;
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
            Debug.Log("Bar value: " + bossHeathBar.valueBar.value);
            bossHeathBar.valueBar.value = boss.CharacterData.heath;
        }
    }
}