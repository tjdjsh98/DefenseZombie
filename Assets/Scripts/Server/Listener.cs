using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using ServerCore;

public class Listener 
{
    Socket _listenSocket;
    Func<Session> _sessionFactory;

    public void Init(IPEndPoint endPoint, Func<Session> sessionFactory, int register = 10, int backlog = 100)
    {
        _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _sessionFactory += sessionFactory;

        // 문지기 교육
        _listenSocket.Bind(endPoint);

        // 영업시작
        // backlog : 최대 대기수
        _listenSocket.Listen(backlog);

        for (int i = 0; i < register; i++)
        {
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            RegisterAccpet(args);
        }
    }
    void RegisterAccpet(SocketAsyncEventArgs args)
    {
        args.AcceptSocket = null;

        bool pending = _listenSocket.AcceptAsync(args);
        if (pending == false)
            OnAcceptCompleted(null, args);
    }
    void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
    {
        Debug.Log("Accep");
        if (args.SocketError == SocketError.Success)
        {
            Session session = _sessionFactory.Invoke();
            session.Start(args.AcceptSocket);
            session.OnConnected(args.AcceptSocket.RemoteEndPoint);

        }
        else
            Console.WriteLine(args.SocketError.ToString());

        RegisterAccpet(args);
    }
}
