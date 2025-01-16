using System;
using Pools;
using Projectiles;
using UnityEngine;

namespace Mega_man
{
    public class PlayerShoot : MonoBehaviour
    {
        [SerializeField] private float shootCooldown = 0.2f;
        private MegaManBullet _bullet;
        private float _lastShootTime;
        private PlayerAnimationController _animController;
        [SerializeField] private Transform rightShootSpawn;
        [SerializeField] private Transform leftShootSpawn;
        
        

        public void Shoot(bool isFacingRight)
        {
            if (Time.time - _lastShootTime < shootCooldown) return;
            _lastShootTime = Time.time;
            
            //spawn Bullet
            var bullet = MegaManBulletPool.Instance.Get();
            bullet.transform.position = isFacingRight ? rightShootSpawn.position : leftShootSpawn.position;
            
            int direction = isFacingRight ? 1 : -1;
            
            bullet.Initialize(direction);
        }
    }
}
