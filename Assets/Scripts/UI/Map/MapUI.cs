using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapUI : MonoBehaviour
{
    [SerializeField] private RectTransform mapContainer;
    [SerializeField] private RectTransform zoneVisual;
    [SerializeField] private MapDraggableUI mapDraggableUI;

    private GameObject currentMapDisplayed;

    public float zoomTime = 0.6f;

    bool isZoomed = false;

    private void Awake()
    {
        currentMapDisplayed = null;
    }

    private void Start()
    {
        if (InputController.Instance != null)
        {
            InputController.Instance.OnBackPressed += CloseMap;
        }
    }

    private void OnDestroy()
    {
        if (InputController.Instance != null)
        {
            InputController.Instance.OnBackPressed -= CloseMap;
        }
    }

    private void CloseMap(object sender, EventArgs e)
    {
        if (!isZoomed)
            return;

        if (currentMapDisplayed != null)
        {
            Destroy(currentMapDisplayed);
            currentMapDisplayed = null;
        }

        mapContainer.gameObject.SetActive(false);
        mapDraggableUI.gameObject.transform.localScale = Vector3.zero;

        isZoomed = false;

        InputController.Instance.SwapInputActionMap("Player");
    }

    public void ZoomToZone(Vector2 pos, float scale, MapZoneObject mapZoneObject)
    {
        if (isZoomed)
            return;

        isZoomed = true;

        SetupMap(mapZoneObject);

        ZoomMap(pos, scale);

        InputController.Instance.SwapInputActionMap("UI");
    }

    private void SetupMap(MapZoneObject mapZoneObject)
    {
        mapDraggableUI.gameObject.transform.localScale = Vector3.zero;


        currentMapDisplayed = Instantiate(mapZoneObject.mapPrefab, mapDraggableUI.transform);
        // Setup map drag profile
        mapDraggableUI?.SetMapDragProfile(mapZoneObject.mapDragProfile);

        mapContainer.gameObject.SetActive(true);
    }

    private void ZoomMap(Vector2 pos, float scale)
    {
        Vector3 targetScale = new Vector3(scale, scale, 1);
        Vector3 targetPos = -pos * scale;

        LeanTween.scale(mapDraggableUI.gameObject, targetScale, zoomTime)
            .setEaseOutExpo();

        LeanTween.move(mapDraggableUI.gameObject, targetPos, zoomTime)
            .setEaseOutExpo();
    }
}
