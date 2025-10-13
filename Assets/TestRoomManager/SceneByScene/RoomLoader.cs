using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class RoomLoader : MonoBehaviour
{
    public static RoomLoader Instance;

    [SerializeField] private string currentRoom;
    private string nextSpawnId;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        currentRoom = SceneManager.GetActiveScene().name;
    }

    public void LoadRoom(string roomName, string spawnId, Light2D lightToDestroy)
    {
        nextSpawnId = spawnId;
        Destroy(lightToDestroy);
        StartCoroutine(LoadRoomAsync(roomName));

    }

    private IEnumerator LoadRoomAsync(string roomName)
    {
        // Charger la nouvelle room
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(roomName, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
            yield return null;

        // Trouver le PlayerSpawner correspondant
        PlayerSpawner[] spawners = GameObject.FindObjectsOfType<PlayerSpawner>();
        foreach (var spawner in spawners)
        {
            if (spawner.SpawnId == nextSpawnId)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                player.GetComponent<PlayerMovements>().ResetPlayerState(spawner.transform.position);
                break;
            }
        }

        // Décharger l’ancienne scène
        if (!string.IsNullOrEmpty(currentRoom))
        {
            SceneManager.UnloadSceneAsync(currentRoom);
        }

        currentRoom = roomName;
    }
}
