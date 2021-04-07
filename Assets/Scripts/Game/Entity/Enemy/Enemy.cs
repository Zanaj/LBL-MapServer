using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType
{
    Imp,
}

public class Enemy : Entity
{
    public delegate void _OnDeath();
    public delegate void _OnRespawn();

    public EnemyState state;

    public event _OnDeath OnDeath;
    public event _OnRespawn OnRespawn;
    public string targetGUID;

    public bool isInvisible;
    public override bool isInteractable => true;
    public override EntityType type => EntityType.Enemy;

    public EnemyType enemyType;
    public int subType;

    public SphereCollider aggroCollider;

    public bool isAlive;

    public List<Player> targets;

    public float _health;
    public float _maxHealth;
    public float _wound;

    public float health 
    {
        set { GetVital(Stat.Health).SetCurrent(value); }
        get { return GetVital(Stat.Health).currentValue; }
    }

    public float maxHealth
    {
        set { GetVital(Stat.Health).maxValue = value; }
        get { return GetVital(Stat.Health).maxValue; }
    }

    public float wound
    {
        set { GetVital(Stat.Health).debuffValue = value; }
        get { return GetVital(Stat.Health).debuffValue; }
    }

    public int level;

    private void Start()
    {
        OnDamageRecieved += EnemyDamaged;

        Spawn();
    }

    private void EnemyDamaged(EntityStats attacker)
    {
        EntitySync sync = new EntitySync();
        sync.entity = this;
        sync.entityType = EntityType.Enemy;
        sync.Serialize();
        NetworkManager.instance.SendAll(sync);

    }

    public void Spawn()
    {
        OnRespawn?.Invoke();

        entityGUID = System.Guid.NewGuid().ToString();
        Initialize();
        targetGUID = "NA";

        targets = new List<Player>();
        startPosition = gameObject.transform.position;
        EnemyManager.instance.enemies.Add(this);
        EntityManager.instance.entities.Add(this);

        isAlive = true;
    }

    public bool dies = false;

    private void Update()
    {
        if (needRespawning)
        {
            TimeSpan span = DateTime.Now - respawnTimer;

            if (span.TotalSeconds >= respawnSeconds)
            {
                OnRespawn();
            }
        }

        if (dies)
        {
            dies = false;
            Death();
        }

        _health = health;
        _maxHealth = maxHealth;
        _wound = wound;

    }

    public override void MakeEntityPacket(BinaryWriter writer)
    {
        writer.Write((int)type);
        writer.Write(displayName);
        writer.Write(entityGUID);
        writer.Write(isInteractable);

        writer.Write(transform.position.x);
        writer.Write(transform.position.y);
        writer.Write(transform.position.z);

        writer.Write(transform.eulerAngles.y);

        writer.Write(options.Length);
        if (options.Length > 0)
        {
            for (int i = 0; i < options.Length; i++)
            {
                writer.Write((int)options[i]);
            }
        }
        writer.Write((int)enemyType);
        writer.Write(subType);

        writer.Write(targetGUID);
        writer.Write(maxHealth);
        writer.Write(health);
        writer.Write(0);
        writer.Write(level);
        writer.Write(isInvisible);
        writer.Write((int)state);
    }

    public override void Death()
    {
        base.Death();
        OnDeath?.Invoke();

        respawnTimer = DateTime.Now;

        Vector3 endPos = EnemyManager.instance.outOfSight.transform.position;

        isAlive = false;
        transform.position = endPos;

        EntitySync sync = new EntitySync();
        sync.entity = this;
        sync.entityType = EntityType.Enemy;
        sync.Serialize();

        NetworkManager.instance.SendAll(sync);
        needRespawning = true;
    }

    public override void Respawn()
    {
        base.Respawn();
        isAlive = true;
        
        Spawn();

        EntitySync sync = new EntitySync();
        sync.entity = this;
        sync.entityType = EntityType.Enemy;
        sync.Serialize();

        NetworkManager.instance.SendAll(sync);

        OnRespawn?.Invoke();
    }
}
