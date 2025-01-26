using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class MainLobby : MonoBehaviour
{
    [SerializeField] private GameObject playerIdol;
    [SerializeField] private TextMeshProUGUI jem;
    [SerializeField] private TextMeshProUGUI money;
    void Start()
    {
        playerIdol.SetActive(true);
        UpdateMoneyUI();
    }
    public async void UpdateMoneyUI()
    {
        Dictionary<string, Unity.Services.CloudSave.Models.Item> serverData = await DataManager.instance.LoadAllPlayerData();
        jem.text="태양의 정수: " + serverData["Jem"].Value.GetAsString();
        money.text = "밤조각 샤드: " + serverData["Money"].Value.GetAsString();

    }
}
