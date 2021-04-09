using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EffectData : ScriptableObject
{
    public abstract bool CanApply(Entity owner, Entity target);
    public abstract void Apply(Entity owner, Entity target); 
}
