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
            hitBox.DoHit();
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
        hitBox.hitCollider = hitBoxes[nextIndex];
        nextIndex = (nextIndex + 1) % animNames.Length;
        isCancelable = true;
        isEnd = false;
        isHitBoxEnabled = false;
        if (hitBox.genHitInfo == null)
        {
            hitBox.genHitInfo = GenHitInfo;
        }
        hitBox.PreDiscreteHit();
    }

    public override void OnSummonEnd()
    {
        base.OnSummonEnd();
        isHitBoxEnabled = false;
        hitBox.PostDiscreteHit();
    }

    private HitInfo GenHitInfo() => new()
    {
        attacker = Game.Player,
        damage = 1,
        force = new Vector2(Game.Player.FacingDirection, 0),
        hitCollider = hitBox.hitCollider,
        hitType = HitType.EntityAttack,
    };
}
