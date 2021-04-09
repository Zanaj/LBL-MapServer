using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEntry
{
    public SkillData data;
    public DateTime lastCast;

    #region Ease of reference
    public string desc => data.desc;
    public string displayName => data.displayName;
    public Sprite icon => data.icon;
    public Stat stat => data.stat;

    public float cost => data.cost;
    public float innerRange => data.innerRange;
    public float outerRange => data.outerRange;

    public int stunTimeMilliseconds => data.stunTimeMilliseconds;
    public int cooldownTimeMilliseconds => data.cooldownTimeMilliseconds;

    public bool isStun => data.isStun;
    public bool isRanged => data.isRanged;

    public EffectData[] effects => data.effects;
    #endregion

    public SkillEntry(SkillData data)
    {
        this.data = data;
        lastCast = DateTime.Now;

    }

    public bool CanCast(Entity owner, Entity target)
    {
        int cnt = 0;

        TimeSpan span = DateTime.Now - lastCast;
        if (span.TotalMilliseconds < cooldownTimeMilliseconds)
            return false;

        foreach (EffectData effect in effects)
            cnt += effect.CanApply(owner, target) ? 0 : 1;

        return cnt == 0;
    }

    public void Cast(Entity owner, Entity target)
    {
        if (CanCast(owner, target)) {
            foreach (EffectData effect in effects)
                effect.Apply(owner, target);

            lastCast = DateTime.Now;
        }
    }
}
