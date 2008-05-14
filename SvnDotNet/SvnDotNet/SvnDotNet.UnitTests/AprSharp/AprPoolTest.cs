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
    public class AprPoolTest {
        [TestFixtureSetUp]
        public void Init()
        {
            //Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
        }

        [Test]
        public void CreateDestroy()
        {
            AprPool p = new AprPool();
            Assert.IsTrue(p.IsNull, "#A01");

            p = AprPool.Create();
            Assert.IsFalse(p.IsNull, "#A02");

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#A03");
        }

        [Test]
        public void CreateDestroySubPool()
        {
            AprPool p = new AprPool();
            Assert.IsTrue(p.IsNull, "#B01");

            p = AprPool.Create();
            Assert.IsFalse(p.IsNull, "#B02");

            AprPool sp = AprPool.Create(p);
            Assert.IsFalse(sp.IsNull, "#B03");
            Assert.AreEqual(((IntPtr) p).ToInt32(), ((IntPtr) (sp.Parent)).ToInt32(), "#B04");

            sp.Destroy();
            Assert.IsTrue(sp.IsNull, "#B05");

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#B06");
        }

        [Test]
        public void CreateDestroyWithCustomAllocator()
        {
            AprAllocator a = AprAllocator.Create();
            Assert.IsFalse(a.IsNull, "#C01");

            AprPool p = AprPool.Create(a);
            Assert.IsFalse(p.IsNull, "#C02");
            Assert.AreEqual(((IntPtr) a).ToInt32(), ((IntPtr) (p.Allocator)).ToInt32(), "#C03");

            a.Owner = p;
            Assert.AreEqual(((IntPtr) p).ToInt32(), ((IntPtr) (a.Owner)).ToInt32(), "#C04");

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#C05");
        }

        [Test]
        public void CreateDestroySubPoolWithCustomAllocator()
        {
            AprPool p = new AprPool();
            Assert.IsTrue(p.IsNull, "#D01");

            p = AprPool.Create();
            Assert.IsFalse(p.IsNull, "#D02");

            AprAllocator a = AprAllocator.Create();
            Assert.IsFalse(a.IsNull, "#D03");

            AprPool sp = AprPool.Create(p, a);
            Assert.IsFalse(sp.IsNull, "#D04");
            Assert.AreEqual(((IntPtr) p).ToInt32(), ((IntPtr) (sp.Parent)).ToInt32(), "#D05");
            Assert.AreEqual(((IntPtr) a).ToInt32(), ((IntPtr) (sp.Allocator)).ToInt32(), "#D06");

            a.Owner = p;
            Assert.AreEqual(((IntPtr) p).ToInt32(), ((IntPtr) (a.Owner)).ToInt32(), "#D07");

            sp.Destroy();
            Assert.IsTrue(sp.IsNull, "#D08");

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#D09");
        }

        [Test]
        public void Alloc()
        {
            AprPool p = new AprPool();
            Assert.IsTrue(p.IsNull, "#E01");

            p = AprPool.Create();
            Assert.IsFalse(p.IsNull, "#E02");

            Assert.IsTrue(p.Alloc(128).ToInt32() != 0, "#E03");
            Assert.IsTrue(p.Alloc(256).ToInt32() != 0, "#E04");
            Assert.IsTrue(p.Alloc(512).ToInt32() != 0, "#E05");
            Assert.IsTrue(p.Alloc(1024).ToInt32() != 0, "#E06");
            Assert.IsTrue(p.Alloc(2048).ToInt32() != 0, "#E07");
            Assert.IsTrue(p.Alloc(4096).ToInt32() != 0, "#E08");
            Assert.IsTrue(p.Alloc(6148).ToInt32() != 0, "#E09");
            Assert.IsTrue(p.Alloc(9216).ToInt32() != 0, "#E10");
            Assert.IsTrue(p.Alloc(12265).ToInt32() != 0, "#E11");
            Assert.IsTrue(p.Alloc(16384).ToInt32() != 0, "#E12");

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#E13");
        }

        [Test]
        public void CAlloc()
        {
            AprPool p = new AprPool();
            Assert.IsTrue(p.IsNull, "#F01");

            p = AprPool.Create();
            Assert.IsFalse(p.IsNull, "#F02");

            IntPtr m = p.CAlloc(256);
            Assert.IsTrue(m.ToInt32() != 0, "#F03");
            Assert.IsTrue(Marshal.ReadInt32(m) == 0, "#F04");

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#F04");
        }

        [Test]
        public void AllocLoop()
        {
            AprPool p = new AprPool();
            Assert.IsTrue(p.IsNull, "#G000001");

            p = AprPool.Create();
            Assert.IsFalse(p.IsNull, "#G000002");

            for(int i = 24; i < 4096; i += 24) {
                Assert.IsTrue(p.Alloc(i).ToInt32() != 0, String.Format("#G{0,6}", i));
            }

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#G000004");
        }

        [Test]
        public void CAllocLoop()
        {
            AprPool p = new AprPool();
            Assert.IsTrue(p.IsNull, "#H000001");

            p = AprPool.Create();
            Assert.IsFalse(p.IsNull, "#H000002");

            for(int i = 24; i < 4096; i += 24) {
                IntPtr m = p.CAlloc(i);
                Assert.IsTrue(m.ToInt32() != 0, String.Format("#H{0,6}a", i));
                Assert.IsTrue(Marshal.ReadInt32(m) == 0, String.Format("#H{0,6}b", i));
                Assert.IsTrue(p.Alloc(i).ToInt32() != 0, String.Format("#H{0,6}c", i));
            }

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#H000004");
        }

        [Test]
        public void Parentship()
        {
            AprPool p = AprPool.Create();
            Assert.IsFalse(p.IsNull, "#I01");

            AprPool sp = AprPool.Create(p);
            Assert.IsFalse(sp.IsNull, "#I02");
            Assert.AreEqual(((IntPtr) p).ToInt32(), ((IntPtr) (sp.Parent)).ToInt32(), "#I03");

            AprPool ssp = AprPool.Create(sp);
            Assert.IsFalse(ssp.IsNull, "#I05");
            Assert.AreEqual(((IntPtr) sp).ToInt32(), ((IntPtr) (ssp.Parent)).ToInt32(), "#I06");

            Assert.IsTrue(p.IsAncestor(sp), "#I08");
            Assert.IsTrue(sp.IsAncestor(ssp), "#I09");
            Assert.IsTrue(p.IsAncestor(ssp), "#I10");
            Assert.IsFalse(sp.IsAncestor(p), "#I11");
            Assert.IsFalse(ssp.IsAncestor(p), "#I12");
            Assert.IsFalse(ssp.IsAncestor(sp), "#I13");

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#I14");
        }

    }
}