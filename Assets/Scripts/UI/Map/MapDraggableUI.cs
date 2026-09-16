using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(RectTransform))]
public class MapDraggableUI : MonoBehaviour, IDragHandler
{
    [Header("Joystick (optionnel, sans InputAction)")]
    [SerializeField] private bool enableJoystick = true;
    [SerializeField] private float joystickSpeed = 300f; // en pixels UI / seconde
    [SerializeField] private float joystickDeadzone = 0.15f;

    [SerializeField] private MapDragProfile currentProfile;
    private Image image;
    private RectTransform rectTransform;
    private Canvas canvas;

    [SerializeField] private Vector2 horizontalBoundaries;
    [SerializeField] private Vector2 verticalBoundaries;

    private void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        // Test 
        //CalculateBoundaries();
    }

    public void SetMapDragProfile(MapDragProfile profile)
    {
        this.currentProfile = profile;
        CalculateBoundaries();
    }

    private void CalculateBoundaries()
    {
        horizontalBoundaries = currentProfile.mapDragHorizontalSize;
        verticalBoundaries = currentProfile.mapDragVerticalSize;
    }

    // --- Souris / tactile via EventSystem ---
    public void OnDrag(PointerEventData eventData)
    {
        // eventData.delta est en pixels écran, on le ramène à l'échelle du canvas
        float scaleFactor = canvas != null ? canvas.scaleFactor : 1f;
        Vector2 delta = eventData.delta / scaleFactor;

        MoveAndClamp(delta);
    }

    // --- Joystick, sans InputAction, lecture directe du device ---
    private void Update()
    {
        if (!enableJoystick || Gamepad.current == null) return;

        Vector2 stick = Gamepad.current.leftStick.ReadValue();
        if (stick.sqrMagnitude < joystickDeadzone * joystickDeadzone) return;

        Vector2 delta = stick * joystickSpeed * Time.deltaTime;
        MoveAndClamp(delta);
    }

    private void MoveAndClamp(Vector2 delta)
    {
        Vector2 newPosition = rectTransform.anchoredPosition + delta;

        newPosition.x = Mathf.Clamp(newPosition.x, horizontalBoundaries.x, horizontalBoundaries.y);
        newPosition.y = Mathf.Clamp(newPosition.y, verticalBoundaries.x, verticalBoundaries.y);

        rectTransform.anchoredPosition = newPosition;
    }
}