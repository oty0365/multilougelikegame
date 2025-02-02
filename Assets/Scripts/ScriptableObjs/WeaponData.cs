using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
    public int Index;
    public string Code;
    public string Name;
    public Sprite Icon;
    [TextArea] public string Description;
    public float Damage;
}
