using Unity.Netcode;
using UnityEngine;

public class WeaponPibot : NetworkBehaviour
{
    private void Update()
    {
        if (IsOwner)
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(mousePos);
            var dir = Mathf.Atan2(mousePos.y- gameObject.transform.position.y,mousePos.x- gameObject.transform.position.x) * Mathf.Rad2Deg;
            Debug.Log(dir);
            gameObject.transform.rotation = Quaternion.AngleAxis(dir, Vector3.forward);
        }

    }
}
