using System;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemies.Blaster
{
    public class BlasterAnimatorController : MonoBehaviour
    {
        private static readonly int IsDead = Animator.StringToHash("IsDead");
        private static readonly int IsOpen = Animator.StringToHash("IsOpen");
        [SerializeField] private Animator animator;
        [SerializeField] private HealthManager healthManager;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            animator = GetComponent<Animator>();
            healthManager = GetComponent<HealthManager>();
        }

        private void OnEnable()
        {
            healthManager.OnDie += PlayDeathAnimation;
            animator.SetBool(IsDead, false);
        }

        private void PlayDeathAnimation()
        {
            animator.SetBool(IsDead, true);
            print("Play death animation");
        }
        

    

        public void SetOpen(bool isOpen)
        {
            animator.SetBool(IsOpen, isOpen);
        }
    }
}
