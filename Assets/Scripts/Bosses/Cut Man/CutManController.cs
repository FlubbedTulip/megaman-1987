using System;
using System.Collections;
using Events;
using Managers;
using Projectiles;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Bosses.Cut_Man
{
    public class CutManController : MonoBehaviour
    {
        [Header("References")]
        public Transform player;
        public GameObject rollingCutterPrefab;
    
        [Header("Movement Settings")]
        public float moveSpeed = 2.0f;
        public float jumpForce = 5.0f;
        public float minJumpInterval = 1.5f;
        public float maxJumpInterval = 3.0f;
    
        [Header("Attack Settings")]
        public float attackCooldown = 2.0f;
        public float preAttackDelay = 0.5f;
        public float attackRange = 5.0f;
    
        // Internal State Management
        private enum CutManState { Idle, Move, Jump, Attack }
        private CutManState _currentState = CutManState.Idle;
    
        private float _nextJumpTime;     // Timer for when the next jump is allowed.
        private float _nextAttackTime;   // Timer for when the next attack is allowed.
        private bool _isFacingRight = true;
    
        // Components
        private Rigidbody2D _rb;
        private HealthManager _healthManager;
        private CutManAnimationController _animController;


        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _animController = GetComponent<CutManAnimationController>();
            _healthManager = GetComponent<HealthManager>();
        }

        private void OnEnable()
        {
            _healthManager.OnDie += Die;
        }
        
        
        
        private void OnDisable()
        {
            _healthManager.OnDie -= Die;
        }
        
        
        private void Die()
        {
            //Stop any movement
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
            _rb.bodyType = RigidbodyType2D.Kinematic;
            var col = GetComponent<Collider2D>();
            if (col) col.enabled = false;
            enabled = false;
        }

        private void Start()
        {
            // Initialize timers
            ScheduleNextJump();
            _nextAttackTime = Time.time + attackCooldown;
        }

        private void Update()
        {
            // Decide next state based on conditions
            switch (_currentState)
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
        }

        private void FixedUpdate()
        {
            //flip the character to always face the player
            if (player != null)
            {
                float dx = player.position.x - transform.position.x;
                if (dx > 0f && _isFacingRight) Flip();
                else if (dx < 0f && !_isFacingRight) Flip();
            }
            
            if (_currentState == CutManState.Move)
            {
                MoveTowardsPlayer();
            }
        }

        private void HandleIdleState()
        {
            _animController.SetMoving(false);
            _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);

            // If time to attack and in range, prepare to attack
            if (Time.time >= _nextAttackTime && IsPlayerInRange())
            {
                // Transition to Attack State (throw Rolling Cutter)
                _currentState = CutManState.Attack;
                StartCoroutine(PerformAttackRoutine());
                return;
            }

            // Otherwise, if not attacking, transition to Move
            _currentState = CutManState.Move;
        }

        private void HandleMoveState()
        {
            // If it’s time to jump, do so
            if (Time.time >= _nextJumpTime && IsGrounded())
            {
                Jump();
                ScheduleNextJump(); // Schedule the next jump time
                return; // Jump will put us in Jump state
            }

            // Check if time to attack
            if (Time.time >= _nextAttackTime && IsPlayerInRange())
            {
                _currentState = CutManState.Attack;
                StartCoroutine(PerformAttackRoutine());
            }
        }

        private void MoveTowardsPlayer()
        {
            if (player == null) return;
            _animController.SetMoving(true);


            // Determine direction
            float direction = (player.position.x - transform.position.x) > 0 ? 1f : -1f;

            // Move horizontally
            Vector2 velocity = _rb.linearVelocity;
            velocity.x = direction * moveSpeed;
            _rb.linearVelocity = velocity;
        }

        private void Jump()
        {
            // Make sure we have a valid player reference
            if (player == null)
                return;
            
            _animController.SetJumping(true);


            // Determine if player is to the left or right
            float direction = (player.position.x - transform.position.x) >= 0 ? 1f : -1f;

            // Optionally, use a higher horizontal speed during jumps to ensure Cut Man can leap over the player
            // e.g., 1.5x or 2x the normal move speed
            float jumpHorizontalSpeed = moveSpeed * 2f; 

            // Apply vertical force and horizontal speed
            _rb.linearVelocity = new Vector2(direction * jumpHorizontalSpeed, jumpForce);

            _currentState = CutManState.Jump;
        }


        private void ScheduleNextJump()
        {
            float timeToNextJump = Random.Range(minJumpInterval, maxJumpInterval);
            _nextJumpTime = Time.time + timeToNextJump;
        }

        private bool IsGrounded()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.0f);
            return hit.collider != null;
        }

        // --- Attacking ---
        private IEnumerator PerformAttackRoutine()
        {
            // Brief idle before throwing
            _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y); // Stop horizontal movement
            yield return new WaitForSeconds(preAttackDelay);

            // Trigger Attack animation here
            _animController.TriggerAttack();
            _animController.SetHasHat(false);

            // Instantiate the Rolling Cutter
            ThrowRollingCutter();

            // Wait a moment for throw to finish if needed (like animation event)
            yield return new WaitForSeconds(0.2f);

            // Reset attack timer
            _nextAttackTime = Time.time + attackCooldown;

            // Return to Move or Idle after the throw
            _currentState = CutManState.Move;
        }

        private void ThrowRollingCutter()
        {

            if (rollingCutterPrefab == null) return;

            // Instantiate the projectile
            Vector2 spawnPos = transform.position + (_isFacingRight ? Vector3.right : Vector3.left) * 0.5f;
            GameObject cutterObj = Instantiate(rollingCutterPrefab, spawnPos, Quaternion.identity);
            RollingCutter rollingCutter = cutterObj.GetComponent<RollingCutter>();
            if (rollingCutter != null)
            {
                rollingCutter.playerTransform = player;      // The player's Transform
                rollingCutter.cutManTransform = transform;   // Cut Man's Transform
            }
        }

        private bool IsPlayerInRange()
        {
            if (player == null) return false;
            float distance = Vector2.Distance(transform.position, player.position);
            return distance <= attackRange;
        }

        private void Flip()
        {
            _isFacingRight = !_isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // If we land on ground while in Jump state, transition back to Move
            if (_currentState == CutManState.Jump && collision.collider.CompareTag("Ground"))
            {
                _animController.SetJumping(false);
                _currentState = CutManState.Move;
            }
        }
    }
}
