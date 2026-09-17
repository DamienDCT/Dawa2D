using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private IInteractable currentInteraction;

    public void Interact()
    {
        if(currentInteraction == null)
            return;

        if(currentInteraction.CanInteract)
            currentInteraction.Interact();
    }


    public void SetCurrentInteractable(IInteractable interactable)
    {
        currentInteraction = interactable;
    }

    public void ClearCurrentInteractable(IInteractable interactable)
    {
        if (ReferenceEquals(currentInteraction, interactable))
        {
            currentInteraction = null;
            // Ici : cacher le prompt UI
        }
    }


}
