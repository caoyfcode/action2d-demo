using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonAttack : MonoBehaviour
{
    public const string ANIM_HERO_KNIGHT_ATTACK_1 = "HeroKnightAttack_1";
    public const string ANIM_HERO_KNIGHT_ATTACK_2 = "HeroKnightAttack_2";

    public BlueWitch parent;
    public SpriteRenderer sr;
    public Animator animator;
    public Collider2D hitBoxHeroKnightAttack1;
    public Collider2D hitBoxHeroKnightAttack2;
    public Collider2D CurentHitBox { get; private set; }

    #region Status
    public bool isEnd = false;
    public bool isCancelable = true;
    public bool isHitBoxEnabled = false;
    public float attackComboTimer = 0;
    #endregion

    bool ComponentsEnabled 
    {
        set
        {
            animator.enabled = value;
            sr.enabled = value; 
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ComponentsEnabled = false;
        CurentHitBox = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (attackComboTimer > 0)
        {
            attackComboTimer -= Time.deltaTime;
        }
    }

    public void StartAttack()
    {
        ComponentsEnabled = true;
        if (CurentHitBox == null || attackComboTimer <= 0 || CurentHitBox == hitBoxHeroKnightAttack2)
        {
            CurentHitBox = hitBoxHeroKnightAttack1;
            animator.Play(ANIM_HERO_KNIGHT_ATTACK_1);
        }
        else
        {
            CurentHitBox = hitBoxHeroKnightAttack2;
            animator.Play(ANIM_HERO_KNIGHT_ATTACK_2);
        }
        isCancelable = true;
        isEnd = false;
        isHitBoxEnabled = false;
    }

    // 由状态机调用
    public void OnAttackEnd()
    {
        isEnd = true;
        isHitBoxEnabled = false;
        ComponentsEnabled = false;
        attackComboTimer = parent.attackComboTime;
    }
}
