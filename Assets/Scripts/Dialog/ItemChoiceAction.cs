using UnityEngine;

[CreateAssetMenu(fileName = "ItemChoiceAction", menuName = "DialogChoiceAction/ItemChoiceAction")]
public class ItemChoiceAction : DialogChoiceAction
{
    [SerializeField] private string itemId;

    public override void PerformAction(object context = null)
    {
        Debug.Log("We give item " + itemId);
    }
}
