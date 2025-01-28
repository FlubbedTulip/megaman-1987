using System;
using UnityEngine;

namespace Enemies.Blaster
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab; // Assign the enemy prefab

        private void Start()
        {
            enemyPrefab.SetActive(false);
        }

        private void OnBecameVisible()
        {
            SpawnEnemy();
        }

        private void OnBecameInvisible()
        {
            DespawnEnemy();
        }

        private void SpawnEnemy()
        {
            enemyPrefab.SetActive(true);
           
        }

        private void DespawnEnemy()
        {
            enemyPrefab.SetActive(false);
        }
    }
}