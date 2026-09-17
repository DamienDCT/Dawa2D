using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class InteractionZone : MonoBehaviour
{
    [SerializeField] private float rangeDetection = 2f;

    private CircleCollider2D detectionCollider;
    private IInteractable interactable;

    private void Awake()
    {
        detectionCollider = GetComponent<CircleCollider2D>();
        detectionCollider.isTrigger = true;
        detectionCollider.radius = rangeDetection;

        // Récupère automatiquement tous les IInteractable sur ce GameObject
        interactable = GetComponent<IInteractable>();

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent(out PlayerInteraction player)) return;

        player.SetCurrentInteractable(interactable);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent(out PlayerInteraction player)) return;

        player.ClearCurrentInteractable(interactable);
    }
}