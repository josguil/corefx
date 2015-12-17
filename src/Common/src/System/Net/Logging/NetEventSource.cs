// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.Tracing;
using System.Globalization;

namespace System.Net
{
    [EventSource(Name = "Microsoft-System-Net",
        Guid = "501a994a-eb63-5415-9af1-1b031260f16c")]
    internal sealed class NetEventSource : EventSource
    {
        private const int FUNCTIONSTART_ID = 1;

        private static NetEventSource s_log = new NetEventSource();
        private NetEventSource() { }
        public static NetEventSource Log
        {
            get
            {
                return s_log;
            }
        }

        [NonEvent]
        internal static void Enter(object obj, string method, object paramObject, Component componentType)
        {
            if (!s_log.IsEnabled())
            {
                return;
            }
            string callerName;
            int callerHash;
            string parametersName = "";
            int parametersHash = 0;
            if (obj is string)
            {
                callerName = obj as string;
                callerHash = 0;
            }
            else
            {
                callerName = LoggingHash.GetObjectName(obj);
                callerHash = LoggingHash.HashInt(obj);
            }

            if (paramObject is string)
            {
                parametersName = paramObject as string;
                parametersHash = 0;
            }

            else if (paramObject != null)
            {
                parametersName = LoggingHash.GetObjectName(paramObject);
                parametersHash = LoggingHash.HashInt(paramObject);
            }
            s_log.FunctionStart(callerName, callerHash, method, parametersName, parametersHash, componentType);
        }

        [Event(FUNCTIONSTART_ID, Keywords = Keywords.FunctionEntryExit,
    Level = EventLevel.Verbose, Message = "[{5}] {0}#{1}::{2}({3}#{4})")]
        internal unsafe void FunctionStart(string callerName,
                                            int callerHash,
                                            string method,
                                            string parametersName,
                                            int parametersHash,
                                            Component componentType)
        {
            
            fixed (char* arg1Ptr = callerName, arg2Ptr = method, arg3Ptr = parametersName)
            {
                EventData* dataDesc = stackalloc EventSource.EventData[6];

                dataDesc[0].DataPointer = (IntPtr)arg1Ptr;
                dataDesc[0].Size = (callerName.Length + 1) * sizeof(short); // Size in bytes, including a null terminator. 
                dataDesc[1].DataPointer = (IntPtr)(&callerHash);
                dataDesc[1].Size = sizeof(int);
                dataDesc[2].DataPointer = (IntPtr)(arg2Ptr);
                dataDesc[2].Size = (method.Length + 1) * sizeof(short);
                dataDesc[3].DataPointer = (IntPtr)(arg3Ptr);
                dataDesc[3].Size = (parametersName.Length + 1) * sizeof(short);
                dataDesc[4].DataPointer = (IntPtr)(&parametersHash);
                dataDesc[4].Size = sizeof(int);
                dataDesc[5].DataPointer = (IntPtr)(&componentType);
                dataDesc[5].Size = sizeof(int);

                WriteEventCore(FUNCTIONSTART_ID, 6, dataDesc);
            }
        }

        public class Keywords
        {
            public const EventKeywords Default = (EventKeywords)0x0001;
            public const EventKeywords Debug = (EventKeywords)0x0002;
            public const EventKeywords FunctionEntryExit = (EventKeywords)0x0004;
        }

        public enum Component
        {
            Socket,
            Http,
            WebSocket
        }
    }
}
