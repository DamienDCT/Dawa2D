using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Basic Attack", story: "Enemy attacks", category: "Action", id: "basic-attack")]
public class BasicAttack : EnemyAction
{
    [SerializeReference] public BlackboardVariable<float> AttackCooldown = new BlackboardVariable<float>(1.5f);
    [SerializeReference] public BlackboardVariable<int> Damage = new BlackboardVariable<int>(1);

    private float lastAttackTime = -999f;

    protected override Status OnStart()
    {
        Setup();
        if (rb2D != null) rb2D.linearVelocity = Vector2.zero;

        TryAttack();
        // Running : le noeud reste "actif" pendant le cooldown, c'est la
        // garde Abort au-dessus (IsPlayerInAttackRange) qui décide s'il
        // faut rebasculer sur Chase, pas ce noeud lui-même.
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (TryAttack() == Status.Success)
            return Status.Success;
        return Status.Running;
    }

    private Status TryAttack()
    {
        if (Time.time - lastAttackTime < AttackCooldown.Value)
            return Status.Failure;

        lastAttackTime = Time.time;
        if (animator != null) animator.SetTrigger("Attack");

        // Adapte le nom de la méthode à ta vraie classe Destructable.
        Destructable destructable = player != null ? player.GetComponent<Destructable>() : null;
        if (destructable != null)
        {
            destructable.Hit((float)Damage.Value, Vector2.negativeInfinity);
            Debug.Log("we attack");
        }
        return Status.Success;
    }

    protected override void OnEnd() { }
}
