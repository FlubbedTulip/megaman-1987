using System;
using Interfaces;
using Pools;
using UnityEngine;

namespace Projectiles
{
    public class BlasterBullet : MonoBehaviour , IPoolable
    {
        [SerializeField] private float speed = 10;
        private Vector2 _direction = Vector2.right;

        public void Initialize(Vector2 direction)
        {
            _direction = direction.normalized;
        }

        private void Update()
        {
            transform.Translate( _direction * (speed * Time.deltaTime));
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                // TODO damage the player, animation, etc...
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
