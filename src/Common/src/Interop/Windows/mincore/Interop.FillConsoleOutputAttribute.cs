// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class mincore
    {
        [DllImport(Libraries.Console_L2, SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool FillConsoleOutputAttribute(IntPtr hConsoleOutput, short wColorAttribute, int numCells, COORD startCoord, out int pNumBytesWritten);
    }
}
