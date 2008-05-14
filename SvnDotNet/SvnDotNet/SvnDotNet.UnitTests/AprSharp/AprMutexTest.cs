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
using NUnit.Framework;
using PumaCode.SvnDotNet.AprSharp;

namespace PumaCode.SvnDotNet.UnitTests.AprSharp {
    [TestFixture]
    public class AprMutexTest {
        [TestFixtureSetUp]
        public void Init()
        {
            //Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
        }

        [Test]
        public void CreateDestroy()
        {
            AprPool p = AprPool.Create();
            Assert.IsFalse(p.IsNull, "#A01");

            AprThreadMutex m = new AprThreadMutex();
            Assert.IsTrue(m.IsNull, "#A02");

            m = AprThreadMutex.Create(p);
            Assert.IsFalse(m.IsNull, "#A03");
            Assert.AreEqual(((IntPtr) p).ToInt32(), ((IntPtr) (m.Pool)).ToInt32(), "#A04");

            m.Destroy();
            Assert.IsTrue(m.IsNull, "#A05");

            m = AprThreadMutex.Create(AprThreadMutex.AprThreadMutexFlags.Unnested, p);
            Assert.IsFalse(m.IsNull, "#A06");
            Assert.AreEqual(((IntPtr) p).ToInt32(), ((IntPtr) (m.Pool)).ToInt32(), "#A04");

            m.Destroy();
            Assert.IsTrue(m.IsNull, "#A07");

            m = AprThreadMutex.Create(AprThreadMutex.AprThreadMutexFlags.Nested, p);
            Assert.IsFalse(m.IsNull, "#A08");
            Assert.AreEqual(((IntPtr) p).ToInt32(), ((IntPtr) (m.Pool)).ToInt32(), "#A04");

            m.Destroy();
            Assert.IsTrue(m.IsNull, "#A09");

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#A10");
        }

        [Test]
        public void TryLockUnlock()
        {
            AprPool p = AprPool.Create();
            Assert.IsFalse(p.IsNull, "#B01");

            AprThreadMutex m = AprThreadMutex.Create(AprThreadMutex.AprThreadMutexFlags.Unnested, p);
            Assert.IsFalse(m.IsNull, "#B02");
            Assert.AreEqual(((IntPtr) p).ToInt32(), ((IntPtr) (m.Pool)).ToInt32(), "#B03");

            Assert.IsTrue(m.TryLock(), "#B04");
            Assert.IsFalse(m.TryLock(), "#B05");
            m.Unlock();
            Assert.IsTrue(m.TryLock(), "#B06");
            m.Unlock();

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#B07");
        }

        [Test]
        public void LockUnlock()
        {
            AprPool p = AprPool.Create();
            Assert.IsFalse(p.IsNull, "#B01");

            AprThreadMutex m = AprThreadMutex.Create(AprThreadMutex.AprThreadMutexFlags.Unnested, p);
            Assert.IsFalse(m.IsNull, "#B02");
            Assert.AreEqual(((IntPtr) p).ToInt32(), ((IntPtr) (m.Pool)).ToInt32(), "#B03");

            m.Lock();
            Assert.IsFalse(m.TryLock(), "#B04");
            m.Unlock();
            Assert.IsTrue(m.TryLock(), "#B05");
            m.Unlock();

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#B06");
        }

        [Test]
        public void AllocatorMutex()
        {
            AprAllocator a = AprAllocator.Create();
            Assert.IsFalse(a.IsNull, "#C01");

            AprPool p = AprPool.Create(a);
            Assert.IsFalse(p.IsNull, "#C02");
            Assert.AreEqual(((IntPtr) a).ToInt32(), ((IntPtr) (p.Allocator)).ToInt32(), "#C03");

            a.Owner = p;
            Assert.AreEqual(((IntPtr) p).ToInt32(), ((IntPtr) (a.Owner)).ToInt32(), "#C04");

            AprThreadMutex m = AprThreadMutex.Create(p);
            Assert.IsFalse(m.IsNull, "#C05");
            Assert.AreEqual(((IntPtr) p).ToInt32(), ((IntPtr) (m.Pool)).ToInt32(), "#C06");

            a.Mutex = m;
            Assert.AreEqual(((IntPtr) m).ToInt32(), ((IntPtr) (a.Mutex)).ToInt32(), "#C07");

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#C08");
        }
    }
}