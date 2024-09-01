using UnityEngine;

// Entity 类为所有实体的基类, 可以利用 velocity 改变速度
// 必有: Animator, Rigidbody2D, Collider2D
// 可选: HitFlashFX
[RequireComponent(typeof(Animator), typeof(Rigidbody2D), typeof(Collider2D))]
public class Entity : MonoBehaviour
{
    #region Constant
    internal float horizontalMoveSpeed = 6f;
    internal float gravity = 15f;
    internal float forceMoveDecay = 10f; // 水平衰减速度
    internal float attackForceMove = 5f; // 单位力施加到单位质量产生的速度
    internal float minForceMove = 0.1f; // 水平衰减的最小值, 到达此值设为 0
    #endregion

    #region Component
    internal Animator animator;
    protected Rigidbody2D rb;
    internal Collider2D col;
    protected HitFlashFX hitFX;
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
        protected set
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
    public bool isDead = false;
    // 之所以两边都检测是因为更新过程方向会变
    public bool IsFacingWall => FacingDirection > 0 ? isOnRightWall : isOnLeftWall;
    public virtual bool IsInvincible => false;
    #endregion

    protected ContactFilter2D envCastFilter;
    private RaycastHit2D[] castHits = new RaycastHit2D[5];
    private Collider2D[] overlapHits = new Collider2D[5];

    protected virtual void Start()
    {
        hitFX = GetComponent<HitFlashFX>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        controller = GetComponent<EntityController>();
        envCastFilter.SetLayerMask(LayerMask.GetMask("Environment"));
        if (controller)
        {
            controller.Init();
        }
    }

    protected virtual void Update()
    {
        isOnGround = col.Cast(Vector2.down, envCastFilter, castHits, 0.1f) > 0;
        isOnCeiling = col.Cast(Vector2.up, envCastFilter, castHits, 0.1f) > 0;
        isOnRightWall = col.Cast(Vector2.right, envCastFilter, castHits, 0.1f) > 0;
        isOnLeftWall = col.Cast(Vector2.left, envCastFilter, castHits, 0.1f) > 0;
        isOverlapEnvironment = col.OverlapCollider(envCastFilter, overlapHits) > 0;
        float prev = forceMove > 0 ? 1 : -1;
        forceMove *= 1 - forceMoveDecay * Time.deltaTime; // delta_v = - k v delta_t / m => v' = v * (1 - kk * delta_t)
        if (prev * forceMove <= minForceMove) // 速度过小或反向, 则设置为 0
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

    public void HorizontalMoveAndFlip(float moveX)
    {
        velocity.x = 0;
        if (moveX != 0)
        {
            FacingDirection = moveX > 0 ? 1 : -1;
            velocity.x = moveX * horizontalMoveSpeed;
        }
    }

    public virtual void OnAttackSuccess(HitInfo hitInfo)
    {
        Debug.Log(gameObject.name + " attack success");
    }

    public virtual void OnAttacked(HitInfo hitInfo)
    {
        Debug.Log(gameObject.name + " is attacked.");
        if (hitFX)
        {
            hitFX.StartFlash();
        }
        Vector2 deltaV = hitInfo.force * attackForceMove / rb.mass;
        forceMove += deltaV.x;
        var vel = rb.velocity;
        vel.x += deltaV.x;
        vel.y += deltaV.y; // TIP: 竖直方向若处于持续设置为 0 的状态，则无法生效
        rb.velocity = vel;
        // velocity.y += deltaV.y;
        if (controller)
        {
            controller.OnAttacked(hitInfo);
        }
    }

    public virtual void OnDeath()
    {
        Debug.Log(gameObject.name + " dies.");
        col.enabled = false;
        isDead = true;
        if (controller)
        {
            controller.OnDeath();
        }
    }
}