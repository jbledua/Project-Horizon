using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementOld : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // Normal movement speed
    [SerializeField] private float boostSpeed = 10f; // Boosted movement speed

    private Vector2 moveInput; // Input value from the Input System
    private Rigidbody rb; // Rigidbody for movement
    private bool isBoosting; // Whether the boost key is held

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component missing from the GameObject.");
        }
    }

    // Called when the Move action is triggered
    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>().normalized;

        //Debug.Log("Move Input: " + moveInput); // Debugging to confirm input is received
    }

    // Called when the Boost action is triggered
    private void OnBoost(InputValue value)
    {
        isBoosting = value.isPressed;

        //Debug.Log("Boost: " + isBoosting); // Debugging to confirm boost key state
    }

    private void FixedUpdate()
    {
        // Determine speed based on whether boost is active
        float currentSpeed = isBoosting ? boostSpeed : moveSpeed;

        // Apply movement in the XZ plane
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y) * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }
}
