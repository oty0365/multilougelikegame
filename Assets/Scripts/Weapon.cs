using System.Collections;
using System.Security.Cryptography.X509Certificates;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class Weapon : NetworkBehaviour
{
    public WeaponData weaponData;
    public Animator ani;
    public int maxComboRange;
    protected int curComboRange;
    public bool isAttacking;
    private IEnumerator prevCombo;
    protected virtual void OnBasicAttack()
    {
        if (!isAttacking)
        {
            if (curComboRange == 0)
            {
                ani.SetTrigger("attack1");
                    
            }
            else if (curComboRange == 1)
            {
                ani.SetTrigger("attack2");
                 
            }
            else if (curComboRange == 2)
            {
                ani.SetTrigger("attack3");
                curComboRange = 0;
            }
            if (curComboRange + 1 <= maxComboRange)
            {
                curComboRange++;
            }
        }


    }
    private IEnumerator ComboFlow()
    {
        yield return new WaitForSeconds(0.3f);
        curComboRange = 0;
    }
    public void EndAttack()
    {
        isAttacking = false;
        if(prevCombo != null)
        {
            StopCoroutine(prevCombo);
        }
        prevCombo = ComboFlow();
        StartCoroutine(prevCombo);
    }
}
