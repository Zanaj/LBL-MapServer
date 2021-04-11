using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    public Player player;
    public ItemData item;
    public int amount;
    public bool addItem;

    // Start is called before the first frame update
    void Update()
    {
        if (addItem)
        {
            ItemEntry itemEntry = new ItemEntry(item, amount);
            player.AddItems(itemEntry);

            itemEntry = new ItemEntry(item, amount);
            player.AddItems(itemEntry);

            itemEntry = new ItemEntry(item, amount);
            InventoryErrorCode error = player.RemoveItem(itemEntry, false);

            InventorySync sync = new InventorySync();
            sync.player = player;
            sync.Serialize();
            int id = NetworkManager.instance.connectionToAccountID.Where(x => x.Value == player.characterID).First().Key;
            NetworkManager.Send(id, sync);
            addItem = false;
        }
    }
}
