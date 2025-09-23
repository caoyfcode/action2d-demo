
using System;
using UnityEngine;

[AddComponentMenu("Entity Component/Animator Component")]
public class AnimatorComponent : EntityComponent
{
    public Animator animator;

    public override void OnEntityAwake()
    {
        if (animator == null)
        {
            animator = entity.GetComponentInChildren<Animator>();
        }
    }

    public void ChangeAnimation(string name)
    {
        animator.Play(name);
    }
}