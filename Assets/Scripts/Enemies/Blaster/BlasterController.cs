using System;
using System.Collections;
using Pools;
using Projectiles;
using UnityEngine;

namespace Enemies.Blaster
{
    public class BlasterController : MonoBehaviour
    {
        private static readonly int IsOpen = Animator.StringToHash("IsOpen");

        [Header("Timings")]
        [SerializeField] private float closedDelay = 2f;
        [SerializeField] private float openDuration = 1f;     
        
        [Header("Shooting")]
        [SerializeField] private float shootCooldown = 0.2f;  
        [SerializeField] private Transform spawnPoint;       
        [SerializeField] private bool isFacingRight = true;   
        
        [Header("References")]
        [SerializeField] private Animator animator;           
        private bool _isOpen;                                 
        private bool _isInvulnerable;     
        
        private Coroutine _cycleRoutine;


        private void OnEnable()
        {
            // Restart the main cycle when the enemy is enabled
            _cycleRoutine = StartCoroutine(CycleRoutine());
        }

        private void OnDisable()
        {
            // Stop the main cycle when the enemy is disabled
            if (_cycleRoutine != null)
            {
                StopCoroutine(_cycleRoutine);
                _cycleRoutine = null;
            }
        }
        
        /// Main open/close cycle coroutine.
        private IEnumerator CycleRoutine()
        {
            while (true)
            {
                // 1) Closed state
                SetClosed(true);
                yield return new WaitForSeconds(closedDelay);

                // 2) Open state
                SetClosed(false);
                
                // Fire bullets immediately
                yield return StartCoroutine(FireBulletsRoutine());
                
                // Wait openDuration in open state
                yield return new WaitForSeconds(openDuration);
            }
        }
        
        
        // Toggles the Blaster's open/closed animations and states
        private void SetClosed(bool closed)
        {
            _isOpen = !closed;
            _isInvulnerable = closed;  // If closed => can't take damage
            animator.SetBool(IsOpen, _isOpen);
        }

       
        // Fires 4 bullets in a half-circle pattern.
        private IEnumerator FireBulletsRoutine()
        {
            yield return new WaitForSeconds(0.5f);
            int bulletCount = 4;
            float startAngle = 45f;
            float endAngle   = -45f;
            
            // If we're facing left, we flip angles horizontally
            float facingMultiplier = isFacingRight ? 1f : -1f;

            for (int i = 0; i < bulletCount; i++)
            {
                // Interpolate between startAngle and endAngle
                float t = (float)i / (bulletCount - 1); // 0..1
                float angleDeg = Mathf.Lerp(startAngle, endAngle, t);

                // Final angle, multiplied by facing
                angleDeg *= facingMultiplier;

                // Base direction is "right" if facing right, "left" if facing left
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

     
        // Utility function to rotate a vector by some degrees
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
