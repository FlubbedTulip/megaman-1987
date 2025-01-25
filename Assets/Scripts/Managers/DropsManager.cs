using UnityEngine;

public class DropManager : MonoBehaviour
{
    [SerializeField] private GameObject powerUpA;
    [SerializeField] private GameObject powerUpB;

    private void OnEnable()
    {
        GameEvents.OnEnemyDied += HandleEnemyDied;
    }

    private void OnDisable()
    {
        GameEvents.OnEnemyDied -= HandleEnemyDied;
    }

    private void HandleEnemyDied(Vector3 position)
    {
        // 50% drop chance
        float roll = Random.value; 
        if (roll < 0.5f)
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
