using Managers;
using UnityEngine;

namespace Drops.Extra_Points
{
    public class ExtraPointsDrop : MonoBehaviour
    {
        [SerializeField] private int points = 1000;
        [SerializeField] private AudioClip pickupSound;
        
        void OnTriggerEnter2D(Collider2D other)
{
        if (other.CompareTag("Player"))
        {
            SoundManager.Instance.PlaySound(pickupSound);
            GameManager.Instance.AddScore(points);
            Destroy(gameObject);
        }
}
        
    
    }
}
