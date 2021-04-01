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
    [HideInInspector]public Vector3 startPosition;
    public NavMeshAgent agent;

    public string targetGUID;

    public bool isInvisible;
    public override bool isInteractable => true;
    public override EntityType type => EntityType.Enemy;

    public EnemyType enemyType;
    public int subType;

    public SphereCollider aggroCollider;

    [Range(1, 10)]
    public float innerAttackRange = 1;

    [Range(1,100)]
    public float visableRange = 2;

    [Range(1,100)]
    public float wanderDistance = 10;

    public List<Player> targets;

    public float health 
    {
        set { GetVital(Stat.Health).SetCurrent(value); }
        get { return GetVital(Stat.Health).currentValue; }
    }

    private void Start()
    {
        Initialize();
        targetGUID = "NA";

        Debug.Log("HP: " + health);

        targets = new List<Player>();
        startPosition = gameObject.transform.position;
        EnemyManager.instance.enemies.Add(this);

        aggroCollider.radius = visableRange;
    }

    private void Update()
    {
        if (PlayerManager.instance.onlinePlayers.Exists(x => x.entityGUID != targetGUID))
            UpdateTarget(null);

        if (target != null)
        {
            Vector3 targetPos = target.transform.position;
            Vector3 myPos = gameObject.transform.position;

            float distance = Vector3.Distance(myPos, targetPos);
            float currWander = Vector3.Distance(startPosition, myPos);

            if (currWander < wanderDistance)
            {
                if (distance <= innerAttackRange)
                {
                    agent.SetDestination(myPos);
                    //TODO: do the stabby wabby
                }
                else
                {
                    agent.SetDestination(target.transform.position);
                }
            }
            else
            {
                UpdateTarget(null);
            }
        }
        else
        {
            agent.SetDestination(startPosition);
        }
    }

    private void UpdateTarget(Player t)
    {
        if(t == null)
        {
            agent.SetDestination(startPosition);
            target = t;
            targetGUID = "NA";
        }
        else
        {
            target = t;
            targetGUID = t.entityGUID;
        }

        if (DoesNeedUpdate())
        {
            EntitySync sync = new EntitySync();
            sync.entityType = EntityType.Enemy;
            sync.entity = this;
            sync.Serialize();
            NetworkManager.instance.SendAll(sync);
        }
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
        writer.Write(health);
        writer.Write(isInvisible);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(255, 0, 0, 0.5f);
        Gizmos.DrawSphere(this.transform.position, innerAttackRange);

        Gizmos.color = new Color(0, 255, 0, 0.5f);
        Gizmos.DrawSphere(this.transform.position, visableRange);

        Gizmos.color = new Color(0, 0, 255, 0.5f);
        if (EditorApplication.isPlaying)
        {
            Gizmos.DrawSphere(startPosition, wanderDistance);
        }
        else
        {
            Gizmos.DrawSphere(transform.position, wanderDistance);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        GameObject gObj = col.gameObject;
        Player p = gObj.GetComponent<Player>();

        if (p != null)
        {
            UpdateTarget(p);
        }
    }

    private void OnTriggerExit(Collider col)
    {
        GameObject gObj = col.gameObject;
        Player p = gObj.GetComponent<Player>();

        if (p != null)
        {
            if (targets.Contains(p))
                targets.Remove(p);

            UpdateTarget(targets.FirstOrDefault());
        }
    }

    private bool DoesNeedUpdate()
    {
        if(agent.remainingDistance >= agent.stoppingDistance) { return true; }
        else { return false; }
    }
}
