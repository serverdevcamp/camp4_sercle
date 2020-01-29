using UnityEngine;
using System.Collections;

public class MatchingManager : MonoBehaviour
{
    public static MatchingManager instance;

    // 네트워크 매니저
    private NetworkManager networkManager;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // Use this for initialization
    void Start()
    {
        networkManager = transform.parent.GetComponent<NetworkManager>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnReceiveMatchingPacket(MatchingPacketId id, byte[] data)
    {

    }
}
