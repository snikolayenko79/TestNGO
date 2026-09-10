using Unity.Netcode;
using UnityEngine;

public class NetworkSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    
    [SerializeField] INetworkColorAssigmentService  networkColorAssigmentService;

    private void Awake()
    {
        // здесь бы Zenject/Vcontainer
        networkColorAssigmentService = new RandomNetworkColorAssignmentService();
    }
    
    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayer;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= SpawnPlayer;
    }

    private void SpawnPlayer(ulong clientId)
    {
        // Жесткая проверка: спавнить может только сервер
        if (!NetworkManager.Singleton.IsServer) return;

        Debug.Log($"[Spawner] Сервер спавнит куб вручную для ClientId: {clientId}");

        // Генерируем случайную позицию
        Vector3 randomSpawnPos = new Vector3(Random.Range(-3f, 3f), 0.5f, Random.Range(-3f, 3f));
        
        // Обычный Instantiate
        GameObject playerInstance = Instantiate(playerPrefab, randomSpawnPos, Quaternion.identity);

        // Сетевой спавн с передачей прав владения (Ownership) конкретному клиенту
        NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();
        networkObject.SpawnAsPlayerObject(clientId);
        
        // ПРЯМОЕ НАЗНАЧЕНИЕ ЦВЕТА ИЗ СПАВНЕРА:
        if (playerInstance.TryGetComponent<INetworkColorable>(out var colorable))
        {
            networkColorAssigmentService.AssignColor(colorable);
        }
    }
}