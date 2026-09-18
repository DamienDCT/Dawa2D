using System;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private IInteractable currentInteraction;

    private void Start()
    {
        // Plugged the event of InputController
        InputController.Instance.OnInteractButtonPressed += InputController_OnInteractButtonPressed;
    }

    private void OnDestroy()
    {
        InputController.Instance.OnInteractButtonPressed -= InputController_OnInteractButtonPressed;
    }

    private void InputController_OnInteractButtonPressed(object sender, EventArgs e)
    {
        Interact();
    }

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
