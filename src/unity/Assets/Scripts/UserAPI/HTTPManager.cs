using System.Net;
using System.IO;
using System.Text;


public class HTTPManager
{
    string url;
    string res = string.Empty;
    string postData = string.Empty;
    byte[] sendData;
    HttpWebRequest request;
    HttpWebResponse resp;
    Stream requestStream;

    public HTTPManager()
    {
        url = Info.URL;
    }

    //이메일과 비밀번호를 입력받아 Flask서버로 로그인 요청 
    public string LoginReq(string email, string pw)
    {
        request = Init("clientlogin");      //URL 경로 이름
        request.Method = "POST";
        postData = string.Format("email={0}&password={1}", email, pw);      //데이터 포맷 방법.
        SendData(postData);
        return ReceiveData(request);
    }

    public string UserCacheReq(string email)
    {
        request = Init("clientusercache");
        request.Method = "POST";
        postData = string.Format("email={0}", email);
        SendData(postData);
        return ReceiveData(request);

    }

    public string DestroyUserCache(string email)
    {
        request = Init("delet");
        request.Method = "POST";
        postData = string.Format("email={0}", email);
        SendData(postData);
        return ReceiveData(request);
    }
    //URL Path를 입력받아 request객체 생성
    private HttpWebRequest Init(string req)
    {
        HttpWebRequest tmp;
        tmp = (HttpWebRequest)WebRequest.Create(url + req);
        tmp.Timeout = 30 * 1000;
        tmp.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
        return tmp;
    }

    //C#에서 서버로 데이터를 보내기 위해 byte로 변환후 전송.
    private void SendData(string data)
    {
        sendData = UTF8Encoding.UTF8.GetBytes(data);
        requestStream = request.GetRequestStream();
        requestStream.Write(sendData, 0, sendData.Length);
        requestStream.Close();
    }

    //Response데이터 받아오기
    private string ReceiveData(HttpWebRequest req)
    {
        using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
        {
            HttpStatusCode status = resp.StatusCode;

            Stream stream = resp.GetResponseStream();
            using (StreamReader sr = new StreamReader(stream))
            {
                res = sr.ReadToEnd();
                return res;
            }
        }
    }

    //전체 업적 리스트 Json
    public string AchieveListDataReq()
    {
        request = Init("get-achieve-data");
        request.Method = "POST";
        postData = string.Format("");
        SendData(postData);
        return ReceiveData(request);
    }

    //유저가 달성한 업적 리스트 Json
    public string UserAchieveListDataReq(string id)
    {
        request = Init("get-user-achieve-data");
        request.Method = "POST";
        postData = string.Format("id={0}", id);
        SendData(postData);
        return ReceiveData(request);
    }
    public string UserDataReq(string id)
    {
        request = Init("get-data");
        request.Method = "POST";
        postData = string.Format("id={0}", id);
        SendData(postData);
        return ReceiveData(request);
    }

    //승부 여부에 따른 전적 업데이트
    public string UpdateUserWinReq(string id, bool winFlag)
    {
        if (winFlag)
        {
            request = Init("update-user-victory");
            request.Method = "POST";
            postData = string.Format("id={0}", id);
            SendData(postData);
            return ReceiveData(request);
        }
        else
        {
            request = Init("update-user-lose");
            request.Method = "POST";
            postData = string.Format("id={0}", id);
            SendData(postData);
            return ReceiveData(request);
        }
    }

    //게임 결과에 따른 업적 업데이트 및 업적 점수 업데이트
    public string UpdateAchieveReq(string id, UserPlayData data)
    {
        request = Init("update-user-achieve");
        request.Method = "POST";
        postData = string.Format("id={0}&kill={1}&death={2}&damage={3}", id, data.kill, data.death, data.damage);
        SendData(postData);
        return ReceiveData(request);
    }
}
