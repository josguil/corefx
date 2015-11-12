// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.Tracing;

namespace System.Net
{
    internal class SystemNetEventSource : EventSource
    {
        public SystemNetEventSource(string nameString) : base (nameString) { }

        [Event(1, Keywords = Keywords.Debug, Level = EventLevel.Informational)]
        public void Print(int id, string message)
        {
            WriteEvent(1, id, message);
        }

        public class Keywords
        {
            public const EventKeywords Default = (EventKeywords)0x0001;
            public const EventKeywords Debug = (EventKeywords)0x0002;
            public const EventKeywords FunctionEntryExit = (EventKeywords)0x0004;
        }
    }
}
