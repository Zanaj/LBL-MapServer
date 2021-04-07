using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Roaming,
    ChaseTarget,
    Attacking,
    Returning,
    Dead,
}

public class EnemyAI : MonoBehaviour
{
    [Header("Basic stats")]
    [HideInInspector]public Enemy enemy;
    public NavMeshAgent agent;

    public float healthOverride;
    public float minDamageOverride;
    public float maxDamageOverride;

    [Range(150, 10000)]
    public float stabCooldown;
    private DateTime lastStab;
    private float nextStabTime;
    private EnemyState state
    {
        get => enemy.state;
        set => enemy.state = value;
    }
    private Vector3 roamPosition;
    private Vector3 startingPosition
    {
        get => enemy.startPosition;
        set => enemy.startPosition = value;
    }

    [Header("AI Ranges")]
    [Range(1, 10)]
    public float innerAttackRange = 1;

    [Range(1, 100)]
    public float visableRange = 2;

    [Range(1, 100)]
    public float wanderDistance = 10;

    private void Start()
    {
        //Setting up our events so the ai can eventually deal with these.
        enemy.OnDamageDealt += Enemy_OnDamageDealt;
        enemy.OnDamageRecieved += Enemy_OnDamageRecieved;
        enemy.OnDeath += Enemy_OnDeath;
        enemy.OnRespawn += Enemy_OnRespawn;

        //Set it to default state.
        state = EnemyState.Roaming;

        //If someone slipped up we try get the enemy reference.
        if (enemy == null)
            enemy = GetComponent<Enemy>();

        //We shouldn't update enemy if there is no enemy to update!
        if(enemy != null)
        {
            //We set its starting values!
            enemy.SetVital(Stat.Health, healthOverride, EntityStats.VitalSetType.ChangeMax);
            enemy.SetStat(Stat.Min_Damage, minDamageOverride);
            enemy.SetStat(Stat.Max_Damage, maxDamageOverride);

            //Wander position.
            roamPosition = GetRoamingPosition();
        }
    }

    private void Enemy_OnRespawn()
    {
        //Restart the agent and return to starting point
        agent.enabled = true;
        agent.isStopped = false;

        state = EnemyState.Returning;
        agent.SetDestination(startingPosition);
    }

    private void Enemy_OnDeath()
    {
        state = EnemyState.Dead;
        //Weird error where i have to disable or else it doesnt teleport correctly.
        agent.enabled = false;
        agent.isStopped = true;
    }

    private void Enemy_OnDamageRecieved(EntityStats attacker)
    {
        //Shit hurts man
        Debug.Log("Ouchie");
    }

    private void Enemy_OnDamageDealt(EntityStats target)
    {
        //Aww yiss.
        Debug.Log("Fuck yes");
    }

    private void Update()
    {
        //If no enemy reference a lot of things could fuck up.
        if (enemy == null)
            return;

        //VERY basic state machine
        switch (state)
        {
            case EnemyState.Roaming:
                RoamAround();
                break;
            case EnemyState.ChaseTarget:
                ChasePlayer();
                break;
            case EnemyState.Attacking:
                AttackPlayer();
                break;
            case EnemyState.Returning:
                if (isAtDestination())
                    state = EnemyState.Roaming;
                break;
            default:
                break;
        }

        //TODO: better movement update
        EntitySync sync = new EntitySync();
        sync.entity = enemy;
        sync.entityType = EntityType.Enemy;
        sync.Serialize();
        NetworkManager.instance.SendAll(sync);
    }

    private void RoamAround()
    {
        //If we try walking to the random position
        agent.SetDestination(roamPosition);
        if (isAtDestination())
            //If we reached it and still is in this state it means we need a new pos
            roamPosition = GetRoamingPosition();

        //Anyone here?
        FindTarget();
    }

    private void FindTarget()
    {
        //TODO: uncomment this when ai gets serious.
        //if (PlayerManager.instance.onlinePlayers.Count <= 0)
        //    return;

        //Get all collider on the entity layer.
        Collider[] colls = Physics.OverlapSphere(transform.position, visableRange, 1 << 8);
        GameObject bestTarget = null;
        float closestDistance = float.MaxValue;

        //Find the closest.
        for (int i = 0; i < colls.Length; i++)
        {
            GameObject entity = colls[i].gameObject;
            if (entity.tag == "Player")
            {
                float currDis = Vector3.Distance(transform.position, entity.transform.position);
                if (currDis < closestDistance)
                {
                    closestDistance = currDis;
                    bestTarget = entity;
                }
            }
        }

        //If we got a good target then prepare to change state.
        if (bestTarget != null)
        {
            Player targetPlayer = bestTarget.GetComponent<Player>();
            if (targetPlayer != null)
            {
                UpdateEntityTarget(targetPlayer);
                state = EnemyState.ChaseTarget;
            }
            else { Debug.LogWarning($"Found {bestTarget.name} with Player tag but no Player Compoment."); }
        }
    }

    private void ChasePlayer()
    {
        Vector3 targetPos = enemy.target.transform.position;

        //Check if we are out of wander distance
        if (!CanMoveToTarget(targetPos))
        {
            state = EnemyState.Roaming;
            return;
        }

        //Ok we can move to them then move to the target.
        agent.SetDestination(targetPos);

        //If we close enough to meele and can stap well then we should stab!
        if (isInMeeleDistance())
            state = EnemyState.Attacking;
    }

    private void AttackPlayer()
    {
        //If we cant see the enemy anymore we should return.
        if (!isWithinSight())
        {
            UpdateEntityTarget(null);
            agent.SetDestination(startingPosition);
            state = EnemyState.Returning;
            return;
        }

        //If we arent within meele distance we should move closer?
        if (!isInMeeleDistance())
        {
            state = EnemyState.ChaseTarget;
            return;
        }

        TimeSpan span = DateTime.Now - lastStab;
        if (span.TotalMilliseconds < stabCooldown)
            return;

        //If all of those are true then we can stab!
        agent.SetDestination(enemy.transform.position);
        enemy.DoDamageTo(enemy.target);
        lastStab = DateTime.Now;
    }

    private Vector3 GetRoamingPosition()
    {
        float x = UnityEngine.Random.Range(-1f, 1f);
        float z = UnityEngine.Random.Range(-1f, 1f);
        Vector3 randomDir = new Vector3(x, 0, z).normalized;

        float distance = UnityEngine.Random.Range(innerAttackRange, wanderDistance);
        return startingPosition + randomDir * distance;
    }

    #region Distance Helper Functions
    private bool isAtDestination()
    {
        return agent.remainingDistance <= agent.stoppingDistance;
    }

    private bool isInMeeleDistance()
    {
        if (enemy.target == null)
            return false;

        Vector3 targetPos = enemy.target.transform.position;
        float distance = Vector3.Distance(enemy.transform.position, targetPos);
        return distance <= innerAttackRange;
    }

    private bool isWithinSight()
    {
        if (enemy.target == null)
            return false;

        Vector3 targetPos = enemy.target.transform.position;
        float distance = Vector3.Distance(enemy.transform.position, targetPos);
        return distance <= visableRange;
    }

    private bool CanMoveToTarget(Vector3 target)
    {
        float distance = Vector3.Distance(startingPosition, target);
        return distance <= wanderDistance;
    }
    #endregion

    private void UpdateEntityTarget(Player player) 
    {
        //We update the target on the enemy
        enemy.target = player;

        //Update the enemy with its new target for all!
        EntitySync sync = new EntitySync();
        sync.entity = enemy;
        sync.entityType = EntityType.Enemy;
        sync.Serialize();
        NetworkManager.instance.SendAll(sync);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(255, 0, 0, 0.5f);
        Gizmos.DrawSphere(this.transform.position, innerAttackRange);

        Gizmos.color = new Color(0, 255, 0, 0.5f);
        Gizmos.DrawSphere(this.transform.position, visableRange);

        Gizmos.color = new Color(0, 0, 255, 0.5f);
        Vector3 pos = transform.position;
        if (enemy != null && EditorApplication.isPlaying)
            pos = enemy.startPosition;

        Gizmos.DrawSphere(pos, wanderDistance);
    }
}
