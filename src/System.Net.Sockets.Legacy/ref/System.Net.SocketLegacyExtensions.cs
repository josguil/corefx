// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.Net.Sockets
{
   
    public static class SocketLegacyExtensions
    {
        public static System.IAsyncResult BeginAccept(this System.Net.Sockets.Socket socket, System.AsyncCallback callback, object state) { return default(System.IAsyncResult); }
        public static System.IAsyncResult BeginAccept(this System.Net.Sockets.Socket socket, int receiveSize, System.AsyncCallback callback, object state) { return default(System.IAsyncResult); }
        
        public static System.Net.Sockets.Socket EndAccept(this System.Net.Sockets.Socket socket, out byte[] buffer, System.IAsyncResult asyncResult) { buffer = default(byte[]); return default(System.Net.Sockets.Socket); }
        public static System.Net.Sockets.Socket EndAccept(this System.Net.Sockets.Socket socket, out byte[] buffer, out int bytesTransferred, System.IAsyncResult asyncResult) { buffer = default(byte[]); bytesTransferred = default(int); return default(System.Net.Sockets.Socket); }
    }
}
