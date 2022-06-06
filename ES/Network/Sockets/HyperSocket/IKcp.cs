﻿using System;

namespace ES.Network.Sockets.HyperSocket
{
    /// <summary>
    /// kcp监听器
    /// </summary>
    internal interface IKcp
    {
        void OnReceive(byte[] data);
        void OnSend(Span<byte> data);
    }
}