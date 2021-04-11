using System;

public class Account
{
    public int ID;
    public string username;
    public string password;
    public string email;
    public DateTime lastLoggedIn;
    public bool isOnline;
}

public static class PlayerConst
{
    public static float TARGET_VIEW_DISTANCE = 50;
    public static int MAX_SKILLBAR_SLOTS = 5;
}