using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SummonAttack : Summon
{
    public Collider2D[] hitBoxes;
    public HitBox hitBox;
    public bool isHitBoxEnabled = false;
    public AudioSource attackSFX;
    public AudioSource attackHitSFX;
    private bool prevIsHitBoxEnabled = false;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (isHitBoxEnabled)
        {
            int hitMum = hitBox.DoHit();
            if (!prevIsHitBoxEnabled)
            {
                attackSFX.pitch = Random.Range(0.8f, 1.2f);
                attackSFX.Play();
            }
            if (hitMum > 0)
            {
                attackHitSFX.pitch = Random.Range(0.8f, 1.2f);
                attackHitSFX.Play();
                attackSFX.Stop();
                GameManager.PauseForSeconds(0.1f);
            }
        }
        prevIsHitBoxEnabled = isHitBoxEnabled;
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
        attacker = player,
        damage = 1,
        force = new Vector2(player.FacingDirection, 0),
        hitCollider = hitBox.hitCollider,
        hitType = HitType.EntityAttack,
    };
}
