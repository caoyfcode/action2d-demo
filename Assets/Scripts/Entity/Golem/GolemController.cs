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
        return Mathf.Abs(Game.Player.transform.position.x - entity.transform.position.x) > 2f;
    }

    public Status FollowPlayer()
    {
        float diff = Game.Player.transform.position.x - entity.transform.position.x;
        if (Mathf.Abs(diff) < 1.5f)
            return Status.Success;
        entity.CurrentControl.speedX = entity.data.moveSpeed * (diff > 0 ? 1 : -1);
        return Status.Running;
    }

    public bool CheckCanAttack()
    {
        return !entity.isDead && entity.fsm.currentDeepStateName != Golem.FSM_STUN;
    }

    public Status StartAttack(int index)
    {
        string name = index switch {
            1 => Golem.FSM_ATTACK_1,
            2 => Golem.FSM_ATTACK_2,
            3 => Golem.FSM_ATTACK_3,
            _ => "",
        };
        var newState = entity.fsm.TriggerState(name);
        if (newState.name == name)
            return Status.Success;
        return Status.Failure;
    }

    public Status WaitAttackEnd(int index)
    {
        string name = index switch {
            1 => Golem.FSM_ATTACK_1,
            2 => Golem.FSM_ATTACK_2,
            3 => Golem.FSM_ATTACK_3,
            _ => "",
        };
        return entity.fsm.currentDeepStateName == name ? Status.Running : Status.Success;
    }

    #endregion
}
