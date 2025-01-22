using System;
using System.Collections;
using UnityEngine;

namespace Managers
{
    public class HealthManager : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 100f;
        private float _currentHealth;

        [Header("Invincibility")]
        [SerializeField] private float invincibilityDuration = 1f;
        [SerializeField] private float flashDelay = 0.0833f;

        private bool _isInvincible;
        private SpriteRenderer _spriteRenderer;
        
        
        // Events
        public event Action<float> OnDamageTaken; 
        public event Action OnHitWhileInvincible;
        public event Action OnDie;

        private void Awake()
        {
            // Optionally initialize current health to max
            _currentHealth = maxHealth;
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            _spriteRenderer.enabled = true;
            _currentHealth = maxHealth;
        }


        public void TakeDamage(float damage)
        {
            if (_isInvincible)
            {
                OnHitWhileInvincible?.Invoke();
                return;
            }

            // Subtract health
            _currentHealth -= damage;
            print("health is " + _currentHealth);

            // Update any UI or other listeners
            OnDamageTaken?.Invoke(_currentHealth);

            // Start invincibility if needed
            if (invincibilityDuration > 0f)
                StartCoroutine(InvincibilityRoutine());

            // Check for death
            if (_currentHealth <= 0f)
            {
                Die();
            }
        }

        public void Heal(float amount)
        {
            if (amount <= 0f) return;

            _currentHealth += amount;
            if (_currentHealth > maxHealth) _currentHealth = maxHealth;
            
            OnDamageTaken?.Invoke(_currentHealth);
        }

        private void Die()
        {
            Debug.Log($"{gameObject.name} has died");
            // Fire event so other scripts can respond
            StopCoroutine(InvincibilityRoutine());
            _spriteRenderer.enabled = true;
            OnDie?.Invoke();
        }

        private IEnumerator InvincibilityRoutine()
        {
            _isInvincible = true;
            for (int i = 0; i < 10; i++)
            {
                _spriteRenderer.enabled = false;
                yield return new WaitForSeconds(0.04f);
                _spriteRenderer.enabled = true;
                yield return new WaitForSeconds(0.04f);
            }
            _isInvincible = false;
        }

        public float GetMaxHealth()
        {
            return maxHealth;
        }
        
        public void SetExternalInvincible(bool state)
        {
            _isInvincible = state;
        }
    }
}
