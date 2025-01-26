using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Apis.Admin.RemoteConfig;
using UnityEngine;
using Newtonsoft.Json;
using Unity.Services.CloudSave.Internal.Http;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using Unity.Services.Apis.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.Core;
using NUnit.Framework;
using UnityEditor.Animations;
using TMPro;

public class PlayerIdol : MonoBehaviour
{
    public List<string> playalbes;
    public Animator playerIdolAni;
    [SerializeField] private TextMeshProUGUI userName;

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

            foreach (var i in DataManager.instance.playableSets.playables)
            {
                if (i.playerCode == serverData["CurrentPlayableCharacter"].Value.GetAsString())
                {
                    Debug.Log(i.moveSets[0]);
                    SwitchToSpecialIdle(i.moveSets[0]);
                    userName.text = serverData["NickName"].Value.GetAsString();
                }
            }
        }
    }
    public async Task InitPlayableCharacters()
    {
        playalbes.Add("Monk");
        playalbes.Add("ScyberSamurai");
        playalbes.Add("WhiteBlueKnight");
        await DataManager.instance.SaveData("CurrentPlayableCharacter", playalbes[UnityEngine.Random.Range(0,playalbes.Count)]);
        await DataManager.instance.SaveData("Playables", playalbes);
    }
    public void SwitchToSpecialIdle(AnimationClip newClip)
    {

        RuntimeAnimatorController currentController = playerIdolAni.runtimeAnimatorController;
        AnimatorOverrideController overrideController = new AnimatorOverrideController(currentController);
        overrideController["Idel"] = newClip; 
        playerIdolAni.runtimeAnimatorController = overrideController;
    }
}
