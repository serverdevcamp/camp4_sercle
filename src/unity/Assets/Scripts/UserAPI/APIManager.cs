using System;
using UnityEngine;
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
       
        // 유니티에서 요구하는 Json 양식으로 변형.
        data = data.Insert(0, " { \"jsonData\" : ");
        data = data.Insert(data.Length, "}");

        UserAchieveDataArray apiData;
        apiData = JsonUtility.FromJson<UserAchieveDataArray>(data);

        for(int i = 0; i < apiData.jsonData.Length; i++)
        {
            UserAchieveData tmp = new UserAchieveData();
            tmp.id = apiData.jsonData[i].id;
            tmp.listid = apiData.jsonData[i].listid;
            tmp.achieve = apiData.jsonData[i].achieve;
            list.Add(tmp);
        }
     
        return list;
    }

}
