using UnityEngine;
using Unity.Behavior;
using System;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[Condition(name: "DetectPlayer", story: "Is [enemyTest] in range of [DistanceDetection] from player", category: "Conditions", id: "detect-player")]
public class DetectPlayer : EnemyConditional
{
    private Transform playerTransform;
    [SerializeReference] public BlackboardVariable<float> DistanceDetection;

    public override void OnStart()
    {
        base.OnStart();

        if (player != null)
            playerTransform = player.transform;
    }

    public override bool IsTrue()
    {
        float distance = Vector2.Distance(playerTransform.position, enemy.transform.position);


        return distance < DistanceDetection.Value;
    }
}
