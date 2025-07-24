using System;
using UnityEngine;

public class BossSystem : MonoBehaviour
{
    public Entity boss;
    public bool bossFightStarted = false;

    private void Awake()
    {
        EventManager.Instance.Register<EvtStartBossFight>(StartBossFight);
        EventManager.Instance.Register<EvtStopBossFight>(StopBossFight);
    }

    void StartBossFight(EvtStartBossFight evt)
    {
        if (!bossFightStarted && evt.boss != null)
        {
            boss = evt.boss;
            bossFightStarted = true;
            UIManager.BossHeathBar.valueBar.maxValue = boss.CharacterInfo.maxHealth;
            UIManager.BossHeathBar.valueBar.value = boss.CharacterInfo.health;
            UIManager.BossHeathBar.Shown = true;
            boss.controller.SetBossRoom(evt.bossRoom);
            boss.controller.OnStartBossFight();
        }
    }

    void StopBossFight(EvtStopBossFight evt)
    {
        if (bossFightStarted)
        {
            bossFightStarted = false;
            UIManager.BossHeathBar.Shown = false;
            boss = null;
        }
    }

    void Update()
    {
        if (bossFightStarted && boss != null)
        {
            Debug.Log("Boss Health: " + boss.CharacterInfo.health);
            Debug.Log("Bar value: " + UIManager.BossHeathBar.valueBar.value);
            UIManager.BossHeathBar.valueBar.value = boss.CharacterInfo.health;
        }
        if (bossFightStarted && boss == null)
        {
            EventManager.Instance.TriggerEvent(new EvtStopBossFight());
        }
    }
}