// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Net.Sockets
{
    public static class SocketLegacyExtensions
    {
        public static IAsyncResult BeginAccept(this Socket thisSocket, AsyncCallback callback, object state)
        {
            return thisSocket.InternalBeginAccept(callback, state);
        }
        public static IAsyncResult BeginAccept(this Socket thisSocket, int receiveSize, AsyncCallback callback, object state)
        {
            return thisSocket.InternalBeginAccept(receiveSize, callback, state);
        }

        public static Socket EndAccept(this Socket thisSocket, out byte[] buffer, IAsyncResult asyncResult)
        {
            return thisSocket.InternalEndAccept(out buffer, asyncResult);
        }
        public static Socket EndAccept(this Socket thisSocket, out byte[] buffer, out int bytesTransferred, IAsyncResult asyncResult)
        {
            return thisSocket.InternalEndAccept(out buffer, out bytesTransferred, asyncResult);
        }
    }        
}

