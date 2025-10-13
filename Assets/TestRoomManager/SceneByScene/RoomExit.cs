using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RoomExit : MonoBehaviour
{
    [SerializeField] private string targetRoom;   // Nom de la scène à charger
    [SerializeField] private string targetSpawn;  // ID du spawn dans la room cible

    [SerializeField] private Light2D globalLightToDestroy;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("we colide the player");
            RoomLoader.Instance.LoadRoom(targetRoom, targetSpawn, globalLightToDestroy);
        }
    }
}