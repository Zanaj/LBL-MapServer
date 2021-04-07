using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    public List<Enemy> enemies;

    public GameObject outOfSight;
    void Awake()
    {
        instance = this;
        enemies = new List<Enemy>();
    }
}
