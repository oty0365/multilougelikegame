using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : NetworkBehaviour
{
    [SerializeField] private Rigidbody2D rb2D;
    public float moveSpeed;
    Vector3 dir = Vector2.zero;
    void Start()
    {
        
    }
    void Update()
    {
        
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed&&IsOwner)
        {
            Vector2 input = context.ReadValue<Vector2>();
            dir = input*moveSpeed;
            UpdateRb();
        }
        else
        {
            dir = Vector2.zero;
            UpdateRb();
        }
    }
    public void UpdateRb()
    {
        rb2D.linearVelocity = dir;
    }
}
