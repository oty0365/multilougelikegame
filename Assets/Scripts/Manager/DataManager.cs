using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.VisualScripting;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;
    public PlayableSets playableSets;
    public Dictionary<string, Unity.Services.CloudSave.Models.Item> ServerData;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public async Task<Dictionary<string, Unity.Services.CloudSave.Models.Item>> LoadAllPlayerData()
    {
        Dictionary<string, Unity.Services.CloudSave.Models.Item> serverData = await CloudSaveService.Instance.Data.Player.LoadAllAsync();
        ServerData = serverData;
        return serverData;
    }
    public async Task SaveData(string dataKey,object dataVlaue)
    {
        var data = new Dictionary<string, object> { {dataKey,dataVlaue} };
        await CloudSaveService.Instance.Data.Player.SaveAsync(data);
    }
}
