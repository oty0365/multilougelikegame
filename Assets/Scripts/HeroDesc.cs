using TMPro;
using UnityEngine;

public class HeroDesc : MonoBehaviour
{
    public static HeroDesc instance;
    [SerializeField] private TextMeshProUGUI Name;
    [SerializeField] private TextMeshProUGUI Desc;
    [SerializeField] private TextMeshProUGUI SkillName;
    [SerializeField] private TextMeshProUGUI SkillDesc;
    [SerializeField] private TextMeshProUGUI UltiName;
    [SerializeField] private TextMeshProUGUI UltiDesc;
    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }
    public void UpdatePannel(PlayableData playableData)
    {
        Name.text = playableData.playerName;
        Desc.text = playableData.info;
        SkillName.text = playableData.skillName;
        SkillDesc.text = playableData.skillInfo;
        UltiName.text = playableData.ultimateName;
        UltiDesc.text = playableData.ultimateInfo;
    }
    public void OnQuit()
    {
        gameObject.SetActive(false);
    }

}
