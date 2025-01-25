using Managers;
using UnityEngine;

public class DropManager : MonoSingleton<DropManager>
{
    [SerializeField] private GameObject powerUpA;
    [SerializeField] private GameObject powerUpB;

  private void OnEnable()
{
    Debug.Log("DropManager subscribed to OnEnemyDied");
    GameEvents.OnEnemyDied += HandleEnemyDied;
}

private void OnDisable()
{
    Debug.Log("DropManager unsubscribed from OnEnemyDied");
    GameEvents.OnEnemyDied -= HandleEnemyDied;
}
    private void HandleEnemyDied(Vector3 position)
    {
        // 50% drop chance
        float roll = Random.value; 
        if (roll < 0.1f)
        {
            // 70% powerUpA, 30% powerUpB
            float which = Random.value;
            GameObject prefab = (which < 0.7f) ? powerUpA : powerUpB;
            Instantiate(prefab, position, Quaternion.identity);

            Debug.Log("Dropped item at " + position);
        }
        else
        {
            Debug.Log("No drop");
        }
    }
}
