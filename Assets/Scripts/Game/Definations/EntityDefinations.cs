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
    Attack_Speed,
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

public enum EntityType
{
    Unknown,
    Interactable,
    Pickup,
    Enemy,
    Player,
    NPC,
    Special,
}

public enum InteractionType
{
    Unknown,
    Attack,
    Examine,
    Talk,
    Shop,
    NpcOption,
    OpenDoor,
    PickItem,
    UseItem,
    EatItem,
    EquipItem,
    DropItem,
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
        set
        {
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
