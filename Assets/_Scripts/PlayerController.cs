using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector3 moveInput;
    public float moveSpeed = 5;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        Vector3 dir = new Vector3(moveInput.x, 0, moveInput.y);
        Vector3 move = dir * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + move);
    }
}