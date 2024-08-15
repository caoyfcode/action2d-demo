using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonProjectile : MonoBehaviour
{
    public BlueWitch parent;

    public GameObject batPrefab;

    public void StartSummon()
    {
        GameObject batObj = Instantiate(batPrefab, transform.position, Quaternion.identity);
        Projectile bat = batObj.GetComponent<Projectile>();
        bat.velocity = (parent.FacingDirection > 0 ? new Vector2(1, 0) : new Vector2(-1, 0)) * 30;
        bat.hitBox.source = parent;
        bat.hitBox.targetFilter.SetLayerMask(LayerMask.GetMask("Enemy") | LayerMask.GetMask("Environment"));
    }
}
