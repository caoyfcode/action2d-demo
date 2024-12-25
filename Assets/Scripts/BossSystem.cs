using System;
using UnityEngine;

public class BossSystem : MonoBehaviour
{
    public Entity boss;
    public bool bossFightStarted = false;


    public void StartBossFight(Entity boss, Rect room)
    {
        if (!bossFightStarted && boss)
        {
            this.boss = boss;
            bossFightStarted = true;
            UIManager.BossHeathBar.valueBar.maxValue = boss.CharacterData.maxHealth;
            UIManager.BossHeathBar.valueBar.value = boss.CharacterData.health;
            UIManager.BossHeathBar.Shown = true;
            boss.onDeath.AddListener(EndBossFight);
            boss.controller.SetBossRoom(room);
            boss.controller.OnStartBossFight();
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
            Debug.Log("Boss Health: " + boss.CharacterData.health);
            Debug.Log("Bar value: " + UIManager.BossHeathBar.valueBar.value);
            UIManager.BossHeathBar.valueBar.value = boss.CharacterData.health;
        }
    }
}