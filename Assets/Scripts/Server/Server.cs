using System.Collections;
using System.Net;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class Server : MonoBehaviour
{
    static Listener _listener = new Listener();
    int _port = 7777;

    public static GameRoom Room = new();
    private bool _initDone;


    Thread _jobFlushThread;
    static void FlushRoom()
    {
        Room.Push(() => Room.Flush());
        JobTimer.Instance.Push(FlushRoom, 40);
    }
    public void Init()
    {
        string host = Dns.GetHostName();
        IPHostEntry entry = Dns.GetHostEntry(host);
        IPAddress ipAddress = IPAddress.Parse("172.25.2.137");
        

        IPEndPoint endPoint = new IPEndPoint(ipAddress, _port);

        _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
        JobTimer.Instance.Push(FlushRoom);

        _initDone = true;

        //_jobFlushThread = new Thread(CorJobFlush);
        //_jobFlushThread.Start();
    }

    private void Update()
    {
        if (!_initDone) return;

        JobTimer.Instance.Flush();
    }

    private void OnDestroy()
    {
        if(_jobFlushThread != null)
            _jobFlushThread.Interrupt();
    }

    void CorJobFlush()
    {
        while (true)
        {
            try
            {
                JobTimer.Instance.Flush();
            }
            catch (ThreadInterruptedException e)
            {
                Debug.Log($"Exit : {e}");
            }
        }
    }
}
