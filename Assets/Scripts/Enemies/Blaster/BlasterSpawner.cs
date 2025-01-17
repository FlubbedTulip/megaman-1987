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
            print("spawner is visible");
            SpawnEnemy();
        }

        private void OnBecameInvisible()
        {
            print("spawner is invisible");
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