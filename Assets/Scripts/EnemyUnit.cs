using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class EnemyUnit
{
    public GameObject enemyPrefab;
    public GameObject enemyItem;

    public EnemyUnit(GameObject _enemyPrefab, GameObject _enemyItem)
    {
        enemyPrefab = _enemyPrefab;
        enemyItem = _enemyItem;
    }
}
