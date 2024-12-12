using System;
using UnityEngine;
using UnityEngine.Events;

// Entity 类为所有实体的基类, 可以利用 velocity 改变速度
// 必有: Animator, Rigidbody2D, Collider2D
// 可选: HitFlashFX
public class Entity : MonoBehaviour
{
    #region Events
    public UnityEvent<HitInfo> onTakeHit;
    public UnityEvent onDeath;
    public UnityEvent<HitInfo> onHitSuccess;
    #endregion

    #region Constant
    internal float gravity = 30f;
    internal float forceSpeedDecay = 10f; // 水平衰减速度
    internal float forceSpeed = 5f; // 单位力施加到单位质量产生的速度
    internal float minForceSpeed = 0.1f; // 水平衰减的最小值, 到达此值设为 0
    #endregion

    #region Component
    internal Animator animator;
    protected Rigidbody2D rb;
    internal Collider2D col;
    protected EntityFX entityFX;
    protected EntityController controller;
    #endregion

    #region Status
    [Header("Status")]
    public Vector2 velocity; // 自身状态确定的速度
    public float forceMove; // 水平方向速度中会衰减的部分, 用于实现瞬间受力
    protected float _facingDirection = 1; // 默认向右
    public float FacingDirection
    {
        get => _facingDirection;
        set
        {
            if (value * _facingDirection < 0)
            {
                rb.transform.Rotate(0, 180, 0);
                _facingDirection = -_facingDirection;
            }
        }
    }
    public bool isOnGround;
    internal bool isOnCeiling;
    internal bool isOnLeftWall = false;
    internal bool isOnRightWall = false;
    internal bool isOverlapEnvironment = false;
    internal bool isOnLeftCliff = false; // 悬崖检测距离并非贴着，而是距离为 1
    internal bool isOnRightCliff = false;
    // 之所以两边都检测是因为更新过程方向会变
    public bool IsFacingWall => FacingDirection > 0 ? isOnRightWall : isOnLeftWall;
    public bool IsFacingCliff => FacingDirection > 0 ? isOnRightCliff : isOnLeftCliff;
    public bool isDead = false;
    // Invincible 代表攻击不起作用，不仅仅是不扣血
    public virtual bool IsInvincible => isDead;
    public virtual CharacterData CharacterData => null;
    #endregion

    [Header("Effects")]
    #region  Effects
    public bool flashFXOnHit = true;
    public bool bloodFXOnHit = true;
    public bool explosionOnDestory = true;
    public bool darkSpriteOnDestory = false;
    #endregion

    protected ContactFilter2D envCastFilter;
    private RaycastHit2D[] castHits = new RaycastHit2D[5];
    private Collider2D[] overlapHits = new Collider2D[5];


    protected virtual void Awake()
    {
        if (!entityFX)
        {
            entityFX = GetComponentInChildren<EntityFX>();
        }
        if (!animator)
        {
            animator = GetComponent<Animator>();
        }
        if (!rb)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        if (!col)
        {
            col = GetComponent<Collider2D>();
        }
        if (!controller)
        {
            controller = GetComponent<EntityController>();
        }
    }

    protected virtual void Start()
    {
        envCastFilter.SetLayerMask(LayerMask.GetMask("Environment"));
        if (controller)
        {
            controller.Init();
        }
        _facingDirection = transform.right.x > 0 ? 1 : -1; // 向右为正
    }

    protected virtual void Update()
    {
        isOnGround = col.Cast(Vector2.down, envCastFilter, castHits, 0.1f) > 0;
        isOnCeiling = col.Cast(Vector2.up, envCastFilter, castHits, 0.1f) > 0;
        isOnRightWall = col.Cast(Vector2.right, envCastFilter, castHits, 0.1f) > 0;
        isOnLeftWall = col.Cast(Vector2.left, envCastFilter, castHits, 0.1f) > 0;
        isOverlapEnvironment = col.OverlapCollider(envCastFilter, overlapHits) > 0;
        // 悬崖条件: 在地面 && 朝向无地面 && 朝向无墙壁, 之所以检测墙壁是因为检测前方地面的起始点可能在墙壁内部
        isOnLeftCliff = isOnGround && Physics2D.Raycast(new Vector2(col.bounds.min.x - 1, col.bounds.min.y), Vector2.down, envCastFilter, castHits, 0.5f) == 0 && Physics2D.Raycast(new Vector2(col.bounds.min.x, col.bounds.center.y), Vector2.left, envCastFilter, castHits, 1f) == 0;
        isOnRightCliff = isOnGround && Physics2D.Raycast(new Vector2(col.bounds.max.x + 1, col.bounds.min.y), Vector2.down, envCastFilter, castHits, 0.5f) == 0 && Physics2D.Raycast(new Vector2(col.bounds.min.x, col.bounds.center.y), Vector2.right, envCastFilter, castHits, 1f) == 0;
        float prev = forceMove > 0 ? 1 : -1;
        forceMove *= 1 - forceSpeedDecay * Time.deltaTime; // delta_v = - k v delta_t / m => v' = v * (1 - kk * delta_t)
        if (prev * forceMove <= minForceSpeed) // 速度过小或反向, 则设置为 0
        {
            forceMove = 0;
        }

        if (controller)
        {
            controller.UpdateCommand();
        }
    }

    // 在 Update 末设置 rigidbody 速度前调用
    protected void MergeVelocity()
    {
        // 严格用 0 判断, 因为会设置为 0
        if (forceMove != 0)
        {
            velocity.x = forceMove;
        }
        // 撞墙后停止
        if (velocity.x > 0 && isOnRightWall || velocity.x < 0 && isOnLeftWall)
        {
            velocity.x = 0;
            forceMove = 0;
        }
    }

    protected void AddForce(Vector2 force)
    {
        Vector2 deltaV = force * forceSpeed / rb.mass;
        forceMove += deltaV.x;
        var vel = rb.velocity;
        vel.x += deltaV.x;
        vel.y += deltaV.y; // TIP: 竖直方向若处于持续设置为 0 的状态，则无法生效
        rb.velocity = vel;
    }

    public virtual void HitSuccess(HitInfo hitInfo)
    {
        Debug.Log(gameObject.name + " hits successfully.");
        onHitSuccess.Invoke(hitInfo);
    }

    public virtual bool TakeHitCheck(HitInfo hitInfo) => !IsInvincible;

    public virtual void TakeHit(HitInfo hitInfo)
    {
        Debug.Log(gameObject.name + " takes hit.");
        if (entityFX)
        {
            if (flashFXOnHit)
                entityFX.StartFlash();
            if (bloodFXOnHit)
                entityFX.StartBloodFX(hitInfo.hitPoint, hitInfo.force);
        }
        
        onTakeHit.Invoke(hitInfo);
    }

    // 死亡, 子类一般重写此类进入死亡动画状态，在死亡状态机中真正Destroy
    public virtual void Die()
    {
        Debug.Log(gameObject.name + " dies.");
        col.enabled = false;
        isDead = true;
        onDeath.Invoke();
    }

    // TIP Entity 的删除不适用 Destory 函数，而是 DestoryEntity
    public void DestoryEntity()
    {
        if (entityFX)
        {   
            if (explosionOnDestory)
                entityFX.StartDeathFX(col.bounds.center);
            if (darkSpriteOnDestory)
                entityFX.GenDarkSprite();
        }
        Destroy(gameObject);
    }
}