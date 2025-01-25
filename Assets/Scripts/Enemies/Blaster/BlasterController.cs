using System;
using System.Collections;
using Managers;
using Pools;
using Projectiles;
using UnityEngine;

namespace Enemies.Blaster
{
    public class BlasterController : MonoBehaviour
    {
        [Header("Timing")]
        [SerializeField] private float closedDelay = 2f;
        [SerializeField] private float openDuration = 1f;     
        [SerializeField] private float shootCooldown = 0.2f;  
        
        [Header("Positioning")]
        [SerializeField] private Transform spawnPoint;       
        [SerializeField] private bool isFacingRight = true;  
        
        [Header("Audio")]
        [SerializeField] private AudioClip shootSfx;
        [SerializeField] private AudioClip deathSfx;
        [SerializeField] private AudioClip invincibleSfx;
        
        
       
        private BlasterAnimatorController _animator; 

        private bool _isOpen;                                 
        private bool _isInvulnerable;     
        
        private Coroutine _cycleRoutine;
        private HealthManager _healthManager;


        private void Awake()
        {
            _healthManager = GetComponent<HealthManager>();
            _animator = GetComponent<BlasterAnimatorController>();
        }

        private void OnEnable()
        {
            // Restart the main cycle when the enemy is enabled
            _cycleRoutine = StartCoroutine(CycleRoutine());
            _healthManager.OnDamageTaken += PlayHitSfx;
            _healthManager.OnHitWhileInvincible += PlayInvincibleSfx;
            _healthManager.OnDie += DropPowerUP;
        }

           private void OnDisable()
        {
            // Stop the main cycle when the enemy is disabled
            if (_cycleRoutine != null)
            {
                StopCoroutine(_cycleRoutine);
                _cycleRoutine = null;
            }
            _healthManager.OnDamageTaken -= PlayHitSfx;
            _healthManager.OnHitWhileInvincible -= PlayInvincibleSfx;
            _healthManager.OnDie -= DropPowerUP;
        }


        private void DropPowerUP()
        {
            GameEvents.OnEnemyDied?.Invoke(transform.position);
        }

        private void PlayInvincibleSfx()
        {
            SoundManager.Instance.PlaySound(invincibleSfx);
        }

        private void PlayHitSfx(float obj)
        {
            SoundManager.Instance.PlaySound(deathSfx);
        }


    
        
        // Main open/close cycle coroutine.
        private IEnumerator CycleRoutine()
        {
            while (true)
            {
                // 1) Closed
                SetClosed(true);
                yield return new WaitForSeconds(closedDelay);

                // 2) Open
                SetClosed(false);

                // Fire bullets
                yield return StartCoroutine(FireBulletsRoutine());

                yield return new WaitForSeconds(openDuration);
            }
        }
        
        
        // Toggles the Blaster's open/closed animations and states
        private void SetClosed(bool closed)
        {
            _isOpen = !closed;
            _isInvulnerable = closed;  // If closed => can't take damage
            _healthManager.SetExternalInvincible(_isInvulnerable);
            _animator.SetOpen(_isOpen);
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
            // play sfx
            SoundManager.Instance.PlaySound(shootSfx);
        }
        
        public void OnDeathAnimationComplete()
        {
            // after the animation is done
            gameObject.SetActive(false);
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
