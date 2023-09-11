using System.Collections;
using System.Net;
using UnityEngine;

public class Server : MonoBehaviour
{
    static Listener _listener = new Listener();
    int _port = 7777;

    public static GameRoom Room = new();
    private bool _initDone;

    static void FlushRoom()
    {
        Room.Push(() => Room.Flush());
        JobTimer.Instance.Push(FlushRoom, 250);
    }
    public void Init()
    {
        string host = Dns.GetHostName();
        IPHostEntry entry = Dns.GetHostEntry(host);
        IPAddress ipAddress = IPAddress.Parse("172.25.2.70");
        

        IPEndPoint endPoint = new IPEndPoint(ipAddress, _port);

        _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
        JobTimer.Instance.Push(FlushRoom);

        _initDone = true;

        StartCoroutine(CorJobFlush());
    }
    private void Update()
    {
        Debug.Log("BB");
    }
    IEnumerator CorJobFlush()
    {
        while (!_initDone)
        {
            yield return new WaitForSeconds(1);
        }

        while (true)
        {
            JobTimer.Instance.Flush();

            Debug.Log("AA");
            yield return new WaitForSeconds(0.001f);
        }
    }
}
