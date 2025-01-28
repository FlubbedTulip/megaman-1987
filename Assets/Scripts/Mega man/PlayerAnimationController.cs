using System;
using System.Collections;
using Events;
using Managers;
using UnityEngine;

namespace Mega_man
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private HealthManager healthManager;
         
        private static readonly int Running = Animator.StringToHash("IsRunning");
        private static readonly int Jumping = Animator.StringToHash("IsJumping");
        private static readonly int Shooting = Animator.StringToHash("IsShooting");
        private static readonly int Climbing = Animator.StringToHash("IsClimbing");
        private static readonly int IsDead = Animator.StringToHash("IsDead");
        private static readonly int IsHurt = Animator.StringToHash("IsHurt");
        
        public event Action OnDeathExplosion;

        private void OnEnable()
        {
            healthManager.OnDie += PlayDeathAnimation;
            healthManager.OnDamageTaken += PlayHurtAnimation;
        }
        

        private void OnDisable()
        {
            healthManager.OnDie -= PlayDeathAnimation;
            healthManager.OnDamageTaken -= PlayHurtAnimation;
        }
        
        private void PlayHurtAnimation(float obj)
        {
            animator.SetTrigger(IsHurt);
        }

        private void PlayDeathAnimation()
        {
            StartCoroutine(SetDeathAnimation());
        }

        
        public void SetRunning(bool isRunning)
        {
            animator.SetBool(Running, isRunning);
        }

        public void SetJumping(bool isJumping)
        {
            animator.SetBool(Jumping, isJumping);
        }

        public void SetShooting()
        {
            animator.SetTrigger(Shooting);
        }

        public void SetClimbing(bool isClimbing){
            animator.SetBool(Climbing,isClimbing);
        }


        private IEnumerator SetDeathAnimation()
        {
            animator.SetTrigger(IsHurt);
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(1f);
            Time.timeScale = 1f;
            animator.SetTrigger(IsDead);
            OnDeathExplosion?.Invoke();
            yield return new WaitForSecondsRealtime(2f);
            gameObject.SetActive(false);
            GameEvents.PlayerDeath?.Invoke();
        }
    }
}
