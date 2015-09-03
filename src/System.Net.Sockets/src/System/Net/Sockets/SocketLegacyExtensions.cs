// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//------------------------------------------------------------------------------
// </copyright>
//------------------------------------------------------------------------------

namespace System.Net.Sockets
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    using System.Net.Sockets;

    /// <devdoc>
    /// <para>The <see cref='Sockets.Socket'/> class implements the Berkeley sockets
    ///    interface.</para>
    /// </devdoc>
    public static class SocketLegacyExtensions
    {
        public static System.IAsyncResult BeginAccept(this System.Net.Sockets.Socket socket, System.AsyncCallback callback, object state)
        {
            return socket.InternalBeginAccept(callback, state);
        }
        public static System.IAsyncResult BeginAccept(this System.Net.Sockets.Socket socket, int receiveSize, System.AsyncCallback callback, object state)
        {
            return socket.InternalBeginAccept(receiveSize, callback, state);
        }

        public static System.Net.Sockets.Socket EndAccept(this System.Net.Sockets.Socket socket, out byte[] buffer, System.IAsyncResult asyncResult)
        {
            return socket.InternalEndAccept(out buffer, asyncResult);
        }
        public static System.Net.Sockets.Socket EndAccept(this System.Net.Sockets.Socket socket, out byte[] buffer, out int bytesTransferred, System.IAsyncResult asyncResult)
        {
            return socket.InternalEndAccept(out buffer, out bytesTransferred, asyncResult);
        }
    }        
}

