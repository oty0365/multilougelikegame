using UnityEngine;
using UnityEngine.UI;


public class HeroIcon : MonoBehaviour
{
    public PlayableData playable;
    public Image icon;
    void Start()
    {
        icon.sprite = playable.icon;
    }
    public void OnClick()
    {

    }
}
