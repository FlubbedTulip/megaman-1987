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
        private bool _isInvincible;

        // Animator triggers
        private static readonly int IsHurt = Animator.StringToHash("IsHurt");
        private static readonly int IsDead = Animator.StringToHash("IsDead");
        private Animator _animator;
        
        // Events
        public event Action<float> OnHealthChanged; // Pass current health as param
        public event Action OnDie;

        private void Awake()
        {
            // Optionally initialize current health to max
            _currentHealth = maxHealth;
            _animator = GetComponent<Animator>();
        }
        
        

        public void TakeDamage(float damage)
        {
            if (_isInvincible || damage <= 0f) return;

            // Subtract health
            _currentHealth -= damage;
            _animator?.SetTrigger(IsHurt);

            // Update any UI or other listeners
            OnHealthChanged?.Invoke(_currentHealth);

            // Start invincibility if needed
            if (invincibilityDuration > 0f)
                StartCoroutine(InvincibilityRoutine(invincibilityDuration));

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
            
            OnHealthChanged?.Invoke(_currentHealth);
        }

        private void Die()
        {
            Debug.Log($"{gameObject.name} has died");
            _animator?.SetTrigger(IsDead);

            // Fire event so other scripts can respond
            OnDie?.Invoke();

            // Optionally destroy this object or do something else:
            // Destroy(gameObject);
        }

        private IEnumerator InvincibilityRoutine(float duration)
        {
            _isInvincible = true;
            yield return new WaitForSeconds(duration);
            _isInvincible = false;
        }

        public float GetMaxHealth()
        {
            return maxHealth;
        }
    }
}
