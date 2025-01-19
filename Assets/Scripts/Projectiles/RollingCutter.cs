using UnityEngine;

public class RollingCutter : MonoBehaviour
{
    // Possible phases of the cutter’s flight
    private enum CutterPhase
    {
        TowardPlayer,
        PostTravel,      // flying straight a bit longer
        ReturnToCutMan
    }

    [Header("Speed & Timing")]
    [Tooltip("Forward speed of the Rolling Cutter.")]
    public float travelSpeed = 10f;

    [Tooltip("How fast the cutter spins (degrees per second).")]
    public float rotationSpeed = 720f;

    [Tooltip("How long (seconds) the cutter keeps going straight after it passes the player's position.")]
    public float postTravelDuration = 0.5f;

    [Header("Distances & Collision")]
    [Tooltip("Minimum distance threshold for 'reaching' a target (player or boss).")]
    public float reachDistance = 0.1f;

    [Tooltip("Lifetime safety net (auto-destroy if still around after this time).")]
    public float lifeTime = 5f;

    [Header("Damage Handling")]
    [Tooltip("Damage dealt to the player if it hits.")]
    public int damageToPlayer = 2;

    [Tooltip("Which layer belongs to the player? Used for OverlapCircle collision checks.")]
    public LayerMask playerLayer;

    [Header("References (assigned at spawn)")]
    public Transform playerTransform;   // The player's position at firing
    public Transform cutManTransform;   // Where to return after traveling

    // Internal variables
    private Vector3 direction;          // Normalized direction of travel
    private CutterPhase currentPhase = CutterPhase.TowardPlayer;
    private float postTravelTimer;
    private Vector3 initialPlayerPos;   // Snapshot of player's position on spawn

    private void Start()
    {
        // Safety destruction
        Destroy(gameObject, lifeTime);

        // Take a snapshot of the player's position at the time of firing
        if (playerTransform != null)
            initialPlayerPos = playerTransform.position;
        else
            initialPlayerPos = transform.position; // fallback if no player reference

        // Phase 1: set direction from spawn -> player
        direction = (initialPlayerPos - transform.position).normalized;
    }

    private void Update()
    {
        // Spin for visual flair
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        // Handle collision with the player each frame
        CheckPlayerCollision();

        switch (currentPhase)
        {
            case CutterPhase.TowardPlayer:
                MoveTowardPlayer();
                break;

            case CutterPhase.PostTravel:
                PostTravel();
                break;

            case CutterPhase.ReturnToCutMan:
                ReturnToCutMan();
                break;
        }
    }

    /// <summary>
    /// Phase 1: Move in a straight line toward the player's position at time of firing.
    /// When we get close enough, switch to PostTravel.
    /// </summary>
    private void MoveTowardPlayer()
    {
        // Move
        transform.position += direction * travelSpeed * Time.deltaTime;

        // Check if we've reached (or passed) the player's position
        float distToPlayerPos = Vector3.Distance(transform.position, initialPlayerPos);
        if (distToPlayerPos <= reachDistance)
        {
            // Switch to Phase 2: keep going straight for a bit
            currentPhase = CutterPhase.PostTravel;
            postTravelTimer = 0f; // reset timer
        }
    }

    /// <summary>
    /// Phase 2: Continue traveling in the same direction for a short duration.
    /// After that delay, switch to ReturnToCutMan.
    /// </summary>
    private void PostTravel()
    {
        // Keep flying in the same direction
        transform.position += direction * travelSpeed * Time.deltaTime;

        // Count down the postTravel time
        postTravelTimer += Time.deltaTime;
        if (postTravelTimer >= postTravelDuration)
        {
            // Phase 3: Return to Cut Man
            currentPhase = CutterPhase.ReturnToCutMan;
            // Recalculate direction to Cut Man's current position
            if (cutManTransform != null)
                direction = (cutManTransform.position - transform.position).normalized;
            else
                direction = Vector3.zero; // fallback to not move if no transform
        }
    }

    /// <summary>
    /// Phase 3: Return to Cut Man in a straight line. Once close enough, destroy the cutter.
    /// </summary>
    private void ReturnToCutMan()
    {
        if (cutManTransform == null)
        {
            Destroy(gameObject);
            return;
        }

        // Keep traveling toward Cut Man
        transform.position += direction * travelSpeed * Time.deltaTime;

        float distToCutMan = Vector3.Distance(transform.position, cutManTransform.position);
        if (distToCutMan <= reachDistance)
        {
            // Reached Cut Man: destroy or attach to Cut Man's sprite
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Simple check if the Rolling Cutter hits the player (using OverlapCircle).
    /// Replace with your custom collision system if desired.
    /// </summary>
    private void CheckPlayerCollision()
    {
        float radius = 0.2f; // Adjust as needed
        Collider2D hit = Physics2D.OverlapCircle(transform.position, radius, playerLayer);
        if (hit != null)
        {
            // Example: apply damage
            // hit.GetComponent<PlayerHealth>()?.TakeDamage(damageToPlayer);

            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // For debugging in the Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.2f);
    }
}
