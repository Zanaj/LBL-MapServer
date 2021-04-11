using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player
{
    [Header("Debug Options")]

    public bool shouldDebug = true;

    [Space(10)]

    public float health = 0;
    public float mana = 0;
    public float stamina = 0;

    [Space(10)]

    public float wound = 0;
    public float exhaustion = 0;
    public float hunger = 0;

    [Space(10)]

    public float strength;
    public float intellegence;
    public float willpower;
    public float luck;

    // Update is called once per frame
    private void Debug_Update()
    {
        if (shouldDebug)
        {
            health = GetVital(Stat.Health).currentValue;
            mana = GetVital(Stat.Mana).currentValue;
            stamina = GetVital(Stat.Stamina).currentValue;

            wound = GetVital(Stat.Health).debuffValue;
            exhaustion = GetVital(Stat.Mana).debuffValue;
            hunger = GetVital(Stat.Stamina).debuffValue;

            strength = GetStat(Stat.Strength, true);
            intellegence = GetStat(Stat.Intelligence, true);
            willpower = GetStat(Stat.Willpower, true);
            luck = GetStat(Stat.Luck, true);
        }
    }
}
