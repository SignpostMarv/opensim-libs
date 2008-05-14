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
    public class AprStringTest {
        [TestFixtureSetUp]
        public void Init()
        {
            //Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
        }

        [Test]
        public void Duplicate()
        {
            AprPool p = AprPool.Create();
            Assert.IsFalse(p.IsNull, "#A01");

            AprString s = new AprString();
            Assert.IsTrue(s.IsNull, "#A02");

            s = AprString.Duplicate(p, "This is a test of string duplication");
            Assert.IsFalse(s.IsNull, "#A03");
            Assert.AreEqual("This is a test of string duplication", s.ToString(), "#A04");

            AprString s2 = AprString.Duplicate(p, s);
            Assert.IsFalse(s2.IsNull, "#A05");
            Assert.AreEqual("This is a test of string duplication", s2.ToString(), "#A06");

            s2 = AprString.Duplicate(p, s, 14);
            Assert.IsFalse(s2.IsNull, "#A07");
            Assert.AreEqual("This is a test", s2.ToString(), "#A08");

            s = AprString.Duplicate(p, "This is a test of string duplication", 14);
            Assert.IsFalse(s.IsNull, "#A09");
            Assert.AreEqual("This is a test", s.ToString(), "#A10");

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#A11");
        }

        [Test]
        public void Length()
        {
            AprPool p = AprPool.Create();
            Assert.IsFalse(p.IsNull, "#A01");

            Assert.AreEqual("".Length, new AprString("", p).Length, "#A02");
            Assert.AreEqual("A".Length, new AprString("A", p).Length, "#A03");
            Assert.AreEqual("AB".Length, new AprString("AB", p).Length, "#A04");
            Assert.AreEqual("ABC".Length, new AprString("ABC", p).Length, "#A05");
        }
    }
}