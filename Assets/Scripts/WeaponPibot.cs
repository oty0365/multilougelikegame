using Unity.Netcode;
using UnityEngine;

public class WeaponPibot : NetworkBehaviour
{
    private void Update()
    {
        if (IsOwner)
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var dir = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
            gameObject.transform.rotation = Quaternion.AngleAxis(dir, Vector3.forward);
        }

    }
}
