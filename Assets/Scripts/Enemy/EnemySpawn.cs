using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public Enemy[] enemyArray;
    public Enemy spawnedEnemy;

    // Start is called before the first frame update
    public void SpawnEnemies()
    {
        spawnedEnemy = Instantiate(enemyArray[Random.Range(0, enemyArray.Length)], transform.position, transform.rotation);
    }

}
