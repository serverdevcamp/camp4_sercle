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
        url = "http://13.125.252.198:8082/";     
        
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
}
