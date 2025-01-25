using Managers;
using UnityEngine;

namespace Drops
{
    public class ExtraPointsDrop : MonoBehaviour
    {
        [SerializeField] private int points = 1000;
        
        void OnTriggerEnter2D(Collider2D other)
{
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AddScore(points);
            Destroy(gameObject);
        }
}
        
    
    }
}
