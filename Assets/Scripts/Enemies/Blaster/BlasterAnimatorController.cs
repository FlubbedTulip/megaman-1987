using Managers;
using UnityEngine;

namespace Enemies.Blaster
{
    public class BlasterAnimatorController : MonoBehaviour
    {
        private static readonly int IsDead = Animator.StringToHash("IsDead");
        private static readonly int IsOpen = Animator.StringToHash("IsOpen");
        [SerializeField] private Animator animator;
        [SerializeField] private HealthManager healthManager;
        
        private void OnEnable()
        {
            healthManager.OnDie += PlayDeathAnimation;
            animator.SetBool(IsDead, false);
        }

        private void OnDisable()
        {
            healthManager.OnDie -= PlayDeathAnimation;
        }

        private void PlayDeathAnimation()
        {
            animator.SetBool(IsDead, true);
        }
        

    

        public void SetOpen(bool isOpen)
        {
            animator.SetBool(IsOpen, isOpen);
        }
    }
}
