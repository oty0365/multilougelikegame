using UnityEngine;

public class PlayableSelectUI : MonoBehaviour
{
    public GameObject selectionPannel;
    public GameObject descPannel;
    public bool isSelecting;
    void Start()
    {
        selectionPannel.SetActive(false);
        //descPannel.SetActive(false);
        isSelecting = false;
    }
    public void OnHover()
    {
        if (!isSelecting)
        {
            descPannel.SetActive (true);
        }
        
    }
    public void OutHover()
    {
        descPannel.SetActive (false);
    }
    public void OnClick()
    {
        if (!isSelecting)
        {
            selectionPannel.SetActive(true);
            isSelecting=true;
            descPannel.SetActive(false);
        }
    }
}
