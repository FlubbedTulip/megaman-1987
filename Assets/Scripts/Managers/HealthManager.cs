using System.Collections;
using UnityEngine;

namespace Managers
{
    public class HealthManager : MonoBehaviour
    {
        private static readonly int IsHurt = Animator.StringToHash("IsHurt");
        private static readonly int IsDead = Animator.StringToHash("IsDead");
        [SerializeField] private float health = 100f;
        private Animator _animator;
        private bool _isInvincible;
        
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _animator = GetComponent<Animator>();
        }
        
        public void TakeDamage(float damage)
        {
            if(_isInvincible) return;
            _animator.SetTrigger(IsHurt);
            health -= damage;
            print("health is: " + health);
            StartCoroutine(TurnInvincible(1f)); //placeholder number

            if (health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            _animator.SetTrigger(IsDead);
            print("Player is Dead");
        }

        private IEnumerator TurnInvincible(float timer)
        {
            _isInvincible = true;
            print("Invincible");
            yield return new WaitForSeconds(timer);
            _isInvincible = false;
            print("Not Invincible");
        }
        
        
    }
}
