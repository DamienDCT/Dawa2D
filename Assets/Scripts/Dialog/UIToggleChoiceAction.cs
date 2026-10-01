using UnityEngine;

public enum UIActionType
{
    Open,
    Close,
    Toggle
}

[CreateAssetMenu(fileName = "UIToggleChoiceAction", menuName = "DialogChoiceAction/UIToggleChoiceAction")]
public class UIToggleChoiceAction : DialogChoiceAction
{
    [SerializeField] private UIId targetUI;
    [SerializeField] private UIActionType actionType;

    public override void PerformAction(object context = null)
    {
        if (UIManager.Instance == null)
        {
            Debug.LogWarning("UIChoiceAction: aucun UIManager trouvé dans la scène.");
            return;
        }


        // On close le dialog avant
        UIManager.Instance.Close(UIId.Dialog);

        switch (actionType)
        {
            case UIActionType.Open:
                UIManager.Instance.Open(targetUI);
                break;
            case UIActionType.Close:
                UIManager.Instance.Close(targetUI);
                break;
            case UIActionType.Toggle:
                UIManager.Instance.Toggle(targetUI);
                break;
        }

    }
}