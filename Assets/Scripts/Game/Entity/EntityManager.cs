using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static EntityManager instance;
    public List<Entity> entities;

    void Awake()
    {
        instance = this;
        entities = new List<Entity>();
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
                    if(target.options.Length < option)
                    {
                        InteractionType type = target.options[option];
                        switch (type)
                        {
                            case InteractionType.Unknown:
                                return false;
                                break;
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
                    else { return false; }
                }
                else { return false; }
            }
            else { return false; }
        }
        else { return false; }
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
                
                if(dis < NetworkManager.instance.MAX_INTERACTION_DISTANCE)
                {
                    if (requester.CanAttack())
                    {
                        return true;
                    }
                    else { return false; }
                }
                else { return false; }
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
                else { return false; }
            }
        }
        else { return false; }
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
