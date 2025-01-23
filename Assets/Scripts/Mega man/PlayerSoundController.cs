using System;
using Managers;
using Mega_man.States;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mega_man
{
    public class PlayerSoundController : MonoBehaviour
    {
        [Header("SFX")]
        [SerializeField] private AudioClip hurtSound;
        [SerializeField] private AudioClip deathSound;
        [SerializeField] private AudioClip shootSound;
        
        [Header("Scripts to listen")]
        [SerializeField] private HealthManager healthManager;
        [SerializeField] private PlayerShoot playerShoot;
        [SerializeField] private PlayerAnimationController playerAnimationController;


      

        private void OnEnable()
        {
            healthManager.OnDamageTaken += PlayHurtSound;
            playerShoot.OnShoot += PlayShootSound;
            playerAnimationController.OnDeathExplostion += PlayDeathSound;
        }

        private void OnDisable()
        {
            healthManager.OnDamageTaken -= PlayHurtSound;
            playerShoot.OnShoot -= PlayShootSound;
            playerAnimationController.OnDeathExplostion -= PlayDeathSound;
        }
        
        private void PlayShootSound()
        {
            SoundManager.Instance.PlaySound(shootSound);
        }

        private void PlayDeathSound()
        {
            SoundManager.Instance.PlaySound(deathSound);
        }

        private void PlayHurtSound(float obj)
        {
            SoundManager.Instance.PlaySound(hurtSound);
        }
        
    }
}
