using System;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Patrol around waypoints", story: "[BlackboardEnemy] patrols around [waypoints]", category: "Action", id: "patrol-around-waypoints")]
public partial class PatrolAroundWaypoints : EnemyAction
{
    [SerializeReference] public BlackboardVariable<List<GameObject>> waypoints;
    [SerializeReference] public BlackboardVariable<float> MoveSpeed;
    [SerializeReference] public BlackboardVariable<float> WaypointThreshold;

    private int currentWaypointIndex;

    protected override Status OnStart()
    {
        base.OnStart();
        if (player == null || rb2D == null)
        {
            return Status.Failure;
        }

        currentWaypointIndex = 0;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // Calcul de la distance vers le waypoint
        Vector2 toPlayer = waypoints.Value[currentWaypointIndex].transform.position - enemy.transform.position;
        // On déplace l'ennemi vers le waypoint
        rb2D.linearVelocity = toPlayer.normalized * MoveSpeed.Value;

        // On calcule si l'ennemi est proche d'un waypoint
        float distance = Vector2.Distance(enemy.transform.position, waypoints.Value[currentWaypointIndex].transform.position);
        if (distance < WaypointThreshold.Value)
        {
            // Si on est proche du waypoint on predn le prochain le waypoint
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Value.Count;
        }
        //  if (animator != null) animator.SetBool("IsMoving", true);

        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (rb2D != null) rb2D.linearVelocity = Vector2.zero;
        //if (animator != null) animator.SetBool("IsMoving", false);
    }
}
