using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class SelectionHighlighter : MonoBehaviour
{
    [SerializeField] private RectTransform highlightRect;
    [SerializeField] private float moveTime = 0.2f;
    [SerializeField] private Vector3 scaleEffect = new Vector3(1.1f, 1.1f, 1f);

    private RectTransform currentTarget;

    private RectTransform lastItemSelected;
    [SerializeField] private RectTransform firstTargetOnMenu;


    private void Start()
    {
        if (lastItemSelected == null)
        {
            if(firstTargetOnMenu != null)
            {
                firstTargetOnMenu.GetComponent<Selectable>().Select();
                lastItemSelected = firstTargetOnMenu;
                highlightRect.gameObject.SetActive(true);
            } else
            {
                highlightRect.gameObject.SetActive(false);
            }
        }
        else
        {
            lastItemSelected.GetComponent<Selectable>().Select();
        }

        StartCoroutine(InitializeHighlight());
    }

    private IEnumerator InitializeHighlight()
    {
        yield return null; // attend 1 frame

        if (lastItemSelected != null)
        {
            currentTarget = lastItemSelected;
            MoveHighlightTo(lastItemSelected);
        }
    }

    private void Update()
    {
        // Récupère l'objet sélectionné actuellement par l'EventSystem
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null) return;

        if (CurrentActiveDeviceManager.Instance.CurrentDevice == CurrentActiveDeviceManager.ActiveDevice.Gamepad && Input.GetButtonDown("Submit"))
        {
            var pointerHandler = selected.GetComponent<IPointerClickHandler>();
            if (pointerHandler != null)
            {
                PointerEventData eventData = new PointerEventData(EventSystem.current);
                pointerHandler.OnPointerClick(eventData);
            }
        }

        // Si on est sur un bouton, on cache et on réinitialise currentTarget
        Button button = selected.GetComponent<Button>();
        if (button != null)
        {
            HideHighlight();
            currentTarget = null; // ✅ AJOUT : Force la réactivation au prochain slot
            return;
        }

        RectTransform targetRect = selected.GetComponent<RectTransform>();
        if (targetRect == null) return;

        // ✅ On compare avant d'écraser currentTarget
        if (targetRect == currentTarget)
            return;

        // Déplace le contour
        currentTarget = targetRect;
        lastItemSelected = targetRect;
        highlightRect.gameObject.SetActive(true);
        MoveHighlightTo(targetRect);
    }

    private void MoveHighlightTo(RectTransform target)
    {
        LeanTween.cancel(highlightRect);

        // Convertit la position de l’icône dans le repère du highlight parent
        Vector3 localPoint = highlightRect.parent.InverseTransformPoint(target.position);

        // Aligne la position
        LeanTween.moveLocal(highlightRect.gameObject, localPoint, moveTime).setEaseOutQuad();

        // Aligne la taille
        LeanTween.size(highlightRect, target.rect.size, moveTime).setEaseOutQuad();

        // Effet visuel (optionnel)
        LeanTween.scale(highlightRect.gameObject, scaleEffect, moveTime / 2f)
            .setEaseOutSine()
            .setLoopPingPong(1)
            .setOnComplete(() => highlightRect.localScale = Vector3.one);

        ItemInfo itemInfo = target.GetComponent<ItemInfo>();

        if (itemInfo == null) return;

        ItemInfoUI.Instance.UpdateInformations(itemInfo);
    }

    public void HideHighlight()
    {
        highlightRect.gameObject.SetActive(false);
    }
}