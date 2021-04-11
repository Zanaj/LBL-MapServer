using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Database : MonoBehaviour
{
    public static Database instance;
    public MySqlConnection connection;
    public static string host = "34.70.181.250";
    public static string database = "lbldb";
    public static string uid = "root";
    public static string password = "czu26ueh";
    public string characterSet = "utf8";

    //Initialize all the database stuff.
    private void Start()
    {
        instance = this;

        string connectionString = "SERVER=" + host + ";" + "DATABASE=" +
        database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";CharSet="+characterSet+";";

        connection = new MySqlConnection(connectionString);
        OpenConnection();
        CloseConnection();
    }

    public bool OpenConnection()
    {
        try
        {
            connection.Open();
            return true;
        }
        catch (MySqlException ex)
        {
            //When handling errors, you can your application's response based 
            //on the error number.
            //The two most common error numbers when connecting are as follows:
            //0: Cannot connect to server.
            //1045: Invalid user name and/or password.
            switch (ex.Number)
            {
                case 0:
                    Debug.LogError("Cannot connect to server.  Contact administrator");
                    break;

                case 1045:
                    Debug.LogError("Invalid username/password, please try again");
                    break;
                default:
                    Debug.LogError(ex.Message);
                    break;
            }
            return false;
        }
    }

    public bool CloseConnection()
    {
        try
        {
            connection.Close();
            return true;
        }
        catch (MySqlException ex)
        {
            Debug.LogError(ex.Message);
            return false;
        }
    }

    public MySqlCommand GetCommand(string query)
    {
        return new MySqlCommand(query, connection);
    }
}
