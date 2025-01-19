using UnityEditor;
using UnityEngine;

public class CutManController : MonoBehaviour
{
    // --- Inspector Fields ---
    [Header("References")]
    [Tooltip("Reference to the player's Transform (Mega Man).")]
    public Transform player;
    
    [Tooltip("Projectile prefab for the Rolling Cutter.")]
    public GameObject rollingCutterPrefab;
    
    [Header("Movement Settings")]
    [Tooltip("Horizontal speed when running toward the player.")]
    public float moveSpeed = 2.0f;
    
    [Tooltip("Vertical force applied when Cut Man jumps.")]
    public float jumpForce = 5.0f;
    
    [Tooltip("Minimum time between jumps (sec).")]
    public float minJumpInterval = 1.5f;
    
    [Tooltip("Maximum time between jumps (sec).")]
    public float maxJumpInterval = 3.0f;
    
    [Header("Attack Settings")]
    [Tooltip("Time between Rolling Cutter throws (sec).")]
    public float attackCooldown = 2.0f;
    
    [Tooltip("Number of seconds to briefly idle before throwing.")]
    public float preAttackDelay = 0.5f;

    [Tooltip("Distance at which Cut Man will attempt to throw Rolling Cutter.")]
    public float attackRange = 5.0f;
    
    // --- Internal State Management ---
    private enum CutManState { Idle, Move, Jump, Attack }
    private CutManState currentState = CutManState.Idle;
    
    private float nextJumpTime;     // Timer for when the next jump is allowed.
    private float nextAttackTime;   // Timer for when the next attack is allowed.
    private bool isFacingRight = true;
    
    // Components
    private Rigidbody2D rb;
    private Animator animator;

    // --- Unity Callbacks ---
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // Initialize timers
        ScheduleNextJump();
        nextAttackTime = Time.time + attackCooldown;
    }

    private void Update()
    {
        // Decide next state based on conditions
        switch (currentState)
        {
            case CutManState.Idle:
                HandleIdleState();
                break;
            case CutManState.Move:
                HandleMoveState();
                break;
            case CutManState.Jump:
                // Movement in mid-air still possible, so handle in FixedUpdate
                break;
            case CutManState.Attack:
                // Attack logic is triggered via coroutine or direct call
                break;
        }

        // Update animator parameters (example)
        //animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
    }

    private void FixedUpdate()
    {
        if (currentState == CutManState.Move || currentState == CutManState.Jump)
        {
            MoveTowardsPlayer();
        }
    }

    // --- State Handlers ---
    private void HandleIdleState()
    {
        // Optionally set velocity to zero if you want him to stand still
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        // If time to attack and in range, prepare to attack
        if (Time.time >= nextAttackTime && IsPlayerInRange())
        {
            // Transition to Attack State (throw Rolling Cutter)
            currentState = CutManState.Attack;
            StartCoroutine(PerformAttackRoutine());
            return;
        }

        // Otherwise, if not attacking, transition to Move
        currentState = CutManState.Move;
    }

    private void HandleMoveState()
    {
        // If it’s time to jump, do so
        if (Time.time >= nextJumpTime && IsGrounded())
        {
            Debug.Log("choose jump");
            Jump();
            ScheduleNextJump(); // Schedule the next jump time
            return; // Jump will put us in Jump state
        }

        // Check if time to attack
        if (Time.time >= nextAttackTime && IsPlayerInRange())
        {
            Debug.Log("choose attack");
            currentState = CutManState.Attack;
            StartCoroutine(PerformAttackRoutine());
        }
    }

    // --- Movement & Jumping ---
    private void MoveTowardsPlayer()
    {
        if (player == null) return;

        // Determine direction
        float direction = (player.position.x - transform.position.x) > 0 ? 1f : -1f;
        
        // Flip sprite if needed
        if (direction > 0 && !isFacingRight) Flip();
        else if (direction < 0 && isFacingRight) Flip();

        // Move horizontally
        Vector2 velocity = rb.linearVelocity;
        velocity.x = direction * moveSpeed;
        rb.linearVelocity = velocity;
    }

    private void Jump()
    {
        // Determine horizontal jump direction (50% chance to go left or right)
        float horizontalJumpDirection = Random.value < 0.5f ? -1f : 1f; // -1 for left, 1 for right
        Debug.Log("boss is jumping in direction: " + horizontalJumpDirection);
        // Apply vertical force
        rb.linearVelocity = new Vector2(horizontalJumpDirection * moveSpeed, jumpForce);
        currentState = CutManState.Jump;
    }

    private void ScheduleNextJump()
    {
        float timeToNextJump = Random.Range(minJumpInterval, maxJumpInterval);
        nextJumpTime = Time.time + timeToNextJump;
    }

    private bool IsGrounded()
    {
        // Simple raycast or check. Replace with your actual ground check:
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.0f);
        return hit.collider != null;
    }

    // --- Attacking ---
    private System.Collections.IEnumerator PerformAttackRoutine()
    {
        // Brief idle before throwing
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y); // Stop horizontal movement
        yield return new WaitForSeconds(preAttackDelay);

        // Trigger Attack animation here
        //animator.SetTrigger("Attack");

        // Actually instantiate the Rolling Cutter
        ThrowRollingCutter();

        // Wait a moment for throw to finish if needed (like animation event)
        yield return new WaitForSeconds(0.2f);

        // Reset attack timer
        nextAttackTime = Time.time + attackCooldown;

        // Return to Move or Idle after the throw
        currentState = CutManState.Move;
    }

    private void ThrowRollingCutter()
    {
        Debug.Log("boss is attacking");

        if (rollingCutterPrefab == null) return;

        // Instantiate the projectile
        Vector2 spawnPos = transform.position + (isFacingRight ? Vector3.right : Vector3.left) * 0.5f;
        GameObject cutterObj = Instantiate(rollingCutterPrefab, spawnPos, Quaternion.identity);
        RollingCutter rollingCutter = cutterObj.GetComponent<RollingCutter>();
        if (rollingCutter != null)
        {
            rollingCutter.playerTransform = player;      // The player's Transform
            rollingCutter.cutManTransform = this.transform; // Cut Man's Transform
        }
    }

    private bool IsPlayerInRange()
    {
        if (player == null) return false;
        float distance = Vector2.Distance(transform.position, player.position);
        return distance <= attackRange;
    }

    // --- Utility ---
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If we land on ground while in Jump state, transition back to Move
        if (currentState == CutManState.Jump && collision.collider.CompareTag("Ground"))
        {
            currentState = CutManState.Move;
        }
    }
}
