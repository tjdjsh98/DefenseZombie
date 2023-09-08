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
        IPAddress ipAddress = entry.AddressList[0];
        
        IPEndPoint endPoint = new IPEndPoint(ipAddress, _port);

        _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
        JobTimer.Instance.Push(FlushRoom);

        _initDone = true;
    }

    private void Update()
    {
        if (!_initDone) return;

        S_AnswerEnterGame packet = new S_AnswerEnterGame() { permission = true, playerId = 1 };
        Room.Broadcast(packet.Write());

        JobTimer.Instance.Flush();
    }
}
