using System;
using UnityEngine;

namespace Mega_man
{
    public class PlayerAnimationManager : MonoBehaviour
    {
         private static Animator _animator;
    
    
        private static readonly int Running = Animator.StringToHash("IsRunning");
        private static readonly int Jumping = Animator.StringToHash("IsJumping");
        private static readonly int Shooting = Animator.StringToHash("IsShooting");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public static void SetIsRunning(bool isRunning)
        {
            _animator.SetBool(Running, isRunning);
        }

        public static void SetIsJumping(bool isJumping)
        {
            _animator.SetBool(Jumping, isJumping);
        }

        public static void SetIsShooting(bool isShooting)
        {
            _animator.SetBool(Shooting,isShooting);
        }
    }
}
