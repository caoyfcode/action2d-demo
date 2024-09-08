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
    public bool isDefendBoxEnabled;
    public bool isHitBoxEnabled;
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
        ComponentsEnabled = true;
        animator.Play(animDefend);
        isDefendBoxEnabled = true;
        defendBox.onSuccess = OnDefendSuccess;
        isCancelable = false;
        isEnd = false;
    }

    public override void OnSummonEnd()
    {
        ComponentsEnabled = false;
        isDefendBoxEnabled = false;
        isHitBoxEnabled = false;
        defendBox.StopDefend();
        hitBox.StopHit();
    }

    void OnDefendSuccess(HitInfo hitInfo)
    {
        animator.Play(animFX, -1, 0);
        hitBox.genHitInfo = GenHitInfo;
    }

    private HitInfo GenHitInfo() => new()
    {
        attacker = Game.Player,
        damage = 4,
        force = new Vector2(Game.Player.FacingDirection, 0),
        hitBox = hitBox.hitBox,
        hitType = HitType.EntityAttack,
    };
}