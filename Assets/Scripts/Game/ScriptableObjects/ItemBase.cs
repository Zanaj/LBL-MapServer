using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Content/GenericItem", order = 1)]
public class ItemData : ScriptableObject
{

    [TextArea()]
    public string desc;
    public string displayName;

    public Sprite icon;

    public int maxStack;
    public int cost;
    
    public int sellCost
    {   get
        {
            float fCost = cost;
            float val = fCost / 0.1f;

            return Mathf.RoundToInt(val);
        } 
    }
    public const float BASE_ITEM_SELL_BACK = 0.1f;
    public bool isStackable { get { return maxStack > 1; } }
}
