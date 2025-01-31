using System.Diagnostics;
using Unity.Netcode;
using UnityEngine;

public class PlayerStatus : NetworkBehaviour
{
    [SerializeField] private Animator ani;
    private NetworkVariable<int> _playableType = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
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
                    _playableType.Value = i.index;
                }
            }
        }
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner)
        {
            UnityEngine.Debug.Log(_playableType.Value);

            _playableType.OnValueChanged += HandlePlayableTypeChanged;
        }
    }
    private void HandlePlayableTypeChanged(int previousValue, int newValue)
    {
        foreach (var i in DataManager.instance.playableSets.playables)
        {
            if (i.index == newValue)
            {
                SwitchToSpecialIdle(i.moveSets[0]);
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
