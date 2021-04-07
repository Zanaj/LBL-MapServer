using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Stat
{
    Health = 2,
    Wound,
    Stamina,
    Hunger,
    Mana,
    Exhaustion,

    Strength = 20,
    Intelligence,
    Dexterity,
    Willpower,
    Luck,

    Balance = 100,
    Min_Damage,
    Max_Damage,

    Magic_Bonus = 1000,
    Stun,
}

public enum StatType
{
    Unknown,
    Vital,
    Major,
    Minor,
    Bonus
}

public class Vital
{
    public static float MAX_DEBUFF_VALUE = 50;

    public Stat vital;
    public Stat debuffVital;

    private float _debuffedMax;
    private float _debuffValue;
    public float debuffValue
    { 
        get { return _debuffValue; } 
        set{
            float procentage = _debuffValue / maxValue;
            if (procentage > MAX_DEBUFF_VALUE)
                procentage = MAX_DEBUFF_VALUE;

            _debuffValue -= maxValue * procentage;
        } 
    }

    public float currentValue;
    public float debuffedMax
    {
        set { _debuffedMax = value; } 
        get { return maxValue - debuffValue; }
    }

    public float maxValue;

    public Vital(Stat vital, float maxValue)
    {
        this.vital = vital;
        int index = (int)vital;
        index++;

        debuffVital = (Stat)index;
        debuffValue = 0;

        this.maxValue = maxValue;
        currentValue = maxValue;
    }

    public void SetCurrent(float value)
    {
        value = Mathf.Clamp(value, 0, debuffedMax);
        currentValue = value;
    }
}

public class EntityStats : MonoBehaviour
{
    private Dictionary<Stat, int> stats = new Dictionary<Stat, int>();
    private List<Vital> vitals;

    private bool hasBeenHit;
    private DateTime lastHit;
    private float stun;

    public delegate void _OnDamageDealt(EntityStats target);
    public delegate void _OnDamageRecieved(EntityStats attacker);

    public event _OnDamageDealt OnDamageDealt;
    public event _OnDamageRecieved OnDamageRecieved;

    public void Initialize()
    {
        //TODO: universal database for base values
        float vitalBase = 100;
        int majorBase = 30;
        int minorBase = 1;
        int bonusBase = 0;

        stats = new Dictionary<Stat, int>();
        vitals = new List<Vital>();

        Vital health = new Vital(Stat.Health, vitalBase);
        Vital stamina = new Vital(Stat.Stamina, vitalBase);
        Vital mana = new Vital(Stat.Mana, vitalBase);

        vitals.Add(health);
        vitals.Add(stamina);
        vitals.Add(mana);

        List<Stat> allStats = Enum.GetValues(typeof(Stat)).Cast<Stat>().ToList();
        for (int i = 0; i < allStats.Count; i++)
        {
            Stat currStat = allStats[i];
            int index = (int)currStat;
            if (!stats.ContainsKey(currStat))
                stats.Add(currStat,0);

            if (index >= 20 && index < 100)
                stats[currStat] = majorBase;
            else if (index >= 100 && index < 1000)
                stats[currStat] = minorBase;
            else
                stats[currStat] = bonusBase;
        }

        UpdateMinorStats();
    }

    public StatType GetStatType(Stat stat)
    {
        int index = (int)stat;
        StatType type = StatType.Unknown;
        type = index < 2 ? StatType.Unknown : type;
        type = index >= 2 && index < 100 ? StatType.Major : type;
        type = index >= 100 && index < 1000 ? StatType.Minor : type;
        type = index >= 1000 ? StatType.Bonus : type;

        return type;
    }

    public void SetStat(Stat stat, float value)
    {
        int index = (int)stat;
        StatType type = GetStatType(stat);
        
        if(type != StatType.Unknown)
        {
            if(type == StatType.Vital)
            {
                if (index % 2 == 0)
                {
                    Vital vital = vitals.Find(x => x.vital == stat);
                    if (vital != null)
                        vital.currentValue = value;
                }
                else
                {
                    Vital vital = vitals.Find(x => x.debuffVital == stat);
                    if (vital != null)
                        vital.debuffValue = value;
                }
            }
            else
            {
                if (stats.ContainsKey(stat))
                {
                    stats[stat] = Mathf.RoundToInt(value);
                }
            }
        }
    }

    public float GetStat(Stat stat)
    {
        int index = (int)stat;
        StatType type = GetStatType(stat);

        if (type != StatType.Unknown)
        {
            if(type == StatType.Vital)
            {
                if(index % 2 == 0)
                {
                    Debug.LogWarning("Trying to get vital with GetStat please use GetVital. Will default to returning currentValue");
                    Vital vital = vitals.Find(x => x.vital == stat);
                    if (vital != null)
                        return vital.currentValue;
                    else
                        return -1;
                }
                else
                {
                    Vital vital = vitals.Find(x => x.vital == stat);
                    if (vital != null)
                        return vital.debuffValue;
                    else
                        return -1;
                }
            }
            else
            {
                if (stats.ContainsKey(stat))
                    return stats[stat];
                else
                    return -1;
            }
        }
        else { return -1; }
    }

    public enum VitalSetType
    {
        ChangeMax,
        ChangeCurrentValue,
        ChangeDebuff,
    }

    public void LoadVital(Stat stat, float currentValue, float debuffValue)
    {
        SetVital(stat, currentValue, VitalSetType.ChangeCurrentValue);
        SetVital(stat, debuffValue, VitalSetType.ChangeDebuff);
    }

    public void SetVital(Stat stat, float value, VitalSetType type)
    {
        Vital vital = GetVital(stat);

        switch (type)
        {
            case VitalSetType.ChangeMax:
                vital.maxValue = value;
                break;
            case VitalSetType.ChangeCurrentValue:
                vital.SetCurrent(value);
                break;
            case VitalSetType.ChangeDebuff:
                vital.debuffValue = value;
                break;
        }

        UpdateMinorStats();
    }

    public Vital GetVital(Stat stat)
    {
        if(vitals.Exists(x => x.vital == stat))
        {
            Vital rtn = vitals.Find(x => x.vital == stat);
            return rtn;
        }
        else
        {
            //TODO: do proper error checking.

            Debug.LogError("Couldnt find vital of: " + stat);
            return null;
        }
    }

    public void UpdateMinorStats()
    {
        float strength = GetStat(Stat.Strength);
        float intelligence = GetStat(Stat.Intelligence);
        float dexterity = GetStat(Stat.Dexterity);
        float willpower = GetStat(Stat.Willpower);
        float luck = GetStat(Stat.Luck);

        SetStat(Stat.Max_Damage, strength / 2.5f);
        SetStat(Stat.Min_Damage, strength / 3f);
    }

    public bool CanDoDamage()
    {

        if (hasBeenHit)
        {
            TimeSpan span = DateTime.Now - lastHit;
            if(span.TotalSeconds >= stun)
            {
                lastHit = DateTime.MinValue;
                hasBeenHit = false;
                stun = 0;
                return true;
            }
            else { return false; }
        }
        else { return true; }
    }

    public virtual void Death() { }

    public void DoDamageTo(EntityStats target)
    {
        if (CanDoDamage())
        {
            Vital enemyHealth = target.GetVital(Stat.Health);
            float myMinDmg = GetStat(Stat.Min_Damage);
            float myMaxDmg = GetStat(Stat.Max_Damage);
            float myStun = GetStat(Stat.Stun);

            target.stun = myStun;
            target.hasBeenHit = true;
            target.lastHit = DateTime.Now;

            double scalar = new System.Random().NextDouble();
            float dmg = (float)(scalar * (myMaxDmg - myMinDmg) + myMinDmg);
            
            float health = enemyHealth.currentValue;
            health -= dmg;

            OnDamageDealt?.Invoke(target);

            health = Mathf.Clamp(health, 0, float.MaxValue);
            enemyHealth.SetCurrent(health);
            if (health <= 0)
                target.Death();

            target.OnDamageRecieved?.Invoke(this);
        }
        else { Debug.LogWarning("Aint allowed to do that"); }
    }
}
