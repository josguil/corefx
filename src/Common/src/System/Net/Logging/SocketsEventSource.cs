// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.Tracing;
using System.Globalization;

namespace System.Net
{
    [EventSource(Name = "Microsoft-System-Net-Sockets")]
    internal sealed class SocketsEventSource : SystemNetEventSource
    {
        private static SocketsEventSource s_log = new SocketsEventSource();
        private SocketsEventSource() { }
        public static SocketsEventSource Log
        {
            get
            {
                return s_log;
            }
        }

        [Event((int)IdManager.DebugMessage, Keywords = Keywords.Debug,
            Level = EventLevel.Informational, Message = "[0] {2}-{1}")]
        new internal void DebugMessage(string managedThreadID, string message, string obj = "")
        {
            base.DebugMessage(managedThreadID, message, obj);
        }

        [Event((int)IdManager.FunctionStart, Keywords = Keywords.FunctionEntryExit, 
            Level = EventLevel.Verbose, Message = "[{0}] {1}::{2}({3})")]
        new internal void FunctionStart(string managedThreadID, string caller, string functionName, string parameters)
        {
            base.FunctionStart(managedThreadID, caller, functionName, parameters);
        }

        [Event((int)IdManager.FunctionStop, Keywords = Keywords.FunctionEntryExit,
            Level = EventLevel.Verbose, Message = "[{0}] {1}::{2}({3})")]
        new internal void FunctionStop(string managedThreadID, string caller, string functionName, string returnValue)
        {
            base.FunctionStop(managedThreadID, caller, functionName, returnValue);
        }

        [Event((int)IdManager.CriticalMessage, Keywords = Keywords.Default,
            Level = EventLevel.Critical, Message = "[{0}] {1}")]
        new internal void CriticalMessage(string managedThreadID, string message)
        {
            base.CriticalMessage(managedThreadID, message);
        }
        [Event((int)IdManager.WarningMessage, Keywords = Keywords.Default,
            Level = EventLevel.Warning, Message = "[{0}] {1}::{2}({3})")]
        new internal void WarningMessage(string managedThreadID, string obj, string method, string message)
        {
            base.WarningMessage(managedThreadID, obj, method, message);
        }

        [Event((int)IdManager.DebugDumpArray, Keywords = Keywords.Debug,
            Level = EventLevel.Verbose, Message = "[{0}] {1}")]
        new internal void DebugDumpArray(string managedThreadID, string array)
        {
            base.DebugDumpArray(managedThreadID, array);
        }

        public class Keywords
        {
            public const EventKeywords Default = (EventKeywords)0x0001;
            public const EventKeywords Debug = (EventKeywords)0x0002;
            public const EventKeywords FunctionEntryExit = (EventKeywords)0x0004;
        }
    }
}