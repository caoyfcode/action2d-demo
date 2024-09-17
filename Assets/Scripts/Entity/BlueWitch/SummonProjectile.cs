using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SummonProjectile : Summon
{
    public GameObject projectilePrefab;
    public GameObject projectPoint;
    public bool startProject; // 由动画状态机控制(先尝试仅用这个变量来做)
    protected bool projectOver; // 射击后设置为 true, 避免多次设计

    public override void StartSummon()
    {
        base.StartSummon();
        startProject = false;
        projectOver = false;
    }

    protected override void Update()
    {
        base.Update();
        if (startProject && !projectOver && Game.Player.status.magic > 0.1f)
        {
            GameObject obj = Instantiate(projectilePrefab);
            obj.transform.position = projectPoint.transform.position;
            Projectile projectile = obj.GetComponent<Projectile>();
            projectile.velocity = (Game.Player.FacingDirection > 0 ? new Vector2(1, 0) : new Vector2(-1, 0)) * 30;
            projectile.damage = 0.5f;
            projectile.source = Game.Player;
            projectile.hitBox.targetFilter.SetLayerMask(LayerMask.GetMask("Enemy"));
            projectOver = true;
            Game.Player.status.magic -= 0.1f;
        }
    }
}
