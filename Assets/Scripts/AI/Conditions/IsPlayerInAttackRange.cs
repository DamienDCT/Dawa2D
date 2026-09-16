using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

[Serializable, GeneratePropertyBag]
[Condition(name: "IsPlayerInAttackRange", story: "Is [enemyTest] in attack range of [AttackRange] from player", category: "Conditions", id: "is-player-in-attack-range")]
public partial class IsPlayerInAttackRangeCondition : EnemyConditional
{
    [SerializeReference] public BlackboardVariable<GameObject> BlackboardEnemy;
    [SerializeReference] public BlackboardVariable<float> AttackRange;

    public override void OnEnd() { }

    public override bool IsTrue()
    {
        if (player == null || enemy == null) return false;

        float distance = Vector2.Distance(enemy.transform.position, player.transform.position);
        return distance <= AttackRange.Value;
    }
}