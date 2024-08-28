using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SummonProjectile : Summon
{
    public GameObject projectilePrefab;

    public override void StartSummon()
    {
        base.StartSummon();
        GameObject obj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile projectile = obj.GetComponent<Projectile>();
        projectile.velocity = (Game.Player.FacingDirection > 0 ? new Vector2(1, 0) : new Vector2(-1, 0)) * 30;
        projectile.damage = 0.5f;
        projectile.source = Game.Player;
        projectile.hitBox.targetFilter.SetLayerMask(LayerMask.GetMask("Enemy") | LayerMask.GetMask("Environment"));
    }
}
