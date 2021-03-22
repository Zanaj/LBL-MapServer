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

    private void Start()
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
        int what = cmd.ExecuteNonQuery();
        MySqlDataReader reader = cmd.ExecuteReader();

        int accountID = -1;

        if (reader.Read())
        {
            player.characterID = reader.GetInt32(0);
            accountID = reader.GetInt32(1);
            player.name = reader.GetString(2);
            player.bodyType = reader.GetInt32(3);
            player.genativ = reader.GetString(4);
            player.referal = reader.GetString(5);
            player.height = reader.GetFloat(7);
            player.weight = reader.GetFloat(8);

            reader.Dispose();
            reader.Close();
            cmd.Dispose();

            query = "SELECT * FROM lbldb.accounts WHERE id=" + accountID;
            Debug.Log(query);
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