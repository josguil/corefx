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
    internal static class HttpLogging
    {
        private static volatile bool s_loggingInitialized;

        private static HttpEventSource s_httpEventSource;

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
                    s_httpEventSource = HttpEventSource.Log;
                    s_loggingInitialized = true;
                }
            }
        }
        private static bool LoggingEnabled()
        {
            bool loggingEnabled = false;
            try
            {
                loggingEnabled = s_httpEventSource.IsEnabled();
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

            if (s_httpEventSource == null)
            {
                return false;
            }

            return true;
        }
        internal static string GetManagedThread()
        {
            return Environment.CurrentManagedThreadId.ToString("d4", CultureInfo.InvariantCulture);
        }
        internal static void Associate(object objA, object objB)
        {
            if (!ValidateSettings())
            {
                return;
            }

            string lineA = Logging.GetObjectLogHash(objA);
            string lineB = Logging.GetObjectLogHash(objB);
            s_httpEventSource.Associate(GetManagedThread(), lineA, lineB);
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
            s_httpEventSource.FunctionStart(GetManagedThread(), caller, method, parameters);
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
            s_httpEventSource.FunctionStop(GetManagedThread(), caller, method, parameters);
        }


        internal static void Exception(object obj, string method, Exception e)
        {
            if (!ValidateSettings())
            {
                return;
            }
            string infoLine = SR.Format(SR.net_log_exception, Logging.GetObjectLogHash(obj), method, e.Message);
            if (!string.IsNullOrEmpty(e.StackTrace))
            {
                infoLine += Environment.NewLine + e.StackTrace;
            }
            s_httpEventSource.CriticalMessage(GetManagedThread(), infoLine);
        }

        internal static void PrintInfo(string msg)
        {
            if (!ValidateSettings())
            {
                return;
            }
            s_httpEventSource.DebugMessage(GetManagedThread(), msg);
        }

        internal static void PrintWarning(string msg)
        {
            if (!ValidateSettings())
            {
                return;
            }
            s_httpEventSource.WarningMessage(GetManagedThread(), msg);
        }

        internal static void PrintInfo(object obj, string msg)
        {
            if (!ValidateSettings())
            {
                return;
            }
            s_httpEventSource.DebugMessage(GetManagedThread(), Logging.GetObjectLogHash(obj), msg);
        }

        internal static void PrintError(string msg)
        {
            if (!ValidateSettings())
            {
                return;
            }
            s_httpEventSource.CriticalMessage(GetManagedThread(), msg);
        }

        internal static void PrintError(object obj, string method, string msg)
        {
            if (!ValidateSettings())
            {
                return;
            }
            s_httpEventSource.CriticalMessage(GetManagedThread(), Logging.GetObjectLogHash(obj) + "::" + method + "() - " + msg);
        }
    }
}
