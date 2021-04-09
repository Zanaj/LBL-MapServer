using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillData : ScriptableObject
{
    [TextArea()]
    public string desc;
    public string displayName;
    public Sprite icon;
    public Stat stat;

    public float cost;
    public float innerRange;
    public float outerRange;
    
    public int stunTimeMilliseconds;
    public int cooldownTimeMilliseconds;

    public bool isMagical;

    public bool isStun { get { return stunTimeMilliseconds > 0; } }
    public bool isRanged { get { return outerRange > 0; } }

    public bool isMeele { get { return !isRanged; } }

    public EffectData[] effects;
}
