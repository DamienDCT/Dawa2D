using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapZoneSelectable : MonoBehaviour,
    IPointerClickHandler,
    ISelectHandler,
    ISubmitHandler
{
    [SerializeField] private MapUI mapController;
    [SerializeField] private Image zoneImage;

    public Vector2 zoomPosition;
    public float zoomScale = 2f;
    public Sprite zoneSprite;
    public string valueToZoom;

    private void Start()
    {
        if (zoneImage == null)
            zoneImage = GetComponent<Image>();

        // Cette valeur détermine le seuil de transparence pour le clic (0-1)
        // 0.1 = ignore les pixels avec alpha < 0.1
        zoneImage.alphaHitTestMinimumThreshold = 0.1f;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SelectZone();
    }

    public void OnSelect(BaseEventData eventData)
    {
        SelectZone();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        SelectZone();
    }

    void SelectZone()
    {
        mapController.ZoomToZone(zoomPosition, zoomScale, zoneSprite, valueToZoom);
    }
}