using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class Player
{
    public int inventorySize;

    [SerializeField]
    public List<ItemEntry> inventory = new List<ItemEntry>();
    public Dictionary<EquipmentSlot, EquipableItem> equipped;

    private void Inventory_Start()
    {
        equipped = new Dictionary<EquipmentSlot, EquipableItem>();
    }

    public InventoryErrorCode CanAddItems(params ItemEntry[] items)
    {
        //If we pretend all items are nonstackable then well and theres enough space go ahead.
        if (inventory.Count + items.Length <= inventorySize)
            return InventoryErrorCode.Success;

        for (int i = items.Length - 1; i >= 0; i--)
        {
            ItemEntry item = items[i];

            //This loop we check for stacks if its not stackable no need to check.
            if (!item.data.isStackable)
                continue;

            //If this item doesn't exist in the inventory no need to check if it can be stacked
            if (!inventory.Exists(x => x.data == item.data))
                continue;

            //Get all entries where this item is
            ItemEntry[] allItems = inventory
                .Where(x => x.data == item.data)
                .ToArray();

            //Sum all space left from the entries.
            int spaceLeftInStacks = allItems.Sum(x => x.leftFromStack);

            //If theres less space than required then return false
            if (spaceLeftInStacks < item.amount)
                return InventoryErrorCode.NotEnoughSpace;
        }

        return InventoryErrorCode.Success;
    }

    public InventoryErrorCode HaveItem(ItemEntry item)
    {
        InventoryErrorCode errorCode = InventoryErrorCode.Unknown;
        if (!inventory.Exists(x => x.data == item.data))
            return InventoryErrorCode.DoesNotOwnItem;

        ItemEntry inventoryItem = inventory.Find(x => x.data == item.data);
        if (inventoryItem.amount < item.amount)
            return InventoryErrorCode.DoesNotHaveEnoughItem;

        if (inventoryItem.amount >= item.amount)
            return InventoryErrorCode.Success;

        return errorCode;
    }

    public void AddItems(params ItemEntry[] items)
    {
        List<ItemEntry> itemsLeft = items.ToList();

        if (CanAddItems(items) != InventoryErrorCode.Success)
            return;

        foreach (ItemEntry item in itemsLeft)
        {
            float fMax = item.data.maxStack;
            float fAmn = item.amount;

            int sumOfEmptyStacks = inventory.Sum(x => x.leftFromStack);
            if (sumOfEmptyStacks > 0)
            {
                List<ItemEntry> allHalfStackedItems = inventory.FindAll(x => x.leftFromStack > 0);
                for (int i = 0; i < allHalfStackedItems.Count; i++)
                {
                    ItemEntry notFullystacked = allHalfStackedItems[i];
                    if (item.amount < notFullystacked.leftFromStack)
                    {
                        notFullystacked.amount += item.amount;
                        continue;
                    }

                    int leftToStack = notFullystacked.leftFromStack;
                    notFullystacked.amount += leftToStack;
                    item.amount -= leftToStack;
                }
            }

            int cnt = Mathf.CeilToInt(fAmn / fMax);
            
            for (int i = 0; i < cnt; i++)
            {
                int maxStack = item.data.maxStack;
                int amount = item.amount;

                int toAdd = amount < maxStack ? amount : maxStack;
                ItemEntry newItem = new ItemEntry(item.data, toAdd);
                AddItem(newItem);

                item.amount -= toAdd;
            }
        }

        //TODO: update player's inventory!
    }

    public InventoryErrorCode RemoveItem(ItemEntry item, bool ignoreAmount)
    {
        //TODO: update so you can write stuff over the stack.

        if (!inventory.Exists(x => x.data == item.data))
            return InventoryErrorCode.DoesNotOwnItem;

        ItemEntry inventoryItem = inventory.Find(x => x.data == item.data);
        if (inventoryItem.amount <= item.amount || ignoreAmount)
            inventoryItem.amount -= item.amount;

        if(inventoryItem.amount <= 0)
            inventory.Remove(inventoryItem);

        return InventoryErrorCode.Success;
    }

    private void AddItem(ItemEntry item)
    {
        if (item.amount <= 0)
            return;

        inventory.Add(item);
    }

    public InventoryErrorCode EquipItem(EquipableItem equipment)
    {
        if (equipped.ContainsKey(equipment.slot))
        {
            EquipableItem previousEquipment = equipped[equipment.slot];
            ItemEntry itemEntry = new ItemEntry(previousEquipment, 1);

            InventoryErrorCode error = CanAddItems(itemEntry);
            if (error != InventoryErrorCode.Success)
                return error;

            equipped.Remove(equipment.slot);
            AddItem(itemEntry);
        }

        equipped.Add(equipment.slot, equipment);
        return InventoryErrorCode.Success;
    }
}
