using Managers;
using Pools;
using UnityEngine;
using IPoolable = Interfaces.IPoolable;

namespace Projectiles
{
    public class MegaManBullet : MonoBehaviour , IPoolable
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private float damage = 1f;
        private int _direction; 

        
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
            
            if (other.CompareTag("Enemy"))
            {
                // TODO damage the player, animation, etc...
                other.gameObject.GetComponent<HealthManager>().TakeDamage(damage);
                ReturnToPool();
            }
        }
        
        private void OnBecameInvisible()
        {
            //a built-in Unity callback that triggers when the Renderer is no longer visible.
            ReturnToPool();
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
