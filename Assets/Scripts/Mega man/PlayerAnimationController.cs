using System;
using UnityEngine;

namespace Mega_man
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
         
        private static readonly int Running = Animator.StringToHash("IsRunning");
        private static readonly int Jumping = Animator.StringToHash("IsJumping");
        private static readonly int Shooting = Animator.StringToHash("IsShooting");
        private static readonly int Climbing = Animator.StringToHash("IsClimbing");

        

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
    }
}
