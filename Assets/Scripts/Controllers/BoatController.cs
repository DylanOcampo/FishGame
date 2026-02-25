using UnityEngine;

public class BoatController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float maxSpeed = 8f;

    private Rigidbody2D rb;
    private float moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate()
    {
        // Apply horizontal force while preserving vertical velocity (buoyancy)
        rb.AddForce(Vector2.right * moveInput * moveSpeed, ForceMode2D.Force);

        // Clamp horizontal speed
        float clampedX = Mathf.Clamp(rb.linearVelocity.x, -maxSpeed, maxSpeed);
        rb.linearVelocity = new Vector2(clampedX, rb.linearVelocity.y);

        // Flip sprite to face movement direction
        if (moveInput != 0f)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveInput) * Mathf.Abs(transform.localScale.x),
                                                transform.localScale.y,
                                                transform.localScale.z);
        }
    }
}
