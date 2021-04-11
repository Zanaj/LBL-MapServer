using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using System;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance = null;
    public GameObject playerPrefab;
    public List<Player> onlinePlayers;
    public PlayerManager instanceReference;

    public float baseSpeed = 5;

    private void Awake()
    {
        Debug.Log("Initializing PlayerManager");

        if (instance != null)
            Destroy(this);

        instance = this;
        onlinePlayers = new List<Player>();
    }

    private void Update()
    {
        instanceReference = instance;
    }

    public int PlayerConnected(int characterID)
    {
        GameObject gObj = Instantiate(playerPrefab) as GameObject;
        Player player = gObj.GetComponent<Player>();

        Database.instance.OpenConnection();
        string query = "SELECT * FROM lbldb.characters WHERE id=" + characterID;
        MySqlCommand cmd = Database.instance.GetCommand(query);
        cmd.ExecuteNonQuery();
        MySqlDataReader reader = cmd.ExecuteReader();

        int accountID = -1;

        if (reader.Read())
        {
            player.characterID = reader.GetInt32(0);
            accountID = reader.GetInt32("accountID");
            player.displayName = reader.GetString("name");
            player.guildId = reader.GetInt32("guildId");
            player.lastRebirth = DateTime.Parse(reader.GetString("lastRebirth"));
            player.ap = reader.GetInt32("ap");
            player.exp = reader.GetFloat("exp");
            player.totalLevel = reader.GetInt32("totalLevel");

            EntityStats stats = new EntityStats();
            stats.Initialize();
            stats.SetStat(Stat.Strength, reader.GetInt32("strength"));
            stats.SetStat(Stat.Intelligence, reader.GetInt32("intelligence"));
            stats.SetStat(Stat.Dexterity, reader.GetInt32("dexterity"));
            stats.SetStat(Stat.Willpower, reader.GetInt32("wisdom"));
            stats.SetStat(Stat.Luck, reader.GetInt32("luck"));

            stats.LoadVital(Stat.Health, reader.GetInt32("health"), reader.GetInt32("wound"));
            stats.LoadVital(Stat.Stamina, reader.GetInt32("stamina"), reader.GetInt32("hunger"));
            stats.LoadVital(Stat.Mana, reader.GetInt32("mana"), reader.GetInt32("exhaustion"));

            reader.Dispose();
            reader.Close();
            cmd.Dispose();

            query = "SELECT * FROM lbldb.accounts WHERE id=" + accountID;
            cmd = Database.instance.GetCommand(query);
            reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                Account newAcc = new Account();
                newAcc.ID = reader.GetInt32(0);
                newAcc.username = reader.GetString(1);
                newAcc.password = reader.GetString(2);
                newAcc.email = reader.GetString(3);
                newAcc.lastLoggedIn = DateTime.Parse(reader.GetString(4));
                newAcc.isOnline = true;
                
                reader.Dispose();
                reader.Close();
                cmd.Dispose();

                query = "SELECT * FROM lbldb.characterdesigns WHERE characterId=" + characterID;
                cmd = Database.instance.GetCommand(query);
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    player.bodyType = reader.GetInt32("bodyType");
                    player.referal = reader.GetString("referalPronoun");
                    player.genativ = reader.GetString("ownershipPronoun");

                    reader.Dispose();
                    reader.Close();
                    cmd.Dispose();

                    player.accountData = newAcc;
                    onlinePlayers.Add(player);
                    Database.instance.CloseConnection();

                    return characterID;
                }
                else
                {
                    Destroy(gObj);
                    reader.Dispose();
                    reader.Close();
                    cmd.Dispose();
                    Database.instance.CloseConnection();
                    return -1;
                }
            }
            else
            {
                Destroy(gObj);
                reader.Dispose();
                reader.Close();
                cmd.Dispose();
                Database.instance.CloseConnection();
                return -1;
            }
        }
        else
        {
            Destroy(gObj);
            reader.Dispose();
            reader.Close();
            cmd.Dispose();
            Database.instance.CloseConnection();
            return -1;
        }
    }


    public void PlayerDisconnected(int connectionID)
    {
        int characterID = NetworkManager.instance.connectionToAccountID[connectionID];
        
        if (onlinePlayers.Exists(x => x.characterID == characterID))
        {
            Player player = onlinePlayers.Find(x => x.characterID == characterID);
            
            onlinePlayers.Remove(player);
            Destroy(player.gameObject);
        }

        NetworkManager.instance.connectionToAccountID.Remove(connectionID);
    }
}