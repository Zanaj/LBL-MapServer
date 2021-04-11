using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    public List<Enemy> enemies;
    public GameObject itemSpawnPrefab;

    public GameObject outOfSight;
    private void Awake()
    {
        instance = this;
        enemies = new List<Enemy>();
    }

    public void OnEnemyDeath(Enemy deadEnemy, Player killer)
    {
        GameObject drop = Instantiate(itemSpawnPrefab);
        drop.transform.position = deadEnemy.transform.position;

        ItemSpawn item = drop.GetComponent<ItemSpawn>();
        if(item == null)
        {
            Destroy(drop);
            return;
        }

        item.owner = killer;
    }
}
