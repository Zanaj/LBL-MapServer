using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractionErrorCode
{
    Unknown,

    TooFarAway,
    InteractionReturnedNo,
    InvalidTarget,
    EntityNotInteractable,
    SelectedOptionIsOutOfBound,

    CannotAttackYet,

    Success = 99
}

public class EntityManager : MonoBehaviour
{
    public static EntityManager instance;

    public List<ItemSpawn> itemSpawns = new List<ItemSpawn>();
    public List<Entity> entities
    {
        get
        {
            List<Entity> _e = new List<Entity>();
            _e.AddRange(PlayerManager.instance.onlinePlayers);
            _e.AddRange(EnemyManager.instance.enemies);
            _e.AddRange(NPCManager.instance.NPCS);
            _e.AddRange(itemSpawns);

            return _e;
        }

    }

    private void Awake()
    {
        instance = this;
    }

    #region UseInteraction
    public void UseInteraction(Player requester, string entityGUID, int opiton, List<string> extraInfo)
    {
        Entity target;
        if (entityGUID == "NA")
            target = requester.target;
        else
            target = EntityManager.instance.entities.Find(x => x.entityGUID == entityGUID);

        if(target == null)
            return;
        
        InteractionType type = target.options[opiton];

        if (type != InteractionType.Attack)
        {
            if (!(target is IInteractable))
                return;

            IInteractable interactable = (IInteractable)target;
            if (!interactable.CanInteract(requester, extraInfo))
                return;

            interactable.InteractWith(requester, extraInfo);
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
    
    public InteractionErrorCode CanUseInteraction(Player requester, string entityGUID, int option, List<string> extraInfo)
    {
        if (entityGUID == "NA" && requester.target == null)
            return InteractionErrorCode.InvalidTarget;

        Entity target;
        if (entityGUID == "NA")
            target = requester.target;
        else
            target = EntityManager.instance.entities.Find(x => x.entityGUID == entityGUID);

        if (target == null)
            return InteractionErrorCode.InvalidTarget;

        if (!target.isInteractable && target.options.Length > 0)
            return InteractionErrorCode.EntityNotInteractable;

        if (option > target.options.Length-1)
            return InteractionErrorCode.SelectedOptionIsOutOfBound;

        InteractionType type = target.options[option];
        switch (type)
        {
            case InteractionType.Unknown:
                return InteractionErrorCode.Unknown;
            case InteractionType.Attack:
                return CanUseAttackAction(requester, option, extraInfo);
            default:
                return CanUseGeneralAction(requester, target, option, extraInfo);

        }
    }
    
    public InteractionErrorCode CanUseAttackAction(Player requester, int option, List<string> extraInfo)
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
                        return InteractionErrorCode.Success;
                    }
                    else
                    {
                        return InteractionErrorCode.CannotAttackYet;
                    }
                }
                else 
                {
                    Debug.Log("Requester Too far away! Distance: " + dis);
                    return InteractionErrorCode.TooFarAway;
                }
            }
            else
            {
                if(requester.skillBar.Length > attackType)
                {
                    //TODO: check for cooldown on normal attack
                    //TODO: check for distance;
                    //TODO: use skill;
                    return InteractionErrorCode.Success;
                }
                else
                {
                    Debug.Log("Requested a skill hotkey out of hotkey bar");
                    return InteractionErrorCode.SelectedOptionIsOutOfBound;
                }
            }
        }
        else
        {
            Debug.LogWarning(attackType + " is not a valid type");
            return InteractionErrorCode.SelectedOptionIsOutOfBound;
        }
    }

    public InteractionErrorCode CanUseGeneralAction(Player requester, Entity target, int option, List<string> extraInfo)
    {
        //TODO: Check distance
        Vector3 a = requester.transform.position;
        Vector3 b = target.transform.position;

        float dis = Vector3.Distance(a, b);
        bool isWithinDis = dis <= NetworkManager.instance.MAX_INTERACTION_DISTANCE;
        return isWithinDis ? InteractionErrorCode.Success : InteractionErrorCode.TooFarAway;
    }
    #endregion
}
