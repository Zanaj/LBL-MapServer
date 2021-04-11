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
