using System;
//using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class APIManager
{
    //JArray apiData;

    public APIManager()
    {
    }
    public List<UserAchieveData> GetAchieveData(string data)
    {
        List<UserAchieveData> list = new List<UserAchieveData>();
        //apiData = JArray.Parse(data);
        //foreach (JObject jsonData in apiData)
        //{
        //    UserAchieveData tmp = new UserAchieveData();
        //    tmp.id = Int32.Parse(jsonData["id"].ToString());
        //    tmp.listid = Int32.Parse(jsonData["id"].ToString());
        //    tmp.achieve = jsonData["listid"].ToString();
        //    list.Add(tmp);
        //}
        return list;
    }

}
