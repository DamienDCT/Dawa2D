using UnityEngine;
using UnityEngine.UI;

public class MapUI : MonoBehaviour
{
    public RectTransform mapContainer;
    public RectTransform zoneVisual;

    public float zoomTime = 0.6f;

    bool isZoomed = false;

    public void ZoomToZone(Vector2 pos, float scale, Sprite sprite, string value)
    {
        Debug.Log("we focus on " + value + " zone");
        if (isZoomed)
            return;

        isZoomed = true;

        zoneVisual.gameObject.transform.localScale = Vector3.zero;

        zoneVisual.GetComponent<Image>().sprite = sprite;
        mapContainer.gameObject.SetActive(true);

        Vector3 targetScale = new Vector3(scale, scale, 1);
        Vector3 targetPos = -pos * scale;

        LeanTween.scale(zoneVisual, targetScale, zoomTime)
            .setEaseOutExpo();

        LeanTween.move(zoneVisual, targetPos, zoomTime)
            .setEaseOutExpo();
    }
}
