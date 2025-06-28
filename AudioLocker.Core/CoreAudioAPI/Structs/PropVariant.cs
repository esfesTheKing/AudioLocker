/*
NOTE:
    The following LICENSE was taken from https://github.com/File-New-Project/EarTrumpet 
    and is ONLY applicable to this file.

    Original file in repository: EarTrumpet/Interop/PropVariant.cs
LICENSE
    The following legal persons or entities (collectively, the "Excluded Entities") are
    expressly excluded from the licensing terms set forth below and, as such, do not
    have the right to reproduce, distribute, or create derivative works from the
    software or associated documentation files:

        Yellow Elephant Productions
        Tidal Media Inc.

    The Excluded Entities may not exercise any of the rights granted to other users
    under these licensing terms.

    ---

    The MIT License (MIT)

    Copyright (c) 2015

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/
using System.Runtime.InteropServices;

namespace AudioLocker.Core.CoreAudioAPI.Structs;

[StructLayout(LayoutKind.Sequential, Pack = 0)]
public struct PropArray
{
    internal uint cElems;
    internal nint pElems;
}

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct PropVariant
{
    [FieldOffset(0)] public VarEnum varType;
    [FieldOffset(2)] public ushort wReserved1;
    [FieldOffset(4)] public ushort wReserved2;
    [FieldOffset(6)] public ushort wReserved3;
    [FieldOffset(8)] public byte bVal;
    [FieldOffset(8)] public sbyte cVal;
    [FieldOffset(8)] public ushort uiVal;
    [FieldOffset(8)] public short iVal;
    [FieldOffset(8)] public uint uintVal;
    [FieldOffset(8)] public int intVal;
    [FieldOffset(8)] public ulong ulVal;
    [FieldOffset(8)] public long lVal;
    [FieldOffset(8)] public float fltVal;
    [FieldOffset(8)] public double dblVal;
    [FieldOffset(8)] public short boolVal;
    [FieldOffset(8)] public nint pclsidVal;
    [FieldOffset(8)] public nint pszVal;
    [FieldOffset(8)] public nint pwszVal;
    [FieldOffset(8)] public nint punkVal;
    [FieldOffset(8)] public PropArray ca;
    [FieldOffset(8)] public System.Runtime.InteropServices.ComTypes.FILETIME filetime;
}
