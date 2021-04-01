using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public static NPCManager instance;
    public List<Entity> NPCS;

    void Awake()
    {
        instance = this;
        NPCS = new List<Entity>();
    }
}
