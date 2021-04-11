using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public partial class Player : Entity
{
    public delegate void _OnLevelUp(int lastLevel, int currentLevel);
    public event _OnLevelUp OnLevelUp;

    public Account accountData;
    
    public int characterID;
    public DateTime lastRebirth;

    public int guildId;
    public int ap;

    public float exp;
    public int level { get { return Mathf.RoundToInt(exp / 100); } }
    public int totalLevel;

    public bool hasInitialized;
    
    public override bool isInteractable => true;
    public override EntityType type => EntityType.Player;

    private void Start()
    {
        Initialize();
        entityGUID = Guid.NewGuid().ToString();
        hasInitialized = true;

        Inventory_Start();
        Movement_Start();
        Combat_Start();
    }

    private void FixedUpdate()
    {
        Debug_Update();
        Movement_Update();
        Combat_Update();
    }

    public override void MakeEntityPacket(BinaryWriter writer)
    {
        writer.Write((int)EntityType.Player);
        writer.Write(displayName);

        writer.Write(entityGUID);
        writer.Write(isInteractable);

        writer.Write(transform.position.x);
        writer.Write(transform.position.y);
        writer.Write(transform.position.z);

        writer.Write(0);

        writer.Write(characterID);
        writer.Write(bodyType);
        writer.Write(genativ);
        writer.Write(referal);
        writer.Write(rotation);

        writer.Write(hasInitialized);
        if (hasInitialized)
        {
            writer.Write(GetVital(Stat.Health).maxValue);
            writer.Write(GetVital(Stat.Health).currentValue);
            writer.Write(GetVital(Stat.Health).debuffValue);

            writer.Write(GetVital(Stat.Stamina).maxValue);
            writer.Write(GetVital(Stat.Stamina).currentValue);
            writer.Write(GetVital(Stat.Stamina).debuffValue);

            writer.Write(GetVital(Stat.Mana).maxValue);
            writer.Write(GetVital(Stat.Mana).currentValue);
            writer.Write(GetVital(Stat.Mana).debuffValue);
        }
    }

    public void GiveExp(float exp)
    {
        int lastLevel = level;
        this.exp += exp;

        int newLevel = level;
        if(newLevel > lastLevel)
        {
            exp = 0;
            totalLevel++;
            OnLevelUp?.Invoke(lastLevel, newLevel);
        }
    }

    public override float GetStat(Stat stat, bool includeBuffs)
    {
        float equipmentValue = 0;
        if (includeBuffs)
        {
            foreach (EquipableItem equipment in equipped.Values)
            {
                List<StatPair> allStats = equipment.buffs.Where(x => x.stat == stat).ToList();
                if (allStats.Count <= 0)
                    continue;

                equipmentValue += allStats.Sum(x => x.value);
            }
        }

        return base.GetStat(stat, includeBuffs) + equipmentValue;
    }
}