using System.Collections;
using System.Collections.Generic;
using NodeCanvas.BehaviourTrees;
using NodeCanvas.Framework;
using UnityEngine;

public class GolemController : EntityController
{
    public Golem entity;
    public BehaviourTreeOwner behaviourTree;
    float jumpLandAttackCoolDown = 0;


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
        if (jumpLandAttackCoolDown > 0)
            jumpLandAttackCoolDown -= Time.deltaTime;
    }

    public override void OnStartBossFight()
    {
        
    }

    #region Behaviour Tree Functions

    public bool CheckPlayerFar(float distance=3f)
    {
        return Mathf.Abs(Game.Player.transform.position.x - entity.transform.position.x) > distance;
    }

    public Status FollowPlayerNormal(float distance=2f) => FollowPlayer(distance, 1f);

    // 2 for ground fllow, 0.5 for air follow
    // 参数多于 1 个，不能直接用于行为树
    public Status FollowPlayer(float distance=2f, float speedScale=1f)
    {
        float diff = Game.Player.transform.position.x - entity.transform.position.x;
        if (Mathf.Abs(diff) < distance)
            return Status.Success;
        entity.CurrentControl.speedX = speedScale * entity.data.moveSpeed * (diff > 0 ? 1 : -1);
        return Status.Running;
    }

    public Status FacePlayer()
    {
        entity.FacingDirection = Game.Player.transform.position.x - entity.transform.position.x > 0 ? 1 : -1;
        return Status.Success;
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

    float airAimPlayerTime = 0f;
    float airNearPlayerTime = 0f;
    public Status StartJump()
    {
        airAimPlayerTime = 0;
        airNearPlayerTime = 0;
        var newState = entity.fsm.TriggerState(Golem.FSM_JUMP);
        if (newState.name == Golem.FSM_JUMP)
            return Status.Success;
        return Status.Failure;
    }

    public Status AirAimPlayer()
    {
        airAimPlayerTime += Time.deltaTime;
        if (CheckPlayerFar(1f))
        {
            airNearPlayerTime = 0f;
        }
        else
        {
            airNearPlayerTime += Time.deltaTime;
        }

        // 瞄准到了足够长时间或者timeout, 结束瞄准状态
        if (airNearPlayerTime >= 0.2f || airAimPlayerTime >= 1f)
        {
            return Status.Success;
        }
        FollowPlayer(0.5f, 2);
        return Status.Running;
    }

    public Status StartLand()
    {
        var newState = entity.fsm.TriggerState(Golem.FSM_LAND_ATTACK);
        if (newState.name == Golem.FSM_LAND_ATTACK)
            return Status.Success;
        return Status.Failure;
    }

    public Status WaitStateEnd(string name)
    {
        return entity.fsm.currentDeepStateName == name ? Status.Running : Status.Success;
    }

    public bool RandomBool(float p) => Random.Range(0f, 1f) < p;

    #endregion
}
