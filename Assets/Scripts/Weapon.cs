using System.Security.Cryptography.X509Certificates;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class Weapon : NetworkBehaviour
{
    public WeaponData weaponData;
    public Animator ani;
    public int comboRange;
    protected void OnAttack()
    {
        
    }
}
