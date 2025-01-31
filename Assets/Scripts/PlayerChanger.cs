using UnityEngine;

public class PlayerChanger : MonoBehaviour
{
    [SerializeField] private GameObject selectPannel;
    public void OnClick()
    {
        selectPannel.SetActive(true);
    }
}
