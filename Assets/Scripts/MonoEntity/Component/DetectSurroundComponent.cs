using UnityEngine;

public class DetectSurroundComponent : EntityComponent
{
    [Header("Detect Control")]
    public bool DetectWall = true;
    public bool DetectGround = true;
    public bool DetectCeiling = true;
    public bool DetectOverlap = false;
    // public bool DetectOtherBody;
    
    public LayerMask envMask;
    // public LayerMask bodyMask;
    // TODO 阵营 tag
    public float envDetectDistance = 0.1f;

    public Collider2D bodyCollider; // TODO 改为自定义 DetectPattern, 可以用 collider, 也可以用定义的 shape
    private ContactFilter2D contactFilter = new();
    private RaycastHit2D[] hits = new RaycastHit2D[5];
    private Collider2D[] overlaps = new Collider2D[5];
    
    public bool IsOnGround { get; private set; }
    public float GroundDistance { get; private set; }
    public bool IsOnCeiling { get; private set; }
    public float CeilingDistance { get; private set; }
    public bool IsOnLeftWall { get; private set; }
    public float LeftWallDistance { get; private set; }
    public bool IsOnRightWall { get; private set; }
    public float RightWallDistance { get; private set; }
    public bool IsOverlapEnvironment {get; private set;}

    public bool IsOnDirectionWall(float direction) => direction > 0 ? IsOnRightWall : IsOnLeftWall;
    public bool IsFacingWall => entity.HasEntityComponent<MoveComponent>() && IsOnDirectionWall(entity.GetEntityComponent<MoveComponent>().CurrentDirection);
    public bool IsMovingToWall => entity.HasEntityComponent<MoveComponent>() && IsOnDirectionWall(entity.GetEntityComponent<MoveComponent>().FinalVelocity.x);
    
    public override void OnEntityAwake()
    {
        contactFilter.useTriggers = true;
        contactFilter.useLayerMask = true;
        contactFilter.SetLayerMask(envMask);
        if (bodyCollider == null)
            bodyCollider = GetComponent<Collider2D>();
    }

    public override void OnEntityUpdate()
    {
        if (bodyCollider == null)
            return;
        if (DetectWall)
        {
            (IsOnRightWall, RightWallDistance) = CastCollider(bodyCollider, Vector2.right, contactFilter, hits, envDetectDistance);
            (IsOnLeftWall, LeftWallDistance) = CastCollider(bodyCollider, Vector2.left, contactFilter, hits, envDetectDistance);
        }

        if (DetectGround)
        {
            (IsOnGround, GroundDistance) = CastCollider(bodyCollider, Vector2.down, contactFilter, hits, envDetectDistance);
        }

        if (DetectCeiling)
        {
            (IsOnCeiling, CeilingDistance) = CastCollider(bodyCollider, Vector2.up, contactFilter, hits, envDetectDistance);
        }
        
        if (DetectOverlap)
        {
            IsOverlapEnvironment = bodyCollider.OverlapCollider(contactFilter, overlaps) > 0;
        }
    }
    
    private (bool, float) CastCollider(Collider2D col, Vector2 dir, ContactFilter2D filter, RaycastHit2D[] results, float distance)
    {
        int cnt = col.Cast(dir, filter, results, distance);
        float resultDistance = float.MaxValue;
        if (cnt > 0)
        {
            for (int i = 0; i < cnt; i++)
            {
                if (results[i].distance < resultDistance)
                {
                    resultDistance = results[i].distance;
                }
            }

            return (true, resultDistance);
        }
        return (false, resultDistance);
    }
}