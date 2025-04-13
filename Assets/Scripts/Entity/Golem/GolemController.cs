using System.Collections;
using System.Collections.Generic;
using NodeCanvas.BehaviourTrees;
using NodeCanvas.Framework;
using UnityEngine;

public class GolemController : EntityController
{
    public Golem entity;
    public BehaviourTreeOwner behaviourTree;
    public Rect bossRoom;
    float jumpLandAttackCoolDown = 0;
    float attackCoolDown = 0;


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
        if (entity.info.stage == 1 && entity.info.health < entity.info.maxHealth / 2)
        {
            entity.info.stage = 2;
        }
        behaviourTree.Tick();
        if (jumpLandAttackCoolDown > 0)
            jumpLandAttackCoolDown -= Time.deltaTime;
        if (attackCoolDown > 0)
            attackCoolDown -= Time.deltaTime;
        if (sprintCoolDown > 0)
            sprintCoolDown -= Time.deltaTime;
        if (airMoveBackwardTimer > 0)
            airMoveBackwardTimer -= Time.deltaTime;
    }

    public override void OnStartBossFight()
    {
        
    }

    public override void SetBossRoom(Rect room)
    {
        bossRoom = room;
    }

    #region Behaviour Tree Functions

    public bool CheckPlayerFar(float distance=3f)
    {
        return Mathf.Abs(WorldManager.Player.transform.position.x - entity.transform.position.x) > distance;
    }

    // 2 for ground fllow, 0.5 for air follow
    public Status FollowPlayerNormal(float distance=2f) => FollowPlayer(distance, 1f);

    // 参数多于 1 个，不能直接用于行为树
    public Status FollowPlayer(float distance=2f, float speedScale=1f)
    {
        float diff = WorldManager.Player.transform.position.x - entity.transform.position.x;
        if (Mathf.Abs(diff) < distance)
            return Status.Success;
        float moveDirection = diff > 0 ? 1 : -1;
        if (moveDirection < 0 && entity.transform.position.x - bossRoom.xMin < 2 * distance)
            return Status.Success;
        if (moveDirection > 0 && bossRoom.xMax - entity.transform.position.x < 2 * distance)
            return Status.Success;
        entity.CurrentControl.speedX = speedScale * entity.info.moveSpeed * moveDirection;
        return Status.Running;
    }

    public Status FacePlayer()
    {
        entity.FacingDirection = WorldManager.Player.transform.position.x - entity.transform.position.x > 0 ? 1 : -1;
        return Status.Success;
    }

    public bool CheckCanAttack()
    {
        return !entity.isDead && entity.fsm.currentDeepStateName != Golem.FSM_STUN && attackCoolDown <= 0;
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
        {
            attackCoolDown = entity.info.attackCoolDownTime;
            return Status.Success;
        }
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
        if (index == 2 && entity.fsm.currentDeepStateName == Golem.FSM_ATTACK_2)
        {
            if (entity.FacingDirection * (WorldManager.Player.transform.position.x - entity.transform.position.x) > 2)
            {
                entity.CurrentControl.speedX = entity.info.moveSpeed * entity.FacingDirection;
            }
        } 
        return entity.fsm.currentDeepStateName == name ? Status.Running : Status.Success;
    }

    public bool CheckCanJumpLandAttack()
    {
        return !entity.isDead && entity.fsm.currentDeepStateName != Golem.FSM_STUN && jumpLandAttackCoolDown <= 0;
    }
    float airAimPlayerTime = 0f;
    float airNearPlayerTime = 0f;
    public Status StartJump()
    {
        airAimPlayerTime = 0;
        airNearPlayerTime = 0;
        var newState = entity.fsm.TriggerState(Golem.FSM_JUMP);
        if (newState.name == Golem.FSM_JUMP)
        {
            jumpLandAttackCoolDown = entity.info.jumpLandAttackCoolDownTime;
            return Status.Success;
        }
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

        if (airNearPlayerTime >= 0.2f || airAimPlayerTime >= 1)
        {
            return Status.Success;
        }
        FollowPlayer(0.5f, 2.5f);
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


    float sprintCoolDown = 0;
    public bool CheckCanSprint()
    {
        return !entity.isDead && entity.fsm.currentDeepStateName != Golem.FSM_STUN && sprintCoolDown <= 0 && entity.info.stage == 2;
    }
    public Status FlipFacingDirection()
    {
        entity.FacingDirection *= -1;
        return Status.Success;
    }
    float sprintTimer = 0;
    public Status StartSprint()
    {
        var newState = entity.fsm.TriggerState(Golem.FSM_SPRINT);
        if (newState.name == Golem.FSM_SPRINT)
        {
            sprintTimer = entity.info.maxSprintTime;
            sprintCoolDown = entity.info.sprintCoolDownTime;
            return Status.Success;
        }
        Debug.LogError("Failed to start sprint");    
        return Status.Failure;
    }

    public Status GiveSprintSpeedToEnd()
    {
        if (entity.fsm.currentDeepStateName != Golem.FSM_SPRINT)
        {
            return Status.Success;
        }
        sprintTimer -= Time.deltaTime;
        if ((entity.FacingDirection < 0 && entity.transform.position.x - bossRoom.xMin < 1) ||
            (entity.FacingDirection > 0 && bossRoom.xMax - entity.transform.position.x < 1) ||
            sprintTimer <= 0)
        {
            entity.CurrentControl.speedX = 0;
            if (entity.fsm.TriggerState(Golem.FSM_FREE).name == Golem.FSM_FREE)
            {
                return Status.Success;
            }
            return Status.Running;
        }
        else
        {
            entity.CurrentControl.speedX = entity.info.sprintSpeed * entity.FacingDirection;
            return Status.Running;
        }
    }

    private float airMoveBackwardTimer = 0f;
    private bool airMoveBackwardStarted = false;
    public Status AirMoveBackward(float time)
    {
        if (!airMoveBackwardStarted)
        {
            airMoveBackwardTimer = time;
            airMoveBackwardStarted = true;
        }
        if (airMoveBackwardTimer < 0)
        {
            entity.CurrentControl.speedX = 0;
            airMoveBackwardStarted = false;
            return Status.Success;
        }
        entity.CurrentControl.speedX = -entity.info.moveSpeed * 4 * entity.FacingDirection;
        return Status.Running;
    }

    #endregion
}
