
using UnityEngine;

[AddComponentMenu("Entity Component/Move Component")]
public class MoveComponent : EntityComponent
{
    public Rigidbody2D rb; // rigidbody, 不过不一定有
    
    [Header("Settings")]
    public float gravity = -30;
    public Vector2 velocityMin;
    public Vector2 velocityMax;
    
    [Header("Base Movement")]
    public MoveType baseMoveType;
    public Vector2 setVelocity;
    public float setDirection;
    public Vector2 setAccelerate;


    private Vector2 baseVelocity;

    private float cachedDirection = 1;
    public float CurrentDirection => cachedDirection;
    public Vector2 FinalVelocity => baseVelocity;
    
    // TODO 添加 SurroundingsDetectComponent, 其中可以选择性检测哪里, 每一帧检测结果
    
    public override void OnEntityAwake()
    {
        rb = GetComponent<Rigidbody2D>();
        cachedDirection = transform.right.x > 0 ? 1 : -1;
    }

    public override void OnEntityUpdate()
    {
        float timeScale = 1f;
        float scaledDeltaTime = timeScale * Time.deltaTime;
        switch (baseMoveType.horizontalMoveType)
        {
            case HorizontalMoveType.None:
                baseVelocity.x = 0f;
                break;
            case HorizontalMoveType.Speed:
                baseVelocity.x = setVelocity.x;
                break;
            case HorizontalMoveType.Accelerate:
                baseVelocity.x += setAccelerate.x * scaledDeltaTime;
                break;
        }

        switch (baseMoveType.verticalMoveType)
        {
            case VerticalMoveType.None:
                baseVelocity.y = 0f;
                break;
            case VerticalMoveType.Speed:
                baseVelocity.y = setVelocity.y;
                break;
            case VerticalMoveType.Gravity:
                baseVelocity.y += gravity * scaledDeltaTime;
                break;
            case VerticalMoveType.Accelerate:
                baseVelocity.y += setAccelerate.y * scaledDeltaTime;
                break;
        }

        var finalDirection = cachedDirection;
        switch (baseMoveType.directionType)
        {
            case DirectionType.Keep:
                break;
            case DirectionType.SameAsVelocity:
                if (baseVelocity.x != 0)
                {
                    finalDirection = baseVelocity.x > 0 ? 1 : -1;
                }
                break;
            case DirectionType.Direction:
                finalDirection = setDirection;
                break;
        }
        
        ApplyVelocity(baseVelocity);
        ApplyDirection(finalDirection);
    }

    public override void OnEntityFixedUpdate()
    {
        float timeScale = 1f;
        
    }

    private void ApplyVelocity(Vector2 velocity, float deltaTime=0.02f)
    {
        if (rb != null)
        {
            rb.velocity = velocity;
        }
        else
        {
            transform.position += new Vector3(velocity.x, velocity.y, 0f) * deltaTime;
        }
    }

    private void ApplyDirection(float direction)
    {
        if (direction * cachedDirection >= 0)
        {
            return;
        }
        if (direction > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            cachedDirection = 1;
        }
        else if (direction < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            cachedDirection = -1;
        }
    }
}

public enum HorizontalMoveType
{
    None,
    Speed,
    Accelerate,
}

public enum VerticalMoveType
{
    None,
    Speed,
    Gravity,
    Accelerate,
    // PlayerJump,
    // Hover, // 保持固定高度
    // LerpPosition,
    // InstantPosition,
    // Spring,
}

public enum DirectionType
{
    Keep,
    SameAsVelocity,
    Direction,
}


[System.Serializable]
public sealed class MoveType
{
    public static readonly MoveType None = new MoveType(HorizontalMoveType.None,  VerticalMoveType.None, DirectionType.Keep);
    public static readonly MoveType Horizontal = new MoveType(HorizontalMoveType.Speed, VerticalMoveType.None, DirectionType.SameAsVelocity);
    
    public HorizontalMoveType horizontalMoveType;
    public VerticalMoveType verticalMoveType;
    public DirectionType directionType;

    public MoveType(HorizontalMoveType horizontalMoveType=HorizontalMoveType.None, VerticalMoveType verticalMoveType=VerticalMoveType.None, DirectionType directionType=DirectionType.Keep)
    {
        this.horizontalMoveType = horizontalMoveType;
        this.verticalMoveType = verticalMoveType;
        this.directionType = directionType;
    }
}

/*
 * 几个想法:
 * 1. 将项目中的各种回调改为事件系统驱动, 如 UI 事件、攻击事件、
 * 2. 将项目中的各种参数(如实体、攻击、特效等)改为由数据驱动，SO类型定义各种Config
 * 3. 将原本基于继承的 Entity 修改为基于组件组合的，为了解决组合时不同组件先后配合问题，添加两种机制
 *      3.1 将 Entity 的各种 Update 分为更多阶段, 如 Pre/On/Post, 保证各组件在 tick 时需要读取状态、计算逻辑、修改状态的顺序
 *      3.2 将所有组件的 Tick 排序
 *    同时，所有 entity
 * 4. 将采用 Resources 的加载方法改为 AssetBundle
 * 5. 将攻击判断 帧攻击、连续攻击(包含内部数量、内部频率、总数量), 并可以添加定帧行为
 */