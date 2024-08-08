using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summon : MonoBehaviour
{
    public BlueWitch parent;

    public GameObject batPrefab;

    public void StartSummon()
    {
        GameObject batObj = Instantiate(batPrefab, transform.position, Quaternion.identity);
        SummonBat bat = batObj.GetComponent<SummonBat>();
        bat.direction = parent.FacingDirection > 0 ? new Vector2(1, 0) : new Vector2(-1, 0);
        bat.source = parent;
        bat.targetCastFilter.SetLayerMask(LayerMask.GetMask("Enemy") | LayerMask.GetMask("Environment"));
    }
}
