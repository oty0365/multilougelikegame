using UnityEngine;

public class HeroSelectPannel : MonoBehaviour
{
    private async void OnEnable()
    {
        await DataManager.instance.LoadAllPlayerData();
    }
    public void OnQuit()
    {
        gameObject.SetActive(false);
    }
}
