using Unity.Netcode;
using UnityEngine;

public class PlayerStatus : NetworkBehaviour
{
    [SerializeField] private Animator ani;
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
