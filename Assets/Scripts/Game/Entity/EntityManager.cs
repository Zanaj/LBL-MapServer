using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static EntityManager instance;

    public List<Entity> unclaimed = new List<Entity>();

    public List<Entity> entities
    {
        get
        {
            List<Entity> _e = new List<Entity>();
            _e.AddRange(PlayerManager.instance.onlinePlayers);
            _e.AddRange(EnemyManager.instance.enemies);
            _e.AddRange(NPCManager.instance.NPCS);
            _e.AddRange(unclaimed);

            return _e;
        }

    }

    void Awake()
    {
        instance = this;
    }

    private void FixedUpdate()
    {
        //List<Entity> checks = entities;
        //checks.RemoveAll(x => x.type == EntityType.Player);
        //checks.RemoveAll(x => x.type == EntityType.NPC);
        //checks.RemoveAll(x => x.type == EntityType.Unknown);

        //for (int i = 0; i < checks.Count; i++)
        //{
        //    Entity chk = checks[i];
        //    TimeSpan span = DateTime.Now - chk.respawnTimer;

        //    if(span.TotalSeconds >= chk.respawnSeconds)
        //    {
        //        chk.transform.position = chk.startPosition;
        //        chk.OnRespawn();
        //    }
        //}
    }

    #region UseInteraction
    public void UseInteraction(Player requester, int opiton, List<string> extraInfo)
    {
        Entity target = requester.target;
        InteractionType type = target.options[opiton];

        if (type != InteractionType.Attack)
        {
            
        }
        else
        {
            string parse = "";
            int attackType = 0;

            if (extraInfo.Count <= 0)
                parse = "-1";
            else
                parse = extraInfo[0];

            if(int.TryParse(parse, out attackType))
            {
                if(attackType < 0)
                {
                    requester.DoDamageTo(requester.target);
                }
                else
                {
                    //TODO: get skill and use it
                }
            }
        }

    }
    #endregion

    #region InteractionChecks
    public bool CanUseInteraction(Player requester, int option, List<string> extraInfo)
    {
        if(requester.target != null)
        {
            Entity target = requester.target;
            if (target.isInteractable)
            {
                if(target.options.Length > 0)
                {
                    if(option <= target.options.Length)
                    {
                        InteractionType type = target.options[option];
                        switch (type)
                        {
                            case InteractionType.Unknown:
                                return false;
                            case InteractionType.Attack:
                                return CanUseAttackAction(requester, option, extraInfo);
                                
                            case InteractionType.Examine:
                                return CanUseGeneralAction(requester, option, extraInfo);
                                
                            case InteractionType.Talk:
                                return CanUseGeneralAction(requester, option, extraInfo);
                                
                            case InteractionType.Shop:
                                return CanUseGeneralAction(requester, option, extraInfo);
                                
                            case InteractionType.NpcOption:
                                return CanUseGeneralAction(requester, option, extraInfo);
                                
                            case InteractionType.UseItem:
                                return CanUseGeneralAction(requester, option, extraInfo);
                                
                            case InteractionType.EatItem:
                                return CanUseGeneralAction(requester, option, extraInfo);
                                
                            case InteractionType.EquipItem:
                                return CanUseGeneralAction(requester, option, extraInfo);
                                
                            case InteractionType.DropItem:
                                return CanUseGeneralAction(requester, option, extraInfo);
                                
                            default:
                                return CanUseGeneralAction(requester, option, extraInfo);
                                
                        }
                    }
                    else { Debug.Log("Selected option is out of the range of options: " + option + "/" + target.options.Length); return false; }
                }
                else { Debug.Log("Target dont have any options"); return false; }
            }
            else { Debug.Log("target is not interactable"); return false; }
        }
        else { Debug.Log("Requster aint got no target!"); return false; }
    }

    public bool CanUseAttackAction(Player requester, int option, List<string> extraInfo)
    {
        string parse = "";
        int attackType = 0;

        if (extraInfo.Count <= 0)
            parse = "-1";
        else
            parse = extraInfo[0];

        if(int.TryParse(parse, out attackType))
        {
            Vector3 a = requester.transform.position;
            Vector3 b = requester.target.transform.position;

            float dis = Vector3.Distance(a, b);

            if (attackType < 0)
            {
                
                if(dis <= NetworkManager.instance.MAX_INTERACTION_DISTANCE)
                {
                    if (requester.CanAttack())
                    {
                        return true;
                    }
                    else
                    {
                        Debug.Log("User cannot attack yet!");
                        return false;
                    }
                }
                else 
                {
                    Debug.Log("Requester Too far away! Distance: " + dis);
                    return false;
                }
            }
            else
            {
                if(requester.skillBar.Length > attackType)
                {
                    //TODO: check for cooldown on normal attack
                    //TODO: check for distance;
                    //TODO: use skill;
                    return true;
                }
                else
                {
                    Debug.Log("Requested a skill hotkey out of hotkey bar");
                    return false;
                }
            }
        }
        else
        {
            Debug.LogWarning(attackType + " is not a valid type");
            return false;
        }
    }

    public bool CanUseGeneralAction(Player requester, int option, List<string> extraInfo)
    {
        //TODO: Check distance
        Vector3 a = requester.transform.position;
        Vector3 b = requester.target.transform.position;

        float dis = Vector3.Distance(a, b);
        return dis <= NetworkManager.instance.MAX_INTERACTION_DISTANCE;
    }
    #endregion
}
