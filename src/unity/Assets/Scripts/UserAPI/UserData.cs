using System;

[Serializable]
public class UserData
{
    public string login;
    public string email;
    public string id;
    public string token;
    public string username;
    public int roomNum;
    public int playerCamp;
}

public class UserAchieveData
{
    public int id;
    public int listid;
    public string achieve;

    public override string ToString()
    {
        string str = "";
        str += id.ToString() + " ";
        str += listid.ToString() + " ";
        str += achieve.ToString() + " ";
        return str;
    }
}

public class UserPlayData
{
    public int achievescore;
    public int victory;
    public int lose;
    public int kill;
    public int death;
    public int damage;

    public override string ToString()
    {
        string str = "";
        str += achievescore.ToString() + " ";
        str += victory.ToString() + " ";
        str += lose.ToString() + " ";
        str += kill.ToString() + " ";
        str += death.ToString() + " ";
        str += damage.ToString();
        return str;
    }
}