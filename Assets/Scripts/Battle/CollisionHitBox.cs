using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 类似于 HitBox 类但由于逻辑差异单独分开
public class CollisionHitBox : MonoBehaviour
{
    public Collider2D body;
    public Entity bodyEntity;
    public ContactFilter2D targetFilter;
    private Collider2D[] hitResults = new Collider2D[1];

    // Start is called before the first frame update
    void Start()
    {
        targetFilter.SetLayerMask(LayerMask.GetMask("Player"));
    }

    // Update is called once per frame
    void Update()
    {
        int num = body.OverlapCollider(targetFilter, hitResults);
        if (num <= 0)
            return;
        var target = hitResults[0].GetComponentInParent<Entity>();
        if (target.IsInvincible)
            return;
        float diffX = target.transform.position.x - bodyEntity.transform.position.x;
        float directionX = Mathf.Abs(diffX) > Mathf.Epsilon ? Mathf.Sign(diffX) : -target.FacingDirection;
        target.OnAttacked(new HitInfo()
        {
            attacker = bodyEntity,
            damage = 1,
            force = new Vector2(directionX * 5, 0),
            hitBox = body,
            hurtBox = hitResults[0],
            hitType = HitType.BodyCollision,
        });
    }
}
