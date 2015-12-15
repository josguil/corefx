// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace System.Net
{
    internal class LoggingUtilities<EventType> where EventType : ILogging
    {
        internal static void Enter(EventType eventSource, object obj, string method, object paramObject)
        {
            string caller;
            string parameters = "";
            if (obj is string)
            {
                caller = obj as string;
            }
            else
            {
                caller = Logging.GetObjectLogHash(obj);
            }

            if (paramObject is string)
            {
                parameters = paramObject as string;
            }

            else if (paramObject != null)
            {
                parameters = Logging.GetObjectLogHash(paramObject);
            }
            eventSource.FunctionStart(Environment.CurrentManagedThreadId.ToString("d4", CultureInfo.InvariantCulture),
                                        caller,
                                        method,
                                        parameters);
        }
    }

    internal interface ILogging
    {
        void FunctionStart(string managedThreadID, string caller, string functionName, string parameters);
    }
}
