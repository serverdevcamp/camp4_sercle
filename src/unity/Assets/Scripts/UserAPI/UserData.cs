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
[Serializable]
public class UserAchieveDataArray
{
    public UserAchieveData[] jsonData;
}

[Serializable]
public class UserAchieveData
{
    public int id;
    public int listid;
    public string achieve;
}

[Serializable]
public class UserPlayData
{
    public int achievescore;
    public int victory;
    public int lose;
    public int kill;
    public int death;
    public int imageid;
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