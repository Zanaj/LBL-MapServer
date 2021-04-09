using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentDatabase : MonoBehaviour
{
    public static ContentDatabase instance;

    public SkillData[] skills;
    public ItemData[] items;

    private void Start()
    {
        instance = this;
    }

}
