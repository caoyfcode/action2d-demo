
using UnityEngine;

[AddComponentMenu("Entity Component/Movement Component")]
public class MovementComponent : EntityComponent
{
    public MoveType baseMove;
    
    public Vector3 velocity;

    public override void OnEntityUpdate()
    {
    }

    public override void OnEntityFixedUpdate()
    {
    }
}

public enum HorizontalMoveType
{
    None,
    ConstantSpeed,
    Accelerate,
}

public enum VerticalMoveType
{
    None,
    Gravity,
    Accelerate,
    ConstantVelocity,
    PlayerJump,
    Hover, // 保持固定高度
    LerpPosition,
    InstantPosition,
    Spring,
}

public enum DirectionType
{
    Keep,
    SameAsVelocity,
    TargetDirection,
}



public sealed class MoveType
{
    public HorizontalMoveType horizontalMoveType;
    public VerticalMoveType verticalMoveType;
    public DirectionType directionType;
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