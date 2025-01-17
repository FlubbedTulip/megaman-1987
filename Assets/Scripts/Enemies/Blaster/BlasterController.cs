using System.Collections;
using Pools;
using Projectiles;
using UnityEngine;

namespace Enemies.Blaster
{
    public class BlasterController : MonoBehaviour
    {
        [Header("Timings")]
        [SerializeField] private float closedDelay = 2f;      // Time spent closed
        [SerializeField] private float openDuration = 1f;     // Time spent open (firing)
        
        [Header("Shooting")]
        [SerializeField] private float shootCooldown = 0.2f;  // Delay between each bullet shot
        [SerializeField] private Transform spawnPoint;        // Where bullets spawn
        [SerializeField] private bool isFacingRight = true;   // Which way the Blaster is facing
        
        [Header("References")]
        [SerializeField] private Animator animator;           // Animator with open/close states
        private bool _isOpen;                                 // Tracks if the blaster is open
        private bool _isInvulnerable;                         // If you want to handle damage logic

        private void Start()
        {
            // Start the main cycle
            StartCoroutine(CycleRoutine());
        }

        /// <summary>
        /// Main open/close cycle coroutine.
        /// </summary>
        private IEnumerator CycleRoutine()
        {
            while (true)
            {
                // 1) Closed state
                SetClosed(true);
                yield return new WaitForSeconds(closedDelay);

                // 2) Open state
                SetClosed(false);
                
                // Fire bullets immediately (or spread them over time)
                yield return StartCoroutine(FireBulletsRoutine());
                
                // Wait openDuration in open state
                yield return new WaitForSeconds(openDuration);
            }
        }

        /// <summary>
        /// Toggles the Blaster's open/closed animations and states
        /// </summary>
        private void SetClosed(bool closed)
        {
            _isOpen = !closed;
            _isInvulnerable = closed;  // If closed => can't take damage
            animator.SetBool("IsOpen", _isOpen);
        }

       
        // Fires 4 bullets in a half-circle pattern.
        private IEnumerator FireBulletsRoutine()
        {
            int bulletCount = 4;
            float startAngle = 45f;
            float endAngle   = -45f;
            
            // If we're facing left, we flip angles horizontally
            // so we can unify logic by always rotating from left to right
            float facingMultiplier = isFacingRight ? 1f : -1f;

            for (int i = 0; i < bulletCount; i++)
            {
                // Interpolate between startAngle and endAngle
                float t = (float)i / (bulletCount - 1); // 0..1
                float angleDeg = Mathf.Lerp(startAngle, endAngle, t);

                // Final angle, multiplied by facing
                angleDeg *= facingMultiplier;

                // Base direction is "right" if facing right, "left" if facing left
                // We'll use Vector2.right as "forward", rotate by angle
                Vector2 baseDir = isFacingRight ? Vector2.right : Vector2.left;
                Vector2 bulletDir = RotateByAngle(baseDir, angleDeg);

                // Spawn the bullet
                FireBullet(bulletDir);

                if(i != bulletCount - 1) yield return new WaitForSeconds(shootCooldown);
            }
        }

        
        // Spawns a single bullet in the given direction.
        private void FireBullet(Vector2 direction)
        {
            // Grab bullet from pool
            BlasterBullet bullet = BlasterBulletPool.Instance.Get();
            // Position at spawn point
            bullet.transform.position = spawnPoint.position;
            // Initialize bullet direction
            bullet.Initialize(direction);
        }

     
        // Utility function to rotate a vector by some degrees in 2D
        private Vector2 RotateByAngle(Vector2 vector, float angleDeg)
        {
            float theta = angleDeg * Mathf.Deg2Rad;
            float cos   = Mathf.Cos(theta);
            float sin   = Mathf.Sin(theta);

            // Rotation matrix multiply
            float rx = vector.x * cos - vector.y * sin;
            float ry = vector.x * sin + vector.y * cos;
            return new Vector2(rx, ry);
        }
    }
}
