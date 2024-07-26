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
    #endregion

    #region Component
    internal Animator animator;
    protected Rigidbody2D rb;
    protected Collider2D col;
    protected HitFlashFX hitFX;
    #endregion

    #region Status
    [Header("Status")]
    public Vector2 velocity;
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
    public bool isDead = false;
    // 之所以两边都检测是因为更新过程方向会变
    public bool IsFacingWall => FacingDirection > 0 ? isOnRightWall : isOnLeftWall;
    #endregion

    protected ContactFilter2D envCastFilter;
    private RaycastHit2D[] castHits = new RaycastHit2D[5];

    protected virtual void Start()
    {
        hitFX = GetComponent<HitFlashFX>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        envCastFilter.SetLayerMask(LayerMask.GetMask("Environment"));
    }

    protected virtual void Update()
    {
        if (col)
        {
            isOnGround = col.Cast(Vector2.down, envCastFilter, castHits, 0.1f) > 0;
            isOnCeiling = col.Cast(Vector2.up, envCastFilter, castHits, 0.1f) > 0;
            isOnRightWall = col.Cast(Vector2.right, envCastFilter, castHits, 0.1f) > 0;
            isOnLeftWall = col.Cast(Vector2.left, envCastFilter, castHits, 0.1f) > 0;
        }
    }

    protected virtual void FixedUpdate()
    {
        // 遇到墙不再前进，避免与墙重叠导致错误检测到地面
        if (IsFacingWall && rb.velocity.x * FacingDirection > 0)
        {
            var velocity = rb.velocity;
            velocity.x = 0;
            rb.velocity = velocity;
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

    public virtual void OnAttacked()
    {
        Debug.Log(gameObject.name + " is attacked.");
        if (hitFX)
        {
            hitFX.StartFlash();
        }
    }

    public virtual void onDeath()
    {
        Debug.Log(gameObject.name + " dies.");
        if (col)
        {
            col.enabled = false;
        }
        isDead = true;
    }
}