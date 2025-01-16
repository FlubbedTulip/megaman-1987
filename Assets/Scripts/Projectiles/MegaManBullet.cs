using Pools;
using UnityEngine;
using IPoolable = Interfaces.IPoolable;

namespace Projectiles
{
    public class MegaManBullet : MonoBehaviour , IPoolable
    {
        [SerializeField] private float speed = 10f;
        private int _direction; // +1 for right, -1 for left

        
        public void Initialize(int direction)
        {
            _direction = direction;
        }

        private void Update()
        {
            transform.Translate(Vector3.right * (_direction * speed * Time.deltaTime));
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            // If we ignore world terrain, presumably you have a layer collision matrix
            // or the bullet's collider set to "IsTrigger" so it passes through terrain.
            // Then you only detect enemies here:

            if (other.CompareTag("Enemy"))
            {
                // Damage the enemy, etc.
                // ...
                ReturnToPool();
            }
        }
        
        private void OnBecameInvisible()
        {
            // This is a built-in Unity callback that triggers when the Renderer is no longer visible.
            // If you want the bullet to vanish when off-screen:
            ReturnToPool();
            Debug.Log("bullet returned");
        }
        
        private void ReturnToPool()
        {
            // Return it to the pool
            MegaManBulletPool.Instance.Return(this);
        }

        public void Reset()
        {
            _direction = 0;
        }
    }
}
