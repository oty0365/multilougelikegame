using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.VisualScripting;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    private void Awake()
    {
        instance = this;
    }
    public async Task<Dictionary<string, Unity.Services.CloudSave.Models.Item>> LoadAllPlayerData()
    {
        Dictionary<string, Unity.Services.CloudSave.Models.Item> serverData = await CloudSaveService.Instance.Data.Player.LoadAllAsync();
        return serverData;
    }
    public async Task SaveData(string dataKey,object dataVlaue)
    {
        var data = new Dictionary<string, object> { {dataKey,dataVlaue} };
        await CloudSaveService.Instance.Data.Player.SaveAsync(data);
    }
}
