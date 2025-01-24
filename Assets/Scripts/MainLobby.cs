using UnityEngine;

public class MainLobby : MonoBehaviour
{
    [SerializeField] private GameObject playerIdol;
    void Start()
    {
        playerIdol.SetActive(true);
    }

    void Update()
    {
        
    }
}
