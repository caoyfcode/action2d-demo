using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireKnightController : EntityController
{
    FireKnight entity;

    public override void Init()
    {
        entity = GetComponent<FireKnight>();
    }

    public override void OnAttacked(HitInfo hitInfo)
    {
        
    }

    public override void OnDeath()
    {
        
    }

    public override void UpdateCommand()
    {
        var cmd = new FireKnight.Command();
        entity.SetCommand(cmd);
    }
}
