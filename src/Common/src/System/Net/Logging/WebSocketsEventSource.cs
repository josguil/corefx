// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.Tracing;
using System.Globalization;

namespace System.Net
{
    [EventSource(Name = "Microsoft-System-Net-WebSockets",
        Guid = "7e230bd6-383d-5c90-5db3-060796636618")]
    internal sealed class WebSocketsEventSource : EventSource
    {
        private static WebSocketsEventSource s_log = new WebSocketsEventSource();
        private WebSocketsEventSource() { }
        public static WebSocketsEventSource Log
        {
            get
            {
                return s_log;
            }
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

        public class Keywords
        {
            public const EventKeywords Default = (EventKeywords)0x0001;
            public const EventKeywords Debug = (EventKeywords)0x0002;
            public const EventKeywords FunctionEntryExit = (EventKeywords)0x0004;
        }
    }
}