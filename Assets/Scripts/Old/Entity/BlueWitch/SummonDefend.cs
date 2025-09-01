using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonDefend : Summon
{
    public string animDefend = "Defend";
    public string animFX = "DefendFX";
    public string animAttack = "SuperAttack";
    public DefendBox defendBox;
    public HitBox hitBox;
    public SpriteRenderer defendFXSr;

    public AudioSource defendSFX;
    public AudioSource defendAttackSFX;
    public AudioSource hitSFX;

    public bool isDefendBoxEnabled;
    public bool isHitBoxEnabled;
    private bool prevIsHitBoxEnabled;

    protected override bool ComponentsEnabled
    {
        set
        {
            base.ComponentsEnabled = value;
            defendFXSr.enabled = value;
        } 
    }

    protected override void Update()
    {
        if (isDefendBoxEnabled)
        {
            defendBox.StartDefend();
        }
        else
        {
            defendBox.StopDefend();
        }
        if (isHitBoxEnabled)
        {
            int hitNum = hitBox.DoHit();
            if (!prevIsHitBoxEnabled)
            {
                defendAttackSFX.Play();
                GameManager.PauseForSeconds(0.1f);
            }
            if (hitNum > 0)
            {
                GameManager.PauseForSeconds(0.1f);
                defendAttackSFX.Stop();
                hitSFX.Play();
            }
        }
        prevIsHitBoxEnabled = isHitBoxEnabled;
    }

    public override void StartSummon()
    {
        ComponentsEnabled = true;
        animator.Play(animDefend);
        isDefendBoxEnabled = true;
        defendBox.onSuccess = OnDefendSuccess;
        defendBox.defendCheck = (_) => true;
        isCancelable = false;
        isEnd = false;
        hitBox.PreDiscreteHit();
    }

    public override void OnSummonEnd()
    {
        ComponentsEnabled = false;
        isDefendBoxEnabled = false;
        isHitBoxEnabled = false;
        defendBox.StopDefend();
    }

    void OnDefendSuccess(HitInfo hitInfo)
    {
        animator.Play(animFX, -1, 0);
        hitBox.genHitInfo = GenHitInfo;
        hitBox.PostDiscreteHit();
        defendSFX.Play();
        GameManager.PauseForSeconds(0.1f);
    }

    private HitInfo GenHitInfo() => new()
    {
        attacker = player,
        damage = 4,
        force = new Vector2(player.FacingDirection, 0),
        hitCollider = hitBox.hitCollider,
        hitType = HitType.EntityAttack,
    };
}