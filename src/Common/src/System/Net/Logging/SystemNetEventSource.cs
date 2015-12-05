// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.Tracing;

namespace System.Net
{
    internal abstract class SystemNetEventSource : EventSource
    {

        internal void DebugMessage(string threadID, string message, string obj = "")
        {
            WriteEvent((int)IdManager.DebugMessage, threadID, message, obj);
        }
        internal unsafe void FunctionStart(string threadID, string caller, string functionName, string parameters)
        {
            fixed (char* arg1Ptr = threadID, arg2Ptr = caller, arg3Ptr = functionName, arg4Ptr = parameters)
            {

                EventData* dataDesc = stackalloc EventSource.EventData[4];

                dataDesc[0].DataPointer = (IntPtr)arg1Ptr;
                dataDesc[0].Size = (threadID.Length + 1) * 2; // Size in bytes, including a null terminator. 
                dataDesc[1].DataPointer = (IntPtr)(arg2Ptr);
                dataDesc[1].Size = (caller.Length + 1) * 2;
                dataDesc[2].DataPointer = (IntPtr)(arg3Ptr);
                dataDesc[2].Size = (functionName.Length + 1) * 2;
                dataDesc[3].DataPointer = (IntPtr)(arg4Ptr);
                dataDesc[3].Size = (parameters.Length + 1) * 2;

                WriteEventCore((int)IdManager.FunctionStart, 4, dataDesc);
            }
        }
        internal unsafe void FunctionStop(string threadID, string caller, string functionName, string returnValue)
        {
            fixed (char* arg1Ptr = threadID, arg2Ptr = caller, arg3Ptr = functionName, arg4Ptr = returnValue)
            {

                EventData* dataDesc = stackalloc EventSource.EventData[4];

                dataDesc[0].DataPointer = (IntPtr)arg1Ptr;
                dataDesc[0].Size = (threadID.Length + 1) * 2; // Size in bytes, including a null terminator. 
                dataDesc[1].DataPointer = (IntPtr)(arg2Ptr);
                dataDesc[1].Size = (caller.Length + 1) * 2;
                dataDesc[2].DataPointer = (IntPtr)(arg3Ptr);
                dataDesc[2].Size = (functionName.Length + 1) * 2;
                dataDesc[3].DataPointer = (IntPtr)(arg4Ptr);
                dataDesc[3].Size = (returnValue.Length + 1) * 2;

                WriteEventCore((int)IdManager.FunctionStop, 4, dataDesc);
            }
        }
        internal void CriticalMessage(string threadId, string message)
        {
            WriteEvent((int)IdManager.CriticalMessage, threadId, message);
        }

        internal unsafe void WarningMessage(string threadID, string obj, string method, string message)
        {
            fixed (char* arg1Ptr = threadID, arg2Ptr = obj, arg3Ptr = method, arg4Ptr = message)
            {

                EventData* dataDesc = stackalloc EventSource.EventData[4];

                dataDesc[0].DataPointer = (IntPtr)arg1Ptr;
                dataDesc[0].Size = (threadID.Length + 1) * 2; // Size in bytes, including a null terminator. 
                dataDesc[1].DataPointer = (IntPtr)(arg2Ptr);
                dataDesc[1].Size = (obj.Length + 1) * 2;
                dataDesc[2].DataPointer = (IntPtr)(arg3Ptr);
                dataDesc[2].Size = (method.Length + 1) * 2;
                dataDesc[3].DataPointer = (IntPtr)(arg4Ptr);
                dataDesc[3].Size = (message.Length + 1) * 2;

                WriteEventCore((int)IdManager.WarningMessage, 4, dataDesc);
            }
        }
        internal void DebugDumpArray(string threadId, string array)
        {
            WriteEvent((int)IdManager.DebugDumpArray, threadId, array);
        }

        internal enum IdManager
        {
            DebugMessage = 1,
            DebugDumpArray,
            WarningDumpArray,
            FunctionStart,
            FunctionStop,
            WarningMessage,
            AssertFailed,
            CriticalMessage
        }
    }
}