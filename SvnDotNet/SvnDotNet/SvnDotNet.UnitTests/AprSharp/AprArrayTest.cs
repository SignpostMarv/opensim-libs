//  AprSharpTest, NUnit tests for AprSharp
#region Copyright (C) 2004 SOFTEC sa.
//
//  This library is free software; you can redistribute it and/or
//  modify it under the terms of the GNU Lesser General Public
//  License as published by the Free Software Foundation; either
//  version 2.1 of the License, or (at your option) any later version.
//
//  This library is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
//  Sources, support options and lastest version of the complete library
//  is available from:
//		http://www.softec.st/AprSharp
//		Support@softec.st
//
//  Initial authors : 
//		Denis Gervalle
//		Olivier Desaive
#endregion
//
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using NUnit.Framework;
using PumaCode.SvnDotNet.AprSharp;

namespace PumaCode.SvnDotNet.UnitTests.AprSharp {
    [TestFixture]
    public class AprArrayTest {
        [TestFixtureSetUp]
        public void Init()
        {
            //Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
        }

        [Test]
        public void Make()
        {
            AprPool p = AprPool.Create();
            Assert.IsFalse(p.IsNull, "#A01");

            AprArray a = new AprArray();
            Assert.IsTrue(a.IsNull, "#A02");

            a = AprArray.Make(p, 10, Marshal.SizeOf(typeof(int)));
            Assert.IsFalse(a.IsNull, "#A03");
            Assert.AreEqual(((IntPtr) p).ToInt32(), ((IntPtr) a.Pool).ToInt32(), "#A04");
            Assert.AreEqual(10, a.AllocatedCount, "#A05");
            Assert.AreEqual(Marshal.SizeOf(typeof(int)), a.ElementSize, "#A06");
            Assert.IsTrue(a.IsEmpty(), "#A07");
            Assert.AreEqual(0, a.Count, "#A08");

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#A09");
        }

        [Test]
        public void PushPop()
        {
            AprPool p = AprPool.Create();
            Assert.IsFalse(p.IsNull, "#B01");

            AprArray a = AprArray.Make(p, 5, Marshal.SizeOf(typeof(int)));
            Assert.IsFalse(a.IsNull, "#B02");

            IntPtr ptr = a.Push();
            Assert.IsTrue(a != IntPtr.Zero, "#B03");
            Marshal.WriteInt32(ptr, 1);
            Marshal.WriteInt32(a.Push(), 2);
            Marshal.WriteInt32(a.Push(), 3);
            Marshal.WriteInt32(a.Push(), 4);
            Marshal.WriteInt32(a.Push(), 5);

            ptr = a.Pop();
            Assert.IsTrue(a != IntPtr.Zero, "#B04");
            Assert.AreEqual(5, Marshal.ReadInt32(ptr), "#B05");
            Assert.AreEqual(4, Marshal.ReadInt32(a.Pop()), "#BO6");
            Assert.AreEqual(3, Marshal.ReadInt32(a.Pop()), "#BO7");
            Assert.AreEqual(2, Marshal.ReadInt32(a.Pop()), "#BO8");
            Assert.AreEqual(1, Marshal.ReadInt32(a.Pop()), "#BO9");

            Marshal.WriteInt32(a.Push(), 1);
            Marshal.WriteInt32(a.Push(), 2);
            Marshal.WriteInt32(a.Push(), 3);
            Marshal.WriteInt32(a.Push(), 4);
            Marshal.WriteInt32(a.Push(), 5);
            Marshal.WriteInt32(a.Push(), 6);
            Marshal.WriteInt32(a.Push(), 7);
            Marshal.WriteInt32(a.Push(), 8);
            Marshal.WriteInt32(a.Push(), 9);
            Marshal.WriteInt32(a.Push(), 10);

            Assert.AreEqual(10, Marshal.ReadInt32(a.Pop()), "#B10");
            Assert.AreEqual(9, Marshal.ReadInt32(a.Pop()), "#B11");
            Assert.AreEqual(8, Marshal.ReadInt32(a.Pop()), "#B12");
            Assert.AreEqual(7, Marshal.ReadInt32(a.Pop()), "#B13");
            Assert.AreEqual(6, Marshal.ReadInt32(a.Pop()), "#B14");
            Assert.AreEqual(5, Marshal.ReadInt32(a.Pop()), "#B15");
            Assert.AreEqual(4, Marshal.ReadInt32(a.Pop()), "#B16");
            Assert.AreEqual(3, Marshal.ReadInt32(a.Pop()), "#B17");
            Assert.AreEqual(2, Marshal.ReadInt32(a.Pop()), "#B18");
            Assert.AreEqual(1, Marshal.ReadInt32(a.Pop()), "#B19");

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#B20");
        }

        [Test]
        public void PushPopObject()
        {
            AprPool p = AprPool.Create();
            Assert.IsFalse(p.IsNull, "#B01");

            AprArray a = AprArray.Make(p, 5, typeof(int));
            Assert.IsFalse(a.IsNull, "#B02");

            a.Push(1);
            a.Push(2);
            a.Push(3);
            a.Push(4);
            a.Push(5);

            Assert.AreEqual(5, a.PopObject(), "#B05");
            Assert.AreEqual(4, a.PopObject(), "#BO6");
            Assert.AreEqual(3, a.PopObject(), "#BO7");
            Assert.AreEqual(2, a.PopObject(), "#BO8");
            Assert.AreEqual(1, a.PopObject(), "#BO9");

            a = AprArray.Make(p, 5, typeof(AprString));

            a.Push(new AprString("This", p));
            a.Push(new AprString("is", p));
            a.Push(new AprString("a", p));
            a.Push(new AprString("test.", p));

            Assert.AreEqual("test.", a.PopObject().ToString(), "#B10");
            Assert.AreEqual("a", a.PopObject().ToString(), "#B11");
            Assert.AreEqual("is", a.PopObject().ToString(), "#B12");
            Assert.AreEqual("This", a.PopObject().ToString(), "#B13");

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#B14");
        }

        [Test]
        public void Copy()
        {
            AprPool p = AprPool.Create();
            Assert.IsFalse(p.IsNull, "#C01");

            AprArray a = AprArray.Make(p, 5, Marshal.SizeOf(typeof(int)));
            Assert.IsFalse(a.IsNull, "#C02");

            Marshal.WriteInt32(a.Push(), 1);
            Marshal.WriteInt32(a.Push(), 2);
            Marshal.WriteInt32(a.Push(), 3);
            Marshal.WriteInt32(a.Push(), 4);
            Marshal.WriteInt32(a.Push(), 5);

            AprArray ca = a.Copy(p);
            Assert.IsTrue(((IntPtr) a).ToInt32() != ((IntPtr) ca).ToInt32(), "#C03");
            Assert.AreEqual(a.Count, ca.Count, "#C04");
            Assert.AreEqual(5, Marshal.ReadInt32(ca.Pop()), "#C05");
            Assert.AreEqual(4, Marshal.ReadInt32(ca.Pop()), "#CO6");
            Assert.AreEqual(3, Marshal.ReadInt32(ca.Pop()), "#CO7");
            Assert.AreEqual(2, Marshal.ReadInt32(ca.Pop()), "#CO8");
            Assert.AreEqual(1, Marshal.ReadInt32(ca.Pop()), "#CO9");

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#C10");
        }

        [Test]
        public void Append()
        {
            AprPool p = AprPool.Create();
            Assert.IsFalse(p.IsNull, "#D01");

            AprArray a = AprArray.Make(p, 2, Marshal.SizeOf(typeof(int)));
            Assert.IsFalse(a.IsNull, "#D02");

            Marshal.WriteInt32(a.Push(), 1);
            Marshal.WriteInt32(a.Push(), 2);

            AprArray a2 = AprArray.Make(p, 3, Marshal.SizeOf(typeof(int)));
            Assert.IsFalse(a2.IsNull, "#D03");

            Marshal.WriteInt32(a2.Push(), 3);
            Marshal.WriteInt32(a2.Push(), 4);
            Marshal.WriteInt32(a2.Push(), 5);

            AprArray ca = a.Append(p, a2);
            Assert.AreEqual(a.Count + a2.Count, ca.Count, "#D04");
            Assert.AreEqual(5, Marshal.ReadInt32(ca.Pop()), "#D05");
            Assert.AreEqual(4, Marshal.ReadInt32(ca.Pop()), "#DO6");
            Assert.AreEqual(3, Marshal.ReadInt32(ca.Pop()), "#DO7");
            Assert.AreEqual(2, Marshal.ReadInt32(ca.Pop()), "#DO8");
            Assert.AreEqual(1, Marshal.ReadInt32(ca.Pop()), "#DO9");

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#D10");
        }

        [Test]
        public void Cat()
        {
            AprPool p = AprPool.Create();
            Assert.IsFalse(p.IsNull, "#D01");

            AprArray a = AprArray.Make(p, 2, Marshal.SizeOf(typeof(int)));
            Assert.IsFalse(a.IsNull, "#D02");

            Marshal.WriteInt32(a.Push(), 1);
            Marshal.WriteInt32(a.Push(), 2);

            AprArray a2 = AprArray.Make(p, 3, Marshal.SizeOf(typeof(int)));
            Assert.IsFalse(a2.IsNull, "#D03");

            Marshal.WriteInt32(a2.Push(), 3);
            Marshal.WriteInt32(a2.Push(), 4);
            Marshal.WriteInt32(a2.Push(), 5);

            a.Cat(a2);
            Assert.AreEqual(5, Marshal.ReadInt32(a.Pop()), "#D05");
            Assert.AreEqual(4, Marshal.ReadInt32(a.Pop()), "#DO6");
            Assert.AreEqual(3, Marshal.ReadInt32(a.Pop()), "#DO7");
            Assert.AreEqual(2, Marshal.ReadInt32(a.Pop()), "#DO8");
            Assert.AreEqual(1, Marshal.ReadInt32(a.Pop()), "#DO9");

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#D10");
        }

        [Test]
        public void StrCat()
        {
            AprPool p = AprPool.Create();
            Assert.IsFalse(p.IsNull, "#E01");

            AprArray a = AprArray.Make(p, 4, Marshal.SizeOf(typeof(IntPtr)));
            Assert.IsFalse(a.IsNull, "#E02");

            Marshal.WriteIntPtr(a.Push(), new AprString("This", p));
            Marshal.WriteIntPtr(a.Push(), new AprString("is", p));
            Marshal.WriteIntPtr(a.Push(), new AprString("a", p));
            Marshal.WriteIntPtr(a.Push(), new AprString("test.", p));

            Assert.AreEqual("This is a test.", a.StrCat(p, ' '), "#EO3");

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#E04");
        }

        [Test]
        public void ArrayOfAprString()
        {
            AprPool p = AprPool.Create();
            Assert.IsFalse(p.IsNull, "#F01");

            AprArray a = AprArray.Make(p, 5, typeof(AprString));
            Assert.IsFalse(a.IsNull, "#F02");

            Marshal.WriteIntPtr(a.Push(), new AprString("1", p));
            Marshal.WriteIntPtr(a.Push(), new AprString("2", p));
            Marshal.WriteIntPtr(a.Push(), new AprString("3", p));
            Marshal.WriteIntPtr(a.Push(), new AprString("4", p));
            Marshal.WriteIntPtr(a.Push(), new AprString("5", p));

            AprString[] arr = new AprString[5];
            a.CopyTo(arr, 0);

            AprArray b = AprArray.LazyMake(p, a);
            Assert.AreEqual(a.ToIntPtr(), b.ToIntPtr(), "#F19");

            AprPool sp = AprPool.Create(p);
            Assert.IsFalse(sp.IsNull, "#F01");
            b = AprArray.LazyMake(sp, a);
            Assert.IsFalse(a.ToIntPtr() == b.ToIntPtr(), "#F20");

            Assert.AreEqual("5", a.PopObject().ToString(), "#F03");
            Assert.AreEqual("4", a.PopObject().ToString(), "#F04");
            Assert.AreEqual("3", a.PopObject().ToString(), "#F05");
            Assert.AreEqual("2", a.PopObject().ToString(), "#F06");
            Assert.AreEqual("1", a.PopObject().ToString(), "#F07");

            Assert.AreEqual("5", b.PopObject().ToString(), "#F21");
            Assert.AreEqual("4", b.PopObject().ToString(), "#F22");
            Assert.AreEqual("3", b.PopObject().ToString(), "#F23");
            Assert.AreEqual("2", b.PopObject().ToString(), "#F24");
            Assert.AreEqual("1", b.PopObject().ToString(), "#F25");

            Assert.AreEqual("1", arr[0].ToString(), "#F08");
            Assert.AreEqual("2", arr[1].ToString(), "#F09");
            Assert.AreEqual("3", arr[2].ToString(), "#F10");
            Assert.AreEqual("4", arr[3].ToString(), "#F11");
            Assert.AreEqual("5", arr[4].ToString(), "#F12");

            a = AprArray.Make(p, arr);

            Assert.AreEqual("5", a.PopObject().ToString(), "#F13");
            Assert.AreEqual("4", a.PopObject().ToString(), "#F14");
            Assert.AreEqual("3", a.PopObject().ToString(), "#F15");
            Assert.AreEqual("2", a.PopObject().ToString(), "#F16");
            Assert.AreEqual("1", a.PopObject().ToString(), "#F17");

            a = AprArray.LazyMake(p, new string[] { "1", "2", "3", "4", "5" }, typeof(AprString));
            Assert.AreEqual("5", a.PopObject().ToString(), "#F26");
            Assert.AreEqual("4", a.PopObject().ToString(), "#F27");
            Assert.AreEqual("3", a.PopObject().ToString(), "#F28");
            Assert.AreEqual("2", a.PopObject().ToString(), "#F29");
            Assert.AreEqual("1", a.PopObject().ToString(), "#F30");

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#F18");
        }

        [Test]
        public void ArrayOfIntPtr()
        {
            AprPool p = AprPool.Create();
            Assert.IsFalse(p.IsNull, "#G01");

            AprArray a = AprArray.Make(p, 5, typeof(IntPtr));
            Assert.IsFalse(a.IsNull, "#G02");

            a.Push((IntPtr) 0);
            a.Push((IntPtr) 1);
            a.Push((IntPtr) 1000);
            a.Push((IntPtr) 1000000);
            a.Push((IntPtr) 2147483647);

            IntPtr[] arr = new IntPtr[5];
            a.CopyTo(arr, 0);

            Assert.AreEqual((IntPtr) 2147483647, a.PopObject(), "#G03");
            Assert.AreEqual((IntPtr) 1000000, a.PopObject(), "#G04");
            Assert.AreEqual((IntPtr) 1000, a.PopObject(), "#G05");
            Assert.AreEqual((IntPtr) 1, a.PopObject(), "#G06");
            Assert.AreEqual((IntPtr) 0, a.PopObject(), "#G07");

            Assert.AreEqual((IntPtr) 0, arr[0], "#I08");
            Assert.AreEqual((IntPtr) 1, arr[1], "#I09");
            Assert.AreEqual((IntPtr) 1000, arr[2], "#I10");
            Assert.AreEqual((IntPtr) 1000000, arr[3], "#I11");
            Assert.AreEqual((IntPtr) 2147483647, arr[4], "#I12");

            a = AprArray.Make(p, arr);

            Assert.AreEqual((IntPtr) 2147483647, a.PopObject(), "#G13");
            Assert.AreEqual((IntPtr) 1000000, a.PopObject(), "#G14");
            Assert.AreEqual((IntPtr) 1000, a.PopObject(), "#G15");
            Assert.AreEqual((IntPtr) 1, a.PopObject(), "#G16");
            Assert.AreEqual((IntPtr) 0, a.PopObject(), "#G17");

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#I18");
        }

        [Test]
        public void ArrayOfBool()
        {
            AprPool p = AprPool.Create();
            Assert.IsFalse(p.IsNull, "#G01");

            AprArray a = AprArray.Make(p, 5, typeof(bool));
            Assert.IsFalse(a.IsNull, "#G02");

            a.Push(true);
            a.Push(false);
            a.Push(true);
            a.Push(false);
            a.Push(true);

            bool[] arr = new bool[5];
            a.CopyTo(arr, 0);

            Assert.IsTrue((bool) a.PopObject(), "#G03");
            Assert.IsFalse((bool) a.PopObject(), "#G04");
            Assert.IsTrue((bool) a.PopObject(), "#G05");
            Assert.IsFalse((bool) a.PopObject(), "#G06");
            Assert.IsTrue((bool) a.PopObject(), "#G07");

            Assert.IsTrue(arr[0], "#G08");
            Assert.IsFalse(arr[1], "#G09");
            Assert.IsTrue(arr[2], "#G10");
            Assert.IsFalse(arr[3], "#G11");
            Assert.IsTrue(arr[4], "#G12");

            a = AprArray.Make(p, arr);

            Assert.IsTrue((bool) a.PopObject(), "#G13");
            Assert.IsFalse((bool) a.PopObject(), "#G14");
            Assert.IsTrue((bool) a.PopObject(), "#G15");
            Assert.IsFalse((bool) a.PopObject(), "#G16");
            Assert.IsTrue((bool) a.PopObject(), "#G17");

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#G18");
        }

        [Test]
        public void ArrayOfByte()
        {
            AprPool p = AprPool.Create();
            Assert.IsFalse(p.IsNull, "#I01");

            AprArray a = AprArray.Make(p, 5, typeof(byte));
            Assert.IsFalse(a.IsNull, "#I02");

            a.Push((byte) 0);
            a.Push((byte) 1);
            a.Push((byte) 10);
            a.Push((byte) 100);
            a.Push((byte) 255);

            byte[] arr = new byte[5];
            a.CopyTo(arr, 0);

            Assert.AreEqual((byte) 255, a.PopObject(), "#I03");
            Assert.AreEqual((byte) 100, a.PopObject(), "#I04");
            Assert.AreEqual((byte) 10, a.PopObject(), "#I05");
            Assert.AreEqual((byte) 1, a.PopObject(), "#I06");
            Assert.AreEqual((byte) 0, a.PopObject(), "#I07");

            Assert.AreEqual((byte) 0, arr[0], "#I08");
            Assert.AreEqual((byte) 1, arr[1], "#I09");
            Assert.AreEqual((byte) 10, arr[2], "#I10");
            Assert.AreEqual((byte) 100, arr[3], "#I11");
            Assert.AreEqual((byte) 255, arr[4], "#I12");

            a = AprArray.Make(p, arr);

            Assert.AreEqual((byte) 255, a.PopObject(), "#I13");
            Assert.AreEqual((byte) 100, a.PopObject(), "#I14");
            Assert.AreEqual((byte) 10, a.PopObject(), "#I15");
            Assert.AreEqual((byte) 1, a.PopObject(), "#I16");
            Assert.AreEqual((byte) 0, a.PopObject(), "#I17");

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#I18");
        }

        [Test]
        public void ArrayOfSByte()
        {
            AprPool p = AprPool.Create();
            Assert.IsFalse(p.IsNull, "#J01");

            AprArray a = AprArray.Make(p, 5, typeof(sbyte));
            Assert.IsFalse(a.IsNull, "#J02");

            a.Push((sbyte) 0);
            a.Push((sbyte) -1);
            a.Push((sbyte) 10);
            a.Push((sbyte) -100);
            a.Push((sbyte) 127);

            sbyte[] arr = new sbyte[5];
            a.CopyTo(arr, 0);

            Assert.AreEqual((sbyte) 127, a.PopObject(), "#J03");
            Assert.AreEqual((sbyte) -100, a.PopObject(), "#J04");
            Assert.AreEqual((sbyte) 10, a.PopObject(), "#J05");
            Assert.AreEqual((sbyte) -1, a.PopObject(), "#J06");
            Assert.AreEqual((sbyte) 0, a.PopObject(), "#J07");

            Assert.AreEqual((sbyte) 0, arr[0], "#J08");
            Assert.AreEqual((sbyte) -1, arr[1], "#J09");
            Assert.AreEqual((sbyte) 10, arr[2], "#J10");
            Assert.AreEqual((sbyte) -100, arr[3], "#J11");
            Assert.AreEqual((sbyte) 127, arr[4], "#J12");

            a = AprArray.Make(p, arr);

            Assert.AreEqual((sbyte) 127, a.PopObject(), "#J13");
            Assert.AreEqual((sbyte) -100, a.PopObject(), "#J14");
            Assert.AreEqual((sbyte) 10, a.PopObject(), "#J15");
            Assert.AreEqual((sbyte) -1, a.PopObject(), "#J16");
            Assert.AreEqual((sbyte) 0, a.PopObject(), "#J17");

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#J18");
        }


        [Test]
        public void ArrayOfShort()
        {
            AprPool p = AprPool.Create();
            Assert.IsFalse(p.IsNull, "#I01");

            AprArray a = AprArray.Make(p, 5, typeof(short));
            Assert.IsFalse(a.IsNull, "#I02");

            a.Push((short) 0);
            a.Push((short) -1);
            a.Push((short) 100);
            a.Push((short) -1000);
            a.Push((short) 32767);

            short[] arr = new short[5];
            a.CopyTo(arr, 0);

            Assert.AreEqual((short) 32767, a.PopObject(), "#K03");
            Assert.AreEqual((short) -1000, a.PopObject(), "#K04");
            Assert.AreEqual((short) 100, a.PopObject(), "#K05");
            Assert.AreEqual((short) -1, a.PopObject(), "#K06");
            Assert.AreEqual((short) 0, a.PopObject(), "#K07");

            Assert.AreEqual((short) 0, arr[0], "#K08");
            Assert.AreEqual((short) -1, arr[1], "#K09");
            Assert.AreEqual((short) 100, arr[2], "#K10");
            Assert.AreEqual((short) -1000, arr[3], "#K11");
            Assert.AreEqual((short) 32767, arr[4], "#K12");

            a = AprArray.Make(p, arr);

            Assert.AreEqual((short) 32767, a.PopObject(), "#K13");
            Assert.AreEqual((short) -1000, a.PopObject(), "#K14");
            Assert.AreEqual((short) 100, a.PopObject(), "#K15");
            Assert.AreEqual((short) -1, a.PopObject(), "#K16");
            Assert.AreEqual((short) 0, a.PopObject(), "#K17");

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#K18");
        }

        [Test]
        public void ArrayOfUShort()
        {
            AprPool p = AprPool.Create();
            Assert.IsFalse(p.IsNull, "#L01");

            AprArray a = AprArray.Make(p, 5, typeof(ushort));
            Assert.IsFalse(a.IsNull, "#L02");

            a.Push((ushort) 0);
            a.Push((ushort) 1);
            a.Push((ushort) 100);
            a.Push((ushort) 1000);
            a.Push((ushort) 65535);

            ushort[] arr = new ushort[5];
            a.CopyTo(arr, 0);

            Assert.AreEqual((ushort) 65535, a.PopObject(), "#L03");
            Assert.AreEqual((ushort) 1000, a.PopObject(), "#L04");
            Assert.AreEqual((ushort) 100, a.PopObject(), "#L05");
            Assert.AreEqual((ushort) 1, a.PopObject(), "#L06");
            Assert.AreEqual((ushort) 0, a.PopObject(), "#L07");

            Assert.AreEqual((ushort) 0, arr[0], "#L08");
            Assert.AreEqual((ushort) 1, arr[1], "#L09");
            Assert.AreEqual((ushort) 100, arr[2], "#L10");
            Assert.AreEqual((ushort) 1000, arr[3], "#L11");
            Assert.AreEqual((ushort) 65535, arr[4], "#L12");

            a = AprArray.Make(p, arr);

            Assert.AreEqual((ushort) 65535, a.PopObject(), "#L13");
            Assert.AreEqual((ushort) 1000, a.PopObject(), "#L14");
            Assert.AreEqual((ushort) 100, a.PopObject(), "#L15");
            Assert.AreEqual((ushort) 1, a.PopObject(), "#L16");
            Assert.AreEqual((ushort) 0, a.PopObject(), "#L17");

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#L18");
        }

        [Test]
        public void ArrayOfInt()
        {
            AprPool p = AprPool.Create();
            Assert.IsFalse(p.IsNull, "#M01");

            AprArray a = AprArray.Make(p, 5, typeof(int));
            Assert.IsFalse(a.IsNull, "#M02");

            a.Push((int) 0);
            a.Push((int) -1);
            a.Push((int) 10000);
            a.Push((int) -1000000);
            a.Push((int) 2147483647);

            int[] arr = new int[5];
            a.CopyTo(arr, 0);

            Assert.AreEqual((int) 2147483647, a.PopObject(), "#M03");
            Assert.AreEqual((int) -1000000, a.PopObject(), "#M04");
            Assert.AreEqual((int) 10000, a.PopObject(), "#M05");
            Assert.AreEqual((int) -1, a.PopObject(), "#M06");
            Assert.AreEqual((int) 0, a.PopObject(), "#M07");

            Assert.AreEqual((int) 0, arr[0], "#M08");
            Assert.AreEqual((int) -1, arr[1], "#M09");
            Assert.AreEqual((int) 10000, arr[2], "#M10");
            Assert.AreEqual((int) -1000000, arr[3], "#M11");
            Assert.AreEqual((int) 2147483647, arr[4], "#M12");

            a = AprArray.Make(p, arr);

            Assert.AreEqual((int) 2147483647, a.PopObject(), "#M13");
            Assert.AreEqual((int) -1000000, a.PopObject(), "#M14");
            Assert.AreEqual((int) 10000, a.PopObject(), "#M15");
            Assert.AreEqual((int) -1, a.PopObject(), "#M16");
            Assert.AreEqual((int) 0, a.PopObject(), "#M17");

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#M18");
        }

        [Test]
        public void ArrayOfUInt()
        {
            AprPool p = AprPool.Create();
            Assert.IsFalse(p.IsNull, "#N01");

            AprArray a = AprArray.Make(p, 5, typeof(uint));
            Assert.IsFalse(a.IsNull, "#N02");

            a.Push((uint) 0);
            a.Push((uint) 1);
            a.Push((uint) 10000);
            a.Push((uint) 1000000);
            a.Push((uint) 4294967295);

            uint[] arr = new uint[5];
            a.CopyTo(arr, 0);

            Assert.AreEqual((uint) 4294967295, a.PopObject(), "#N03");
            Assert.AreEqual((uint) 1000000, a.PopObject(), "#N04");
            Assert.AreEqual((uint) 10000, a.PopObject(), "#N05");
            Assert.AreEqual((uint) 1, a.PopObject(), "#N06");
            Assert.AreEqual((uint) 0, a.PopObject(), "#N07");

            Assert.AreEqual((uint) 0, arr[0], "#N08");
            Assert.AreEqual((uint) 1, arr[1], "#N09");
            Assert.AreEqual((uint) 10000, arr[2], "#N10");
            Assert.AreEqual((uint) 1000000, arr[3], "#N11");
            Assert.AreEqual((uint) 4294967295, arr[4], "#N12");

            a = AprArray.Make(p, arr);

            Assert.AreEqual((uint) 4294967295, a.PopObject(), "#N13");
            Assert.AreEqual((uint) 1000000, a.PopObject(), "#N14");
            Assert.AreEqual((uint) 10000, a.PopObject(), "#N15");
            Assert.AreEqual((uint) 1, a.PopObject(), "#N16");
            Assert.AreEqual((uint) 0, a.PopObject(), "#N17");

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#N18");
        }

        [Test]
        public void ArrayOfLong()
        {
            AprPool p = AprPool.Create();
            Assert.IsFalse(p.IsNull, "#O01");

            AprArray a = AprArray.Make(p, 5, typeof(long));
            Assert.IsFalse(a.IsNull, "#O02");

            a.Push((long) 0);
            a.Push((long) -1);
            a.Push((long) 100000);
            a.Push((long) -1000000000);
            a.Push((long) 9223372036854775807);

            long[] arr = new long[5];
            a.CopyTo(arr, 0);

            Assert.AreEqual((long) 9223372036854775807, a.PopObject(), "#O03");
            Assert.AreEqual((long) -1000000000, a.PopObject(), "#O04");
            Assert.AreEqual((long) 100000, a.PopObject(), "#O05");
            Assert.AreEqual((long) -1, a.PopObject(), "#O06");
            Assert.AreEqual((long) 0, a.PopObject(), "#O07");

            Assert.AreEqual((long) 0, arr[0], "#O08");
            Assert.AreEqual((long) -1, arr[1], "#O09");
            Assert.AreEqual((long) 100000, arr[2], "#O10");
            Assert.AreEqual((long) -1000000000, arr[3], "#O11");
            Assert.AreEqual((long) 9223372036854775807, arr[4], "#O12");

            a = AprArray.Make(p, arr);

            Assert.AreEqual((long) 9223372036854775807, a.PopObject(), "#O13");
            Assert.AreEqual((long) -1000000000, a.PopObject(), "#O14");
            Assert.AreEqual((long) 100000, a.PopObject(), "#O15");
            Assert.AreEqual((long) -1, a.PopObject(), "#O16");
            Assert.AreEqual((long) 0, a.PopObject(), "#O17");

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#O18");
        }

        [Test]
        public void ArrayOfULong()
        {
            AprPool p = AprPool.Create();
            Assert.IsFalse(p.IsNull, "#P01");

            AprArray a = AprArray.Make(p, 5, typeof(ulong));
            Assert.IsFalse(a.IsNull, "#P02");

            a.Push((ulong) 0);
            a.Push((ulong) 1);
            a.Push((ulong) 100000);
            a.Push((ulong) 1000000000);
            a.Push((ulong) 18446744073709551615);

            ulong[] arr = new ulong[5];
            a.CopyTo(arr, 0);

            Assert.AreEqual((ulong) 18446744073709551615, a.PopObject(), "#P03");
            Assert.AreEqual((ulong) 1000000000, a.PopObject(), "#P04");
            Assert.AreEqual((ulong) 100000, a.PopObject(), "#P05");
            Assert.AreEqual((ulong) 1, a.PopObject(), "#P06");
            Assert.AreEqual((ulong) 0, a.PopObject(), "#P07");

            Assert.AreEqual((ulong) 0, arr[0], "#P08");
            Assert.AreEqual((ulong) 1, arr[1], "#P09");
            Assert.AreEqual((ulong) 100000, arr[2], "#P10");
            Assert.AreEqual((ulong) 1000000000, arr[3], "#P11");
            Assert.AreEqual((ulong) 18446744073709551615, arr[4], "#P12");

            a = AprArray.Make(p, arr);

            Assert.AreEqual((ulong) 18446744073709551615, a.PopObject(), "#P13");
            Assert.AreEqual((ulong) 1000000000, a.PopObject(), "#P14");
            Assert.AreEqual((ulong) 100000, a.PopObject(), "#P15");
            Assert.AreEqual((ulong) 1, a.PopObject(), "#P16");
            Assert.AreEqual((ulong) 0, a.PopObject(), "#P17");

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#P18");
        }


    }
}