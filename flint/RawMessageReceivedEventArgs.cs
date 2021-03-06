﻿using System;

namespace flint
{
    /// <summary> Args for the event of a message being received. 
    /// Possibly a little excessive as the relevant event will most likely have 
    /// exactly one subscriber, but it's a small effort.
    /// </summary>
    internal class RawMessageReceivedEventArgs : EventArgs
    {
        public ushort Endpoint { get; private set; }
        public byte[] Payload { get; private set; }

        public RawMessageReceivedEventArgs( ushort endpoint, byte[] payload )
        {
            Endpoint = endpoint;
            Payload = payload;
        }
    }
}