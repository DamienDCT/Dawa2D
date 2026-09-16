using UnityEngine;

[CreateAssetMenu(fileName = "New Profile", menuName = "Map Drag Profile")]
public class MapDragProfile : ScriptableObject
{
    [Header("Map Drag Size")]
    [Tooltip("Always positive => set as negative for interval boundaries")]
    public Vector2 mapDragHorizontalSize;
    [Tooltip("Always positive => set as negative for interval boundaries")]
    public Vector2 mapDragVerticalSize;
}
