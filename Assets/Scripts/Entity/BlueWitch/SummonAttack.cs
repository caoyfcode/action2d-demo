using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonAttack : MonoBehaviour
{
    public string AnimAttack1 = "HeroKnightAttack_1";
    public string AnimAttack2 = "HeroKnightAttack_2";
    
    public SpriteRenderer sr;
    public Animator animator;
    public Collider2D hitBoxAttack1;
    public Collider2D hitBoxAttack2;
    public HitBox hitBox;

    public Collider2D CurrentHitBox
    {
        get => hitBox.hitBox; 
        private set => hitBox.hitBox = value;
    }

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
        CurrentHitBox = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (attackComboTimer > 0)
        {
            attackComboTimer -= Time.deltaTime;
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

    public void StartAttack()
    {
        ComponentsEnabled = true;
        if (CurrentHitBox == null || attackComboTimer <= 0 || CurrentHitBox == hitBoxAttack2)
        {
            CurrentHitBox = hitBoxAttack1;
            animator.Play(AnimAttack1);
        }
        else
        {
            CurrentHitBox = hitBoxAttack2;
            animator.Play(AnimAttack2);
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
        attackComboTimer = Game.Player.attackComboTime;
        hitBox.StopHit();
    }
}
