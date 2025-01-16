using Unity.Netcode;
using UnityEngine;

public class MultiplayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    private void Start()
    {
        SpawnPlayer();
    }
    void SpawnPlayer()
    {
        if (IsServer)
        {
            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                var playerObject = Instantiate(playerPrefab);
                playerObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
            }
        }
    }

}
