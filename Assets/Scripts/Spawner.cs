using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] List<EnemyUnit> enemyUnits = new List<EnemyUnit>();
    [SerializeField] Vector2 spawnAreaSize = new Vector2(10f, 10f);
    [SerializeField] Transform spawnAreaCenter;
    [SerializeField] float minDistanceFromCenter = 5f;

    void Start()
    {
        foreach (EnemyUnit enemy in enemyUnits)
        {
            Vector2 randomPosition;
            int attempts = 0;
            const int maxAttempts = 50;

            do
            {
                randomPosition = new Vector2(
                    Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                    Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2)
                ) + (Vector2)spawnAreaCenter.position;

                attempts++;
                if (attempts >= maxAttempts)
                {
                    randomPosition = (Vector2)spawnAreaCenter.position + Random.insideUnitCircle.normalized * minDistanceFromCenter;
                    break;
                }
            }
            while (Vector2.Distance(randomPosition, spawnAreaCenter.position) < minDistanceFromCenter);

            Instantiate(enemy.enemyPrefab, randomPosition, Quaternion.identity);
        }
    }
}
