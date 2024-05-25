using UnityEngine;

public class WillowMovementController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Get input
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");

        // Update animator parameters
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
    }

    void FixedUpdate()
    {
        // Move the character
        Vector2 position = rb.position;
        position += movement * moveSpeed * Time.fixedDeltaTime;

        // Adjust position to prevent clipping
        position.y = Mathf.Clamp(position.y, 0, float.MaxValue); // Adjust according to your map's boundaries
        rb.MovePosition(position);
    }
}
