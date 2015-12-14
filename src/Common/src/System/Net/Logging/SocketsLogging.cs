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
    internal static class SocketsLogging
    {
        private static volatile bool s_loggingInitialized;

        private const int DefaultMaxDumpSize = 1024;

        private static SocketsEventSource s_socketsEventSource;

        private static object s_internalSyncObject;
        private static object InternalSyncObject
        {
            get
            {
                if (s_internalSyncObject == null)
                {
                    object o = new Object();
                    Interlocked.CompareExchange(ref s_internalSyncObject, o, null);
                }

                return s_internalSyncObject;
            }
        }



        internal static bool On
        {
            get
            {
                if (!s_loggingInitialized)
                {
                    InitializeLogging();
                }
                return LoggingEnabled();
            }
        }

        // Sets up internal config settings for logging.
        private static void InitializeLogging()
        {
            lock (InternalSyncObject)
            {
                if (!s_loggingInitialized)
                {
                    s_socketsEventSource = SocketsEventSource.Log;
                    s_loggingInitialized = true;
                }
            }
        }
        private static bool LoggingEnabled()
        {
            bool loggingEnabled = false;
            try
            {
                loggingEnabled = s_socketsEventSource.IsEnabled();
            }
            catch
            {
                loggingEnabled = false;
            }
            return loggingEnabled;
        }

        // Confirms logging is enabled, given current logging settings
        private static bool ValidateSettings()
        {
            if (!s_loggingInitialized)
            {
                InitializeLogging();
            }

            if (!LoggingEnabled())
            {
                return false;
            }

            if (s_socketsEventSource == null)
            {
                return false;
            }

            return true;
        }

        internal static string GetManagedThread()
        {
            return Environment.CurrentManagedThreadId.ToString("d4", CultureInfo.InvariantCulture);
        }

        internal static void Enter(object obj, string method, object paramObject)
        {
            if (!ValidateSettings())
            {
                return;
            }
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
            s_socketsEventSource.FunctionStart(GetManagedThread(), caller, method, parameters);
        }

        internal static void Exit(object obj, string method, object retObject)
        {
            if (!ValidateSettings())
            {
                return;
            }
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

            if (retObject is string)
            {
                parameters = retObject as string;
            }

            else if (retObject != null)
            {
                parameters = Logging.GetObjectLogHash(retObject);
            }
            s_socketsEventSource.FunctionStop(GetManagedThread(), caller, method, parameters);
        }


        internal static void Exception(object obj, string method, Exception e)
        {
            if (!ValidateSettings())
            {
                return;
            }
            //TODO
            // Uncomment this line when porting code to Git.
            // string infoLine = SR.Format(SR.net_log_exception, GetObjectLogHash(obj), method, e.Message);
            string infoLine = e.Message; //TODO remove this line when ported to Git
            if (!string.IsNullOrEmpty(e.StackTrace))
            {
                infoLine += Environment.NewLine + e.StackTrace;
            }
            s_socketsEventSource.CriticalMessage(GetManagedThread(), infoLine);
        }

        internal static void PrintInfo( string msg)
        {
            if (!ValidateSettings())
            {
                return;
            }
            s_socketsEventSource.DebugMessage(GetManagedThread(), msg);
        }

        internal static void PrintInfo(object obj, string msg)
        {
            if (!ValidateSettings())
            {
                return;
            }
            s_socketsEventSource.DebugMessage(GetManagedThread(), Logging.GetObjectLogHash(obj), msg);
        }

        internal static void SocketAccepted(object socket, object localEp, object remoteEp)
        {
            if (!ValidateSettings())
            {
                return;
            }
            s_socketsEventSource.SocketAccepted(Logging.GetObjectName(localEp), Logging.GetObjectName(remoteEp), GetManagedThread(), Logging.GetObjectLogHash(socket));
        }

        internal static void PrintError(string msg)
        {
            if (!ValidateSettings())
            {
                return;
            }
            s_socketsEventSource.CriticalMessage(GetManagedThread(), msg);
        }

        internal static void PrintError(object obj, string method, string msg)
        {
            if (!ValidateSettings())
            {
                return;
            }
            s_socketsEventSource.CriticalMessage(GetManagedThread(), Logging.GetObjectLogHash(obj) + "::" + method + "() - " + msg);
        }

        internal static void Dump(object obj, string method, IntPtr bufferPtr, int length)
        {
            if (!ValidateSettings() || bufferPtr == IntPtr.Zero || length < 0)
            {
                return;
            }

            byte[] buffer = new byte[length];
            Marshal.Copy(bufferPtr, buffer, 0, length);

            DumpArray(obj, method, buffer, 0, length);
        }

        internal static void Dump(object obj, string method, byte[] buffer, int offset, int length)
        {
            if (!ValidateSettings())
            {
                return;
            }
            DumpArray(obj, method, buffer, offset, length);
        }
        private static void DumpArray(object obj, string method, byte[] buffer, int offset, int length)
        {
            if (buffer == null)
            {
                s_socketsEventSource.DebugDumpArray(GetManagedThread(), "(null)");
                return;
            }

            if (offset > buffer.Length)
            {
                s_socketsEventSource.DebugDumpArray(GetManagedThread(), "(offset out of range)");
                return;
            }
            s_socketsEventSource.DebugDumpArray(GetManagedThread(), "Data from " + Logging.GetObjectLogHash(obj) + "::" + method);
            int maxDumpSize = DefaultMaxDumpSize;
            if (length > maxDumpSize)
            {
                s_socketsEventSource.DebugDumpArray(GetManagedThread(), "(printing " + maxDumpSize.ToString(NumberFormatInfo.InvariantInfo) + " out of " + length.ToString(NumberFormatInfo.InvariantInfo) + ")");
                length = maxDumpSize;
            }

            if ((length < 0) || (length > buffer.Length - offset))
            {
                length = buffer.Length - offset;
            }

            do
            {
                int n = Math.Min(length, 16);
                string disp = string.Format(CultureInfo.CurrentCulture, "{0:X8} : ", offset);
                for (int i = 0; i < n; ++i)
                {
                    disp += string.Format(CultureInfo.CurrentCulture, "{0:X2}", buffer[offset + i]) + ((i == 7) ? '-' : ' ');
                }

                for (int i = n; i < 16; ++i)
                {
                    disp += "   ";
                }

                disp += ": ";
                for (int i = 0; i < n; ++i)
                {
                    disp += ((buffer[offset + i] < 0x20) || (buffer[offset + i] > 0x7e))
                                ? '.'
                                : (char)(buffer[offset + i]);
                }
                s_socketsEventSource.DebugDumpArray(GetManagedThread(), disp);
                offset += n;
                length -= n;
            } while (length > 0);
        }
    }
}
