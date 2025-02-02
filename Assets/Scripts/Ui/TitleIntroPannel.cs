using NUnit.Framework.Constraints;
using UnityEngine;

public class TitleIntroPannel : MonoBehaviour
{
    [SerializeField] private GameObject titlePannel;
    [SerializeField] private GameObject multiPannel;
    [SerializeField] private GameObject loginPannel;

    private void Start()
    {
        multiPannel.SetActive(false);
        loginPannel.SetActive(false);
    }
    public void OnClicked()
    {
        titlePannel.SetActive(false);
        multiPannel.SetActive(true);
        loginPannel.SetActive(true);
    }
}
