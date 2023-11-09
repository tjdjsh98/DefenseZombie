﻿using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;
public class Connector
{
    Func<Session> _sessionFactory;
    Action _successConnectHandler;

    public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory,Action successConnectHandler,Client client)
    {
        Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        if (socket == null)
            UnityEngine.Object.Destroy(client.gameObject);

        _sessionFactory = sessionFactory;
        _successConnectHandler += successConnectHandler;

        SocketAsyncEventArgs args = new SocketAsyncEventArgs();
        args.Completed += OnConnectCompleted;
        args.RemoteEndPoint = endPoint;
        args.UserToken = socket;

        RegisterConnect(args);
    }

    void RegisterConnect(SocketAsyncEventArgs args)
    {
        Socket socket = args.UserToken as Socket;
        if (socket == null)
            return;

        bool pending = socket.ConnectAsync(args);
        if (!pending)
            OnConnectCompleted(null, args);
    }

    void OnConnectCompleted(object sender,SocketAsyncEventArgs args)
    {
        if(args.SocketError == SocketError.Success)
        {
            Session session = _sessionFactory.Invoke();
            session.Start(args.ConnectSocket);
            session.OnConnected(args.RemoteEndPoint);

            _successConnectHandler?.Invoke();
        }
        else
        {
            Console.WriteLine($"OnConnectCompleted Fail : {args.SocketError}");
        }
    }
}
