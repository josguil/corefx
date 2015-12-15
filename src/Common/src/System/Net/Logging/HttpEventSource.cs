// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.Tracing;
using System.Globalization;

namespace System.Net
{
    [EventSource(Name = "Microsoft-System-Net-Http",
        Guid = "bdd9a83e-1929-5482-0d73-2fe5e1c0e16d")]
    internal sealed class HttpEventSource : EventSource, ILogging
    {
        private static HttpEventSource s_log = new HttpEventSource();
        private HttpEventSource() { }
        public static HttpEventSource Log
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

        [Event((int)EventIdManager.Associate, Keywords = Keywords.Default,
    Level = EventLevel.Informational, Message = "[{0}] Associating {1}, with {2}")]
        internal void Associate(string managedThreadID, string lineA, string lineB)
        {
            WriteEvent((int)EventIdManager.Associate, managedThreadID, lineA, lineB);
        }

        [Event((int)EventIdManager.FunctionStart, Keywords = Keywords.FunctionEntryExit,
    Level = EventLevel.Verbose, Message = "[{0}] {1}::{2}({3})")]
        public unsafe void FunctionStart(string managedThreadID, string caller, string functionName, string parameters)
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

        [Event((int)EventIdManager.WarningMessage, Keywords = Keywords.Default,
            Level = EventLevel.Warning, Message = "[{0}] {1}")]
        internal void WarningMessage(string managedThreadID, string message)
        {
            WriteEvent((int)EventIdManager.WarningMessage, managedThreadID, message);
        }

        [Event((int)EventIdManager.CriticalMessage, Keywords = Keywords.Default,
    Level = EventLevel.Critical, Message = "[{0}] {1}")]
        internal void CriticalMessage(string managedThreadID, string message)
        {
            WriteEvent((int)EventIdManager.CriticalMessage, managedThreadID, message);
        }

        [Event((int)EventIdManager.UriBaseAddress, Keywords = Keywords.Debug,
Level = EventLevel.Informational, Message = "[{0}] {2}-BaseAddress: '{1}'")]
        internal void UriBaseAddress(string managedThreadID, string message, string obj)
        {
            WriteEvent((int)EventIdManager.UriBaseAddress, managedThreadID, message, obj);
        }

        public class Keywords
        {
            public const EventKeywords Default = (EventKeywords)0x0001;
            public const EventKeywords Debug = (EventKeywords)0x0002;
            public const EventKeywords FunctionEntryExit = (EventKeywords)0x0004;
        }
    }
}
