using System.Collections;
using System.Collections.Generic;
using NodeCanvas.BehaviourTrees;
using NodeCanvas.Framework;
using UnityEngine;

public class GolemController : EntityController
{
    public Golem entity;
    public BehaviourTreeOwner behaviourTree;


    void Awake()
    {
        if (!entity)
            entity = GetComponent<Golem>();
        if (!behaviourTree)
            behaviourTree = GetComponent<BehaviourTreeOwner>();
    }

    public override void Init()
    {
    }

    public override void UpdateCommand()
    {
        behaviourTree.Tick();
    }

    public override void OnStartBossFight()
    {
        
    }

    #region Behaviour Tree Functions

    public bool CheckPlayerFar()
    {
        return Mathf.Abs(Game.Player.transform.position.x - entity.transform.position.x) > 5f;
    }

    public Status FollowPlayer()
    {
        float diff = Game.Player.transform.position.x - entity.transform.position.x;
        if (Mathf.Abs(diff) < 2f)
            return Status.Success;
        entity.CurrentControl.speedX = entity.data.moveSpeed * (diff > 0 ? 1 : -1);
        return Status.Running;
    }

    #endregion
}
