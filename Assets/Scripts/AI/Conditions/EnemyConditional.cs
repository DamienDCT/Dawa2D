using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Condition = Unity.Behavior.Condition;

[Serializable, GeneratePropertyBag]
public abstract class EnemyConditional : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> enemyTest;
    protected Rigidbody2D rb2D;
    protected Animator animator;
    protected Destructable destructable;

    protected PlayerMovements player;
    protected GameObject enemy;

    protected virtual void Setup()
    {
        GameObject go = enemyTest.Value;
        enemy = go;
        destructable = go.GetComponent<Destructable>();
        rb2D = go.GetComponent<Rigidbody2D>();
        player = PlayerMovements.Instance;
    }

    public override void OnStart()
    {
        Setup();
    }
}
