using Managers;
using UnityEngine;

public class ContactDamage : MonoBehaviour
{
    [SerializeField] private int contactDamage = 2; // how much damage to deal

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var playerHealth = other.GetComponent<HealthManager>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(contactDamage);
            }
        }
    }
}
