﻿namespace flint
{
    /// <summary>
    /// Event args for when the contents of the Pebble's app bank have been received.
    /// </summary>
    public class AppbankContentsReceivedEventArgs : MessageReceivedEventArgs
    {
        public AppBank AppBank { get; private set; }
        public AppbankContentsReceivedEventArgs(Pebble.Endpoints endPoint, byte[] payload)
            : base(endPoint, payload)
        {
            AppBank = new AppBank(Payload);
        }

        public AppbankContentsReceivedEventArgs(byte[] payload)
            : this(Pebble.Endpoints.AppManager, payload)
        {
        }
    }
}