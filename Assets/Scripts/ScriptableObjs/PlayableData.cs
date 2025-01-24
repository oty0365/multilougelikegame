using UnityEngine;

[CreateAssetMenu(fileName = "PlayableData", menuName = "Scriptable Objects/PlayableData")]
public class PlayableData : ScriptableObject
{
    public string playerCode;
    public string playerName;
    public Sprite icon;
    public AnimationClip[] moveSets;
    [TextArea] public string info;
}
