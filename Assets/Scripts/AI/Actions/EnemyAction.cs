using System;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Behavior;

[Serializable]
public abstract class EnemyAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> BlackboardEnemy;

    protected Rigidbody2D rb2D;
    protected Animator animator;
    protected Destructable destructable;

    protected PlayerMovements player;
    protected GameObject enemy;

    protected virtual void Setup()
    {
        GameObject go = BlackboardEnemy.Value;
        enemy = go;
        destructable = go.GetComponent<Destructable>();
        rb2D = go.GetComponent<Rigidbody2D>();
        player = PlayerMovements.Instance;
    }

    protected override Status OnStart()
    {
        Setup();
        return Status.Running;
    }
}
