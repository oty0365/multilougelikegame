using UnityEngine;

[CreateAssetMenu(fileName = "PlayableData", menuName = "Scriptable Objects/PlayableData")]
[System.Serializable]
public class PlayableData : ScriptableObject
{
    public int index;
    public string playerCode;
    public string playerName;
    public Sprite icon;
    public AnimationClip[] moveSets;
    [TextArea] public string info;
    public string skillName;
    public float skillCooldown;
    [TextArea] public string skillInfo;
    public string ultimateName;
    public float ultimateCooldown;
    [TextArea] public string ultimateInfo;
}
