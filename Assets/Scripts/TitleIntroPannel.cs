using UnityEngine;

public class TitleIntroPannel : MonoBehaviour
{
    [SerializeField] private GameObject titlePannel;
    public void OnClicked() => titlePannel.SetActive(false);
}
