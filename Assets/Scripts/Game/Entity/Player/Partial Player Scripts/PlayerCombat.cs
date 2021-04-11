using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player
{
    public static DateTime lastAttack;
    public string pendingTargetGUID;

    public SkillEntry[] skillBar;
    public List<SkillEntry> knownSkills;

    private void Combat_Start()
    {
        pendingTargetGUID = string.Empty;
        lastAttack = DateTime.Now;
        skillBar = new SkillEntry[PlayerConst.MAX_SKILLBAR_SLOTS];
    }

    private void Combat_Update()
    {

    }

    public bool CanAttack()
    {
        TimeSpan span = DateTime.Now - lastAttack;
        return span.TotalMilliseconds >= GetStat(Stat.Attack_Speed, true);
    }

}
