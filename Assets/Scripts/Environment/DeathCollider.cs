using System;
using Managers;
using UnityEngine;

namespace Environment
{
    public class DeathCollider : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                other.gameObject.GetComponent<HealthManager>().TakeDamage(999999);
            }
        }
    }
}
