using System.Diagnostics;
using Unity.Netcode;
using UnityEngine;

public class PlayerStatus : NetworkBehaviour
{
    [SerializeField] private Animator ani;
    private NetworkVariable<string> _playableType = new NetworkVariable<string>("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    async void Start()
    {
        if (IsOwner)
        {
            await DataManager.instance.LoadAllPlayerData();
            foreach (var i in DataManager.instance.playableSets.playables)
            {
                if (i.playerCode == DataManager.instance.ServerData["CurrentPlayableCharacter"].Value.GetAsString())
                {
                    SwitchToSpecialIdle(i.moveSets[0]);
                    _playableType = new NetworkVariable<string>(i.playerCode, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
                }
            }
        }
        else
        {
           foreach(var i in DataManager.instance.playableSets.playables)
            {
                if (i.playerCode == _playableType.Value)
                {
                    SwitchToSpecialIdle(i.moveSets[0]);
                }
            }
        }
    }

    public void SwitchToSpecialIdle(AnimationClip newClip)
    {

        RuntimeAnimatorController currentController = ani.runtimeAnimatorController;
        AnimatorOverrideController overrideController = new AnimatorOverrideController(currentController);
        overrideController["Idel"] = newClip;
        ani.runtimeAnimatorController = overrideController;
    }
}
