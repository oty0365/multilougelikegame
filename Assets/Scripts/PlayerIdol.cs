using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerIdol : MonoBehaviour
{
    public List<string> playalbes;
    async void Start()
    {
        await CheckPlayerCharacters();
        playalbes = new List<string>();
    }
    public async Task CheckPlayerCharacters()
    {
        Dictionary<string, Unity.Services.CloudSave.Models.Item> serverData = await DataManager.instance.LoadAllPlayerData();
        if (!serverData.ContainsKey("CurrentPlayableCharacter"))
        {
            await InitPlayableCharacters();
            await CheckPlayerCharacters();
        }
        else
        {

        }
    }
    public async Task InitPlayableCharacters()
    {
        playalbes.Add("Monk");
        playalbes.Add("ScyberSamurai");
        playalbes.Add("WhiteBlueKnight");
        await DataManager.instance.SaveData("CurrentPlayableCharacter", playalbes[Random.Range(0,playalbes.Count)]);
        await DataManager.instance.SaveData("Playables", playalbes);
    }
    void Update()
    {
        
    }
}
