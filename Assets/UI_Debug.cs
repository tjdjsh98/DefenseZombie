using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class UI_Debug : UIBase
{
    [SerializeField]TextMeshProUGUI _text;
    [SerializeField]TextMeshProUGUI _pingText;

    bool _ping;
    bool _isSend;

    Queue<long> _pingQueue = new Queue<long>();

    float _time = 0;

    Stopwatch stopwatch = new Stopwatch(); 
    

    public override void Init()
    {
        
    }

    public void AddText(string text)
    {
        _text.text += text + "\n";
    }


    public void Update()
    {
        _time+= Time.deltaTime;
        if (!Client.Instance.IsSingle && !_isSend && _time > 0.2f)
        {
            _time= 0;
            stopwatch.Start();
            C_PingPacket packet = new C_PingPacket();
            DateTime date_time = DateTime.Now;
            int ms = date_time.Millisecond;
            packet.time = ms;
            _isSend = false;

            Client.Instance.Send(packet.Write());
        }
    }

    public void ReceivcePing(int ms)
    {
        DateTime date_time = DateTime.Now;

        stopwatch.Stop();
        _pingQueue.Enqueue(stopwatch.ElapsedMilliseconds);

        if (_pingQueue.Count > 5)
        {
            _pingQueue.Dequeue();
        }
        float mean = 0;
        foreach (var time in _pingQueue)
        {
            mean += time;
        }

        _pingText.text = $"Ping : {((int)(mean/_pingQueue.Count)).ToString()}";

        stopwatch.Reset();
    }

}
