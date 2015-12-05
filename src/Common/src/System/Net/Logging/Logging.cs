// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace System.Net
{
    internal static class Logging
    {
        private static volatile bool s_loggingInitialized;

        private const int DefaultMaxDumpSize = 1024;

        private static SocketsEventSource s_socketsEventSource;
        private static WebEventSource s_webEventSource;

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

        internal static SocketsEventSource Sockets
        {
            get
            {
                if (!s_loggingInitialized)
                {
                    InitializeLogging();
                }

                if (!LoggingEnabled())
                {
                    return null;
                }

                return s_socketsEventSource;
            }
        }

        internal static WebEventSource Web
        {
            get
            {
                if (!s_loggingInitialized)
                {
                    InitializeLogging();
                }

                if (!LoggingEnabled())
                {
                    return null;
                }

                return s_webEventSource;
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
                    s_webEventSource = WebEventSource.Log;
                    GlobalLog.Print("Initalizating tracing");

                    s_loggingInitialized = true;
                }
            }
        }
        private static bool LoggingEnabled()
        {
            bool loggingEnabled = false;
            try
            {
                loggingEnabled = (s_socketsEventSource.IsEnabled() ||
                                  s_webEventSource.IsEnabled());
            }
            catch
            {
                loggingEnabled = false;
            }
            return loggingEnabled;
        }

        // Confirms logging is enabled, given current logging settings
        private static bool ValidateSettings(SystemNetEventSource eventSource)
        {
            if (!s_loggingInitialized)
            {
                InitializeLogging();
            }

            if (!LoggingEnabled())
            {
                return false;
            }

            if (eventSource == null)
            {
                return false;
            }

            return true;
        }

        // Converts an object to a normalized string that can be printed
        // takes System.Net.ObjectNamedFoo and coverts to ObjectNamedFoo, 
        // except IPAddress, IPEndPoint, and Uri, which return ToString().
        private static string GetObjectName(object obj)
        {
            if (obj is Uri || obj is System.Net.IPAddress || obj is System.Net.IPEndPoint)
            {
                return obj.ToString();
            }
            else
            {
                return obj.GetType().Name;
            }
        }

        private static string GetManagedThread()
        {
            return Environment.CurrentManagedThreadId.ToString("d4", CultureInfo.InvariantCulture);
        }

        internal static void Enter(SystemNetEventSource eventSource, object obj, string method, object paramObject)
        {
            if (!ValidateSettings(eventSource))
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
                caller = GetObjectLogHash(obj);
            }

            if (paramObject is string)
            {
                parameters = paramObject as string;
            }

            else if (paramObject != null)
            {
                parameters = GetObjectLogHash(paramObject);
            }

            eventSource.FunctionStart(GetManagedThread(), caller, method, parameters);
        }

        internal static void Exit(SystemNetEventSource eventSource, object obj, string method, object retObject)
        {
            if (!ValidateSettings(eventSource))
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
                caller = GetObjectLogHash(obj);
            }

            if (retObject is string)
            {
                parameters = retObject as string;
            }

            else if (retObject != null)
            {
                parameters = GetObjectLogHash(retObject);
            }

            eventSource.FunctionStop(GetManagedThread(), caller, method, parameters);
        }


        internal static void Exception(SystemNetEventSource eventSource, object obj, string method, Exception e)
        {
            if (!ValidateSettings(eventSource))
            {
                return;
            }
            string infoLine = SR.Format(SR.net_log_exception, GetObjectLogHash(obj), method, e.Message);
            if (!string.IsNullOrEmpty(e.StackTrace))
            {
                infoLine += Environment.NewLine + e.StackTrace;
            }
            eventSource.CriticalMessage(GetManagedThread(), infoLine);
        }

        internal static void PrintInfo(SystemNetEventSource eventSource, string msg)
        {
            if (!ValidateSettings(eventSource))
            {
                return;
            }
            eventSource.DebugMessage(GetManagedThread(), msg);
        }

        internal static void PrintInfo(SystemNetEventSource eventSource, object obj, string msg)
        {
            if (!ValidateSettings(eventSource))
            {
                return;
            }
            eventSource.DebugMessage(GetManagedThread(), GetObjectLogHash(obj), msg);
        }

        internal static void PrintWarning(SystemNetEventSource eventSource, object obj, string method, string msg)
        {
            if (!ValidateSettings(eventSource))
            {
                return;
            }
            eventSource.WarningMessage(GetManagedThread(), GetObjectLogHash(obj), method, msg);
        }

        internal static void PrintError(SystemNetEventSource eventSource, string msg)
        {
            if (!ValidateSettings(eventSource))
            {
                return;
            }
            eventSource.CriticalMessage(GetManagedThread(), msg);
        }

        internal static void PrintError(SystemNetEventSource eventSource, object obj, string method, string msg)
        {
            if (!ValidateSettings(eventSource))
            {
                return;
            }
            eventSource.CriticalMessage(GetManagedThread(), GetObjectLogHash(obj) + "::" + method + "() - " + msg);
        }

        internal static string GetObjectLogHash(object obj)
        {
            if (obj == null)
            {
                return "null";
            }
            return GetObjectName(obj) + "#" + Logging.HashString(obj);
        }

        internal static void Dump(SystemNetEventSource eventSource, object obj, string method, IntPtr bufferPtr, int length)
        {
            if (!ValidateSettings(eventSource) || bufferPtr == IntPtr.Zero || length < 0)
            {
                return;
            }

            byte[] buffer = new byte[length];
            Marshal.Copy(bufferPtr, buffer, 0, length);
            DumpArray(eventSource, obj, method, buffer, 0, length);
        }

        internal static void Dump(SystemNetEventSource traceSource, object obj, string method, byte[] buffer, int offset, int length)
        {
            if (!ValidateSettings(traceSource))
            {
                return;
            }
            DumpArray(traceSource, obj, method, buffer, offset, length);
        }
        private static void DumpArray(SystemNetEventSource eventSource, object obj, string method, byte[] buffer, int offset, int length)
        {
            if (buffer == null)
            {
                eventSource.DebugDumpArray(GetManagedThread(), "(null)");
                return;
            }

            if (offset > buffer.Length)
            {
                eventSource.DebugDumpArray(GetManagedThread(), "(offset out of range)");
                return;
            }
            eventSource.DebugDumpArray(GetManagedThread(), "Data from " + GetObjectLogHash(obj) + "::" + method);
            int maxDumpSize = DefaultMaxDumpSize;
            if (length > maxDumpSize)
            {
                eventSource.DebugDumpArray(GetManagedThread(), "(printing " + maxDumpSize.ToString(NumberFormatInfo.InvariantInfo) + " out of " + length.ToString(NumberFormatInfo.InvariantInfo) + ")");
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
                eventSource.DebugDumpArray(GetManagedThread(), disp);
                offset += n;
                length -= n;
            } while (length > 0);
        }
        internal static string ObjectToString(object objectValue)
        {
            if (objectValue == null)
            {
                return "(null)";
            }
            else if (objectValue is string && ((string)objectValue).Length == 0)
            {
                return "(string.empty)";
            }
            else if (objectValue is Exception)
            {
                return ExceptionMessage(objectValue as Exception);
            }
            else if (objectValue is IntPtr)
            {
                return "0x" + ((IntPtr)objectValue).ToString("x");
            }
            else
            {
                return objectValue.ToString();
            }
        }
        internal static string HashString(object objectValue)
        {
            if (objectValue == null)
            {
                return "(null)";
            }
            else if (objectValue is string && ((string)objectValue).Length == 0)
            {
                return "(string.empty)";
            }
            else
            {
                return objectValue.GetHashCode().ToString(NumberFormatInfo.InvariantInfo);
            }
        }

        private static string ExceptionMessage(Exception exception)
        {
            if (exception == null)
            {
                return string.Empty;
            }

            if (exception.InnerException == null)
            {
                return exception.Message;
            }

            return exception.Message + " (" + ExceptionMessage(exception.InnerException) + ")";
        }
    }
}
