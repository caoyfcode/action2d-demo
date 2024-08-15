using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SummonAttack : Summon
{
    public Collider2D[] hitBoxes;
    public HitBox hitBox;
    public bool isHitBoxEnabled = false;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (isHitBoxEnabled)
        {
            hitBox.StartHit();
            hitBox.DoHit();
        }
        else
        {
            hitBox.StopHit();
        }
    }

    public override void StartSummon()
    {
        if (comboTimer <= 0)
        {
            nextIndex = 0;
        }
        ComponentsEnabled = true;
        animator.Play(animNames[nextIndex]);
        hitBox.hitBox = hitBoxes[nextIndex];
        nextIndex = (nextIndex + 1) % animNames.Length;
        isCancelable = true;
        isEnd = false;
        isHitBoxEnabled = false;
    }

    public override void OnSummonEnd()
    {
        base.OnSummonEnd();
        isHitBoxEnabled = false;
        hitBox.StopHit();
    }
}
