// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.Tracing;
using System.Globalization;

namespace System.Net
{

    [EventSource(Name = "Microsoft-System-Net-Sockets",
        Guid = "e03c0352-f9c9-56ff-0ea7-b94ba8cabc6b",
        LocalizationResources = "FxResources.System.Net.Sockets.SR")]
    internal sealed class SocketsEventSource : EventSource
    {
        //const string SRsocketAccepted = SR.GetResourceString("net_log_socket_accepted", @"Accepted connection from {0} to {1}.");//"hey";// SR.net_log_socket_accepted;
        //const string SRsocketAccepted = SR.net_log_socket_accepted;
        private static SocketsEventSource s_log = new SocketsEventSource();
        private SocketsEventSource() { }
        public static SocketsEventSource Log
        {
            get
            {
                return s_log;
            }
        }

        [Event((int)EventIdManager.DebugMessage, Keywords = Keywords.Debug,
            Level = EventLevel.Informational, Message = "[{0}] {2}-{1}")]
        internal void DebugMessage(string managedThreadID, string message, string obj = "")
        {
            WriteEvent((int)EventIdManager.DebugMessage, managedThreadID, message, obj);
        }

        [Event((int)EventIdManager.FunctionStart, Keywords = Keywords.FunctionEntryExit,
            Level = EventLevel.Verbose, Message = "[{0}] {1}::{2}({3})")]
        internal unsafe void FunctionStart(string managedThreadID, string caller, string functionName, string parameters)
        {
            fixed (char* arg1Ptr = managedThreadID, arg2Ptr = caller, arg3Ptr = functionName, arg4Ptr = parameters)
            {

                EventData* dataDesc = stackalloc EventSource.EventData[4];

                dataDesc[0].DataPointer = (IntPtr)arg1Ptr;
                dataDesc[0].Size = (managedThreadID.Length + 1) * 2; // Size in bytes, including a null terminator. 
                dataDesc[1].DataPointer = (IntPtr)(arg2Ptr);
                dataDesc[1].Size = (caller.Length + 1) * 2;
                dataDesc[2].DataPointer = (IntPtr)(arg3Ptr);
                dataDesc[2].Size = (functionName.Length + 1) * 2;
                dataDesc[3].DataPointer = (IntPtr)(arg4Ptr);
                dataDesc[3].Size = (parameters.Length + 1) * 2;

                WriteEventCore((int)EventIdManager.FunctionStart, 4, dataDesc);
            }
        }

        [Event((int)EventIdManager.FunctionStop, Keywords = Keywords.FunctionEntryExit,
            Level = EventLevel.Verbose, Message = "[{0}] {1}::{2}({3})")]
        internal unsafe void FunctionStop(string managedThreadID, string caller, string functionName, string returnValue)
        {
            fixed (char* arg1Ptr = managedThreadID, arg2Ptr = caller, arg3Ptr = functionName, arg4Ptr = returnValue)
            {

                EventData* dataDesc = stackalloc EventSource.EventData[4];

                dataDesc[0].DataPointer = (IntPtr)arg1Ptr;
                dataDesc[0].Size = (managedThreadID.Length + 1) * 2; // Size in bytes, including a null terminator. 
                dataDesc[1].DataPointer = (IntPtr)(arg2Ptr);
                dataDesc[1].Size = (caller.Length + 1) * 2;
                dataDesc[2].DataPointer = (IntPtr)(arg3Ptr);
                dataDesc[2].Size = (functionName.Length + 1) * 2;
                dataDesc[3].DataPointer = (IntPtr)(arg4Ptr);
                dataDesc[3].Size = (returnValue.Length + 1) * 2;

                WriteEventCore((int)EventIdManager.FunctionStop, 4, dataDesc);
            }
        }

        [Event((int)EventIdManager.CriticalMessage, Keywords = Keywords.Default,
            Level = EventLevel.Critical, Message = "[{0}] {1}")]
        internal void CriticalMessage(string managedThreadID, string message)
        {
            WriteEvent((int)EventIdManager.CriticalMessage, managedThreadID, message);
        }
        [Event((int)EventIdManager.WarningMessage, Keywords = Keywords.Default,
            Level = EventLevel.Warning, Message = "[{0}] {1}::{2}({3})")]
        internal unsafe void WarningMessage(string managedThreadID, string obj, string method, string message)
        {
            fixed (char* arg1Ptr = managedThreadID, arg2Ptr = obj, arg3Ptr = method, arg4Ptr = message)
            {

                EventData* dataDesc = stackalloc EventSource.EventData[4];

                dataDesc[0].DataPointer = (IntPtr)arg1Ptr;
                dataDesc[0].Size = (managedThreadID.Length + 1) * 2; // Size in bytes, including a null terminator. 
                dataDesc[1].DataPointer = (IntPtr)(arg2Ptr);
                dataDesc[1].Size = (obj.Length + 1) * 2;
                dataDesc[2].DataPointer = (IntPtr)(arg3Ptr);
                dataDesc[2].Size = (method.Length + 1) * 2;
                dataDesc[3].DataPointer = (IntPtr)(arg4Ptr);
                dataDesc[3].Size = (message.Length + 1) * 2;

                WriteEventCore((int)EventIdManager.WarningMessage, 4, dataDesc);
            }
        }

        [Event((int)EventIdManager.DebugDumpArray, Keywords = Keywords.Debug,
            Level = EventLevel.Verbose, Message = "[{0}] {1}")]
        internal void DebugDumpArray(string managedThreadID, string array)
        {
            WriteEvent((int)EventIdManager.DebugDumpArray, managedThreadID, array);
        }

        [Event((int)EventIdManager.SocketAccepted, Keywords = Keywords.Default,
            Level = EventLevel.Informational)]
        internal unsafe void SocketAccepted(string localEp, string remoteEp, string managedThreadID, string obj)
        {
            fixed (char* arg1Ptr = localEp, arg2Ptr = remoteEp, arg3Ptr = managedThreadID, arg4Ptr = obj)
            {

                EventData* dataDesc = stackalloc EventSource.EventData[4];

                dataDesc[0].DataPointer = (IntPtr)arg1Ptr;
                dataDesc[0].Size = (localEp.Length + 1) * 2; // Size in bytes, including a null terminator. 
                dataDesc[1].DataPointer = (IntPtr)(arg2Ptr);
                dataDesc[1].Size = (remoteEp.Length + 1) * 2;
                dataDesc[2].DataPointer = (IntPtr)(arg3Ptr);
                dataDesc[2].Size = (managedThreadID.Length + 1) * 2;
                dataDesc[3].DataPointer = (IntPtr)(arg4Ptr);
                dataDesc[3].Size = (obj.Length + 1) * 2;

                WriteEventCore((int)EventIdManager.SocketAccepted, 4, dataDesc);
            }
        }

        public class Keywords
        {
            public const EventKeywords Default = (EventKeywords)0x0001;
            public const EventKeywords Debug = (EventKeywords)0x0002;
            public const EventKeywords FunctionEntryExit = (EventKeywords)0x0004;
        }
    }
}
