using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Chase Player", story: "Chase player", category: "Action", id: "chase-player")]
public class ChasePlayer : EnemyAction
{
    [SerializeReference] public BlackboardVariable<float> MoveSpeed;

    protected override Status OnStart()
    {
        base.OnStart();
        if (player == null || rb2D == null)
        {
            return Status.Failure;
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Vector2 toPlayer = player.transform.position - enemy.transform.position;
        rb2D.linearVelocity = toPlayer.normalized * MoveSpeed.Value;
        //  if (animator != null) animator.SetBool("IsMoving", true);
        Debug.Log("We move");

        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (rb2D != null) rb2D.linearVelocity = Vector2.zero;
        //if (animator != null) animator.SetBool("IsMoving", false);
    }
}
