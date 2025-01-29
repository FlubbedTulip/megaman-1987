using System;
using System.Collections;
using Events;
using UnityEngine;
using Managers;

namespace Bosses.Cut_Man
{
    public class CutManAnimationController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Animator animator;
        [SerializeField] private HealthManager healthManager;
        [SerializeField] private AudioClip bossHitSound;

        // Animator Param Hashes (example)
        private static readonly int IsMoving  = Animator.StringToHash("IsRunning");
        private static readonly int IsJumping = Animator.StringToHash("IsJumping");
        private static readonly int Attack   = Animator.StringToHash("Attack");
        private static readonly int IsDead    = Animator.StringToHash("IsDead");
        private static readonly int HasHat = Animator.StringToHash("HasHat");

        // If you want to notify something after death anim:
        public event Action OnDeathExplosion;

        private void OnEnable()
        {
            // Subscribe to HealthManager events
            if (healthManager != null)
            {
                healthManager.OnDie += PlayDeathAnimation;
                healthManager.OnDamageTaken += HandleHurt;
                GameEvents.BossWeaponReturned += HandleReturn;
            }
        }
        

        private void OnDisable()
        {
            if (healthManager != null)
            {
                healthManager.OnDie -= PlayDeathAnimation;
                healthManager.OnDamageTaken -= HandleHurt;
                GameEvents.BossWeaponReturned -= HandleReturn;

            }
        }

        private void HandleHurt(float currentHealth)
        {
            SoundManager.Instance.PlaySound(bossHitSound);
        }
        
        private void PlayDeathAnimation()
        {
            StartCoroutine(SetDeathAnimation());
        }

        private IEnumerator SetDeathAnimation()
        {
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(1f);
            Time.timeScale = 1f;
            animator.SetTrigger(IsDead);
            OnDeathExplosion?.Invoke();
            yield return new WaitForSecondsRealtime(5f);
            gameObject.SetActive(false);
            GameEvents.BossDeath?.Invoke();
        }
        
        

        // --- Public Methods to Set Anim States ---
        public void SetMoving(bool isMoving)
        {
            animator.SetBool(IsMoving, isMoving);
        }

        public void SetJumping(bool isJumping)
        {
            animator.SetBool(IsJumping, isJumping);
        }

        public void TriggerAttack()
        {
            animator.SetTrigger(Attack);
        }

        public void SetHasHat(bool hasHat)
        {
            animator.SetBool(HasHat, hasHat);
        }
        
        private void HandleReturn()
        {
            SetHasHat(true);
        }

        
    }
}
