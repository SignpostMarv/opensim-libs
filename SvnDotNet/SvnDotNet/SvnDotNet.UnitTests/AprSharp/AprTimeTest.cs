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
using System.Threading;
using NUnit.Framework;
using PumaCode.SvnDotNet.AprSharp;

namespace PumaCode.SvnDotNet.UnitTests.AprSharp {
    [TestFixture]
    public class AprTimeTest {
        [TestFixtureSetUp]
        public void Init()
        {
            //Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
        }

        [Test]
        public void Now()
        {
            decimal now = AprTime.Now();
            Thread.Sleep(100);
            decimal now2 = AprTime.Now();

            Assert.AreNotEqual(now, now2, "#A01");
        }

        [Test]
        public void CTime()
        {
            // This is not ideal test (it uses AprTime.FromDateTime)
            // but now it is not timezone dependent
            DateTime dt = new DateTime(2004, 6, 18, 19, 15, 08, 667);
            long aprTm = AprTime.FromDateTime(dt.ToUniversalTime());
            Assert.AreEqual("Fri Jun 18 19:15:08 2004", AprTime.CTime(aprTm), "#B01");
        }

        [Test]
        public void Rfc822Date()
        {
            Assert.AreEqual("Fri, 18 Jun 2004 23:15:08 GMT", AprTime.Rfc822Date(1087600508667156), "#C01");
        }

        [Test]
        public void AprTimeToDateTime()
        {
            long at = 1087600508667000;
            DateTime dt = new DateTime(2004, 6, 18, 23, 15, 08, 667);
            Assert.AreEqual(at, AprTime.FromDateTime(dt), "#C01");
            Assert.AreEqual(dt, AprTime.ToDateTime(at), "#C02");
        }

        [Test]
        public void TimeExpPoolAlloc()
        {
            AprPool p = AprPool.Create();
            Assert.IsFalse(p.IsNull, "#D01");

            AprTimeExp t = new AprTimeExp();
            Assert.IsTrue(t.IsNull, "#D02");

            t = AprTimeExp.Alloc(p);
            Assert.IsFalse(t.IsNull, "#D03");

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#D04");
        }

        [Test]
        public void TimeExpManagedAlloc()
        {
            AprTimeExp t = new AprTimeExp();
            Assert.IsTrue(t.IsNull, "#E01");

            GCHandle mt;
            t = AprTimeExp.Alloc(out mt);
            Assert.IsFalse(t.IsNull, "#E02");
            Assert.IsTrue(mt.IsAllocated, "#E03");

            mt.Free();
        }

        [Test]
        public void TimeExpPoolTest()
        {
            AprPool p = AprPool.Create();
            Assert.IsFalse(p.IsNull, "#F01");

            AprTimeExp t = AprTimeExp.Alloc(p);
            Assert.IsFalse(t.IsNull, "#F02");

            TimeExpTest1(t, "#F");

            t.ClearPtr();
            t = AprTimeExp.Alloc(p);
            Assert.IsFalse(t.IsNull, "#F03");

            TimeExpTest2(t, "#F");

            t.ClearPtr();
            t = AprTimeExp.Alloc(p);
            Assert.IsFalse(t.IsNull, "#F04");

            TimeExpTest3(t, "#F");

            t.ClearPtr();
            t = AprTimeExp.Alloc(p);
            Assert.IsFalse(t.IsNull, "#F04");

            TimeExpTest4(t, "#F");

            p.Destroy();
            Assert.IsTrue(p.IsNull, "#F05");
        }

        [Test]
        public void TimeExpManagedTest()
        {
            GCHandle mt;
            AprTimeExp t = AprTimeExp.Alloc(out mt);
            Assert.IsFalse(t.IsNull, "#G01");
            Assert.IsTrue(mt.IsAllocated, "#G02");

            TimeExpTest1(t, "#G");

            t.ClearPtr();
            mt.Free();
            t = AprTimeExp.Alloc(out mt);
            Assert.IsFalse(t.IsNull, "#G03");

            TimeExpTest2(t, "#G");

            t.ClearPtr();
            mt.Free();
            t = AprTimeExp.Alloc(out mt);
            Assert.IsFalse(t.IsNull, "#G04");

            TimeExpTest3(t, "#G");

            t.ClearPtr();
            mt.Free();
            t = AprTimeExp.Alloc(out mt);
            Assert.IsFalse(t.IsNull, "#G04");

            TimeExpTest4(t, "#G");

            mt.Free();
        }

        public void TimeExpTest1(AprTimeExp t, string tag)
        {
            t.Time = 1087600508667156;
            Assert.AreEqual(1087600508667156, t.Time, tag + "06");
            Assert.AreEqual(1087600508667156, t.GmtTime, tag + "07");
            Assert.AreEqual(2004, t.Year, tag + "08");
            Assert.AreEqual(6, t.Month, tag + "09");
            Assert.AreEqual(18, t.Day, tag + "10");
            Assert.AreEqual(23, t.Hours, tag + "11");
            Assert.AreEqual(15, t.Minutes, tag + "12");
            Assert.AreEqual(8, t.Seconds, tag + "13");
            Assert.AreEqual(667156, t.MicroSeconds, tag + "14");
            Assert.AreEqual(5, t.WeekDay, tag + "15");
            Assert.AreEqual(169, t.YearDay, tag + "16");
            Assert.AreEqual(0, t.TimeZone, tag + "17");
            Assert.IsFalse(t.IsDaylightSaving, tag + "18");
            Assert.AreEqual("2004/06/18 23:15:08", t.ToString("%Y/%m/%d %H:%M:%S"), tag + "19");

            t.Time = 1087607708667156;
            t.TimeZone = 7200;
            Assert.AreEqual(1087607708667156, t.Time, tag + "20");
            Assert.AreEqual(1087600508667156, t.GmtTime, tag + "21");
            Assert.AreEqual(2004, t.Year, tag + "22");
            Assert.AreEqual(6, t.Month, tag + "23");
            Assert.AreEqual(19, t.Day, tag + "24");
            Assert.AreEqual(1, t.Hours, tag + "25");
            Assert.AreEqual(15, t.Minutes, tag + "26");
            Assert.AreEqual(8, t.Seconds, tag + "26");
            Assert.AreEqual(667156, t.MicroSeconds, tag + "28");
            Assert.AreEqual(6, t.WeekDay, tag + "29");
            Assert.AreEqual(170, t.YearDay, tag + "30");
            Assert.AreEqual(7200, t.TimeZone, tag + "31");
            Assert.IsFalse(t.IsDaylightSaving, tag + "32");
            Assert.AreEqual("2004/06/19 01:15:08", t.ToString("%Y/%m/%d %H:%M:%S"), tag + "33");
        }

        public void TimeExpTest2(AprTimeExp t, string tag)
        {
            t.SetTimeTZ(1087600508667156, 7200);
            Assert.AreEqual(1087607708667156, t.Time, tag + "34");
            Assert.AreEqual(1087600508667156, t.GmtTime, tag + "35");
            Assert.AreEqual(2004, t.Year, tag + "36");
            Assert.AreEqual(6, t.Month, tag + "37");
            Assert.AreEqual(19, t.Day, tag + "38");
            Assert.AreEqual(1, t.Hours, tag + "39");
            Assert.AreEqual(15, t.Minutes, tag + "40");
            Assert.AreEqual(8, t.Seconds, tag + "41");
            Assert.AreEqual(667156, t.MicroSeconds, tag + "42");
            Assert.AreEqual(6, t.WeekDay, tag + "43");
            Assert.AreEqual(170, t.YearDay, tag + "44");
            Assert.AreEqual(7200, t.TimeZone, tag + "45");
            Assert.IsFalse(t.IsDaylightSaving, tag + "46");
            Assert.AreEqual("2004/06/19 01:15:08", t.ToString("%Y/%m/%d %H:%M:%S"), tag + "47");
        }

        public void TimeExpTest3(AprTimeExp t, string tag)
        {
            t.Year = 2004;
            t.Month = 6;
            t.Day = 19;
            t.Hours = 1;
            t.Minutes = 15;
            t.Seconds = 8;
            t.MicroSeconds = 667156;
            t.TimeZone = 7200;
            t.IsDaylightSaving = true;
            Assert.AreEqual(1087607708667156, t.Time, tag + "48");
            Assert.AreEqual(1087600508667156, t.GmtTime, tag + "49");
        }

        public void TimeExpTest4(AprTimeExp t, string tag)
        {
            t.GmtTime = 1087600508667156;
            Assert.AreEqual(1087600508667156, t.GmtTime, tag + "50");
            Assert.AreEqual(1087600508667156 + ((long) t.TimeZone * 1000000), t.Time, tag + "51");
        }
    }
}