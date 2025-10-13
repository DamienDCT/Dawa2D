using UnityEngine;


public abstract class DialogChoiceAction : ScriptableObject
{
    public abstract void PerformAction(object context = null);
}
