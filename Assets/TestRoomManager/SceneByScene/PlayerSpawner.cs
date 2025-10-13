using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private string spawnId; // ex: "Room02_LeftDoor"

    public string SpawnId => spawnId;
}
