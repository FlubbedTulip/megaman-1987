using Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Projectiles
{
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
        public float travelSpeed = 10f;
        public float rotationSpeed = 720f;
        public float postTravelDuration = 0.5f;

        [Header("Distances & Collision")]
        public float reachDistance = 0.1f;
        public float lifeTime = 5f;

        [FormerlySerializedAs("_damage")]
        [Header("Damage Handling")]
        [SerializeField] private int damage = 10;

        [Tooltip("Which layer belongs to the player? Used for OverlapCircle collision checks.")]
        public LayerMask playerLayer;

        [Header("References (assigned at spawn)")]
        public Transform playerTransform;   // The player's position at firing
        public Transform cutManTransform;   // Where to return after traveling

        // Internal variables
        private Vector3 _direction;          // Normalized direction of travel
        private CutterPhase _currentPhase = CutterPhase.TowardPlayer;
        private float _postTravelTimer;
        private Vector3 _initialPlayerPos;   // Snapshot of player's position on spawn

        private void Start()
        {
            // Safety destruction
            Destroy(gameObject, lifeTime);

            // Take a snapshot of the player's position at the time of firing
            if (playerTransform != null)
                _initialPlayerPos = playerTransform.position;
            else
                _initialPlayerPos = transform.position; // fallback if no player reference

            // Phase 1: set direction from spawn -> player
            _direction = (_initialPlayerPos - transform.position).normalized;
        }

        private void Update()
        {
            // Spin for visual flair
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

            // Handle collision with the player each frame
            CheckPlayerCollision();

            switch (_currentPhase)
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


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                other.gameObject.GetComponent<HealthManager>().TakeDamage(damage);
            }
        }

        /// <summary>
        /// Phase 1: Move in a straight line toward the player's position at time of firing.
        /// When we get close enough, switch to PostTravel.
        /// </summary>
        private void MoveTowardPlayer()
        {
            // Move
            transform.position += _direction * (travelSpeed * Time.deltaTime);

            // Check if we've reached (or passed) the player's position
            float distToPlayerPos = Vector3.Distance(transform.position, _initialPlayerPos);
            if (distToPlayerPos <= reachDistance)
            {
                // Switch to Phase 2: keep going straight for a bit
                _currentPhase = CutterPhase.PostTravel;
                _postTravelTimer = 0f; // reset timer
            }
        }

        /// <summary>
        /// Phase 2: Continue traveling in the same direction for a short duration.
        /// After that delay, switch to ReturnToCutMan.
        /// </summary>
        private void PostTravel()
        {
            // Keep flying in the same direction
            transform.position += _direction * (travelSpeed * Time.deltaTime);

            // Count down the postTravel time
            _postTravelTimer += Time.deltaTime;
            if (_postTravelTimer >= postTravelDuration)
            {
                // Phase 3: Return to Cut Man
                _currentPhase = CutterPhase.ReturnToCutMan;
                // Recalculate direction to Cut Man's current position
                if (cutManTransform != null)
                    _direction = (cutManTransform.position - transform.position).normalized;
                else
                    _direction = Vector3.zero; // fallback to not move if no transform
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

            _direction = (cutManTransform.position - transform.position).normalized;
            // Keep traveling toward Cut Man
            transform.position += _direction * (travelSpeed * Time.deltaTime);

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
}
