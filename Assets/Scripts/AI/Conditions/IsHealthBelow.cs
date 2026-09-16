using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

[Serializable, GeneratePropertyBag]
[Condition(name: "IsHealthBelow", story: "Is [enemyTest] health below [healthBelow]", category: "Conditions", id: "is-health-below")]
public partial class IsHealthBelow : EnemyConditional
{
    [SerializeReference] public BlackboardVariable<float> healthBelow;

    public override void OnEnd()
    {

    }

    public override bool IsTrue()
    {
        return destructable.GetHealthAmount() < healthBelow.Value;
    }
}