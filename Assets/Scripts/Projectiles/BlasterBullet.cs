using System;
using Interfaces;
using Managers;
using Pools;
using UnityEngine;

namespace Projectiles
{
    public class BlasterBullet : MonoBehaviour , IPoolable
    {
        [SerializeField] private float speed = 10;
        private Vector2 _direction = Vector2.right;
        [SerializeField] private float damage = 1;

        public void Initialize(Vector2 direction)
        {
            _direction = direction.normalized;
        }

        private void Update()
        {
             transform.Translate(_direction * speed * Time.deltaTime * TimeSlower.SlowFactor);
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                // TODO damage the player, animation, etc...
                other.gameObject.GetComponent<HealthManager>().TakeDamage(damage);
                ReturnToPool();
            }
        }

        private void OnBecameInvisible()
        {
            ReturnToPool();
        }

        private void ReturnToPool()
        {
            BlasterBulletPool.Instance.Return(this);
        }


        public void Reset()
        {
            _direction = Vector2.right;
        }
    }
}
