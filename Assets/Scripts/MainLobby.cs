using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class MainLobby : MonoBehaviour
{
    [SerializeField] private GameObject playerIdol;
    [SerializeField] private TextMeshProUGUI jem;
    [SerializeField] private TextMeshProUGUI money;
    [SerializeField] private GameObject multiPlayPannelRandom;
    void Start()
    {
        playerIdol.SetActive(true);
        UpdateMoneyUI();
    }
    public async void UpdateMoneyUI()
    {
        Dictionary<string, Unity.Services.CloudSave.Models.Item> serverData = await DataManager.instance.LoadAllPlayerData();
        jem.text="ÅÂ¾çÀÇ Á¤¼ö: " + serverData["Jem"].Value.GetAsString();
        money.text = "¹ãÁ¶°¢ »þµå: " + serverData["Money"].Value.GetAsString();

    }
    public void multiPlayRandom()
    {
        multiPlayPannelRandom.SetActive(true);
        gameObject.SetActive(false);
    }
}
