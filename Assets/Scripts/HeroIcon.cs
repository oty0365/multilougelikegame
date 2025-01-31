using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HeroIcon : MonoBehaviour
{
    public PlayableData playable;
    public Image icon;
    public bool isUnlocked;
    private void OnEnable()
    {
        if (DataManager.instance.ServerData["Playables"].Value.GetAs<List<string>>().Contains(playable.playerCode))
        {
            icon.sprite = playable.icon;
            isUnlocked = true;
        }
    }
    public void OnClick()
    {
        if (isUnlocked)
        {
            PlayerIdol.instance.ChangePlayableCharacters(playable.playerCode);
            HeroDesc.instance.gameObject.SetActive(true);
            HeroDesc.instance.UpdatePannel(playable);
        }

    }
}
