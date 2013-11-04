﻿/* -----------------------------------------------------------------------------
 * This source file is part of LogReceiver application
 * For the latest info, contact r@smus.nu
 *
 * Copyright (c) 2012 BLOCKS.DK
 * 
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License as published by the Free Software
 * Foundation; either version 3 of the License, or (at your option) any later
 * version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License along with
 * this program; if not, write to the Free Software Foundation, Inc., 59 Temple
 * Place - Suite 330, Boston, MA 02111-1307, USA, or go to
 * http://www.gnu.org/copyleft/lesser.txt.
 * -----------------------------------------------------------------------------
 */

using Blocks.TestHelpers;
using FluentAssertions;
using LogReceiver.Core.StackTraces;
using NUnit.Framework;

namespace LogReceiver.Test.Core.StackTraces
{
    [TestFixture]
    public class CSharpStackTraceParserTest : TestBase<CSharpStackTraceParser>
    {
        private const string TestSet1 =
            @"
   Service exception:System.InvalidOperationException: Sequence contains no matching element
   at System.Linq.Enumerable.First[TSource](IEnumerable`1 source, Func`2 predicate)
   at Systematic.KVKService.Bll.Storage.StorageGateway.ConvertKgToFe(IEnumerable`1 consumptions, ICollection`1 stocks) in C:\RTC\WS Main\DMS\src\KVKService\Bll\Storage\StorageGateway.cs:line 332
   at Systematic.KVKService.Bll.Storage.StorageGateway.GetQuantityPriceByType(Int64 agriBusinessCvrNr, List`1 productCodes, Period period, List`1 stockEventTypes) in C:\RTC\WS Main\DMS\src\KVKService\Bll\Storage\StorageGateway.cs:line 324";

        [Test]
        public void Set1()
        {
            var stackTrace = ClassUnderTest.Parse(TestSet1);
            stackTrace.MethodCalls.Should().HaveCount(3);

            var line1 = stackTrace.MethodCalls[0];
            line1.Namespace.Should().Be("System.Linq");
            line1.Class.Should().Be("Enumerable");
            line1.Method.Should().Be("First[TSource]");

            var line2 = stackTrace.MethodCalls[1];
            line2.Namespace.Should().Be("Systematic.KVKService.Bll.Storage");
            line2.Class.Should().Be("StorageGateway");
            line2.Method.Should().Be("ConvertKgToFe");
            line2.File.Should().Be(@"C:\RTC\WS Main\DMS\src\KVKService\Bll\Storage\StorageGateway.cs");
            line2.Line.GetValueOrDefault().Should().Be(332);

            var line3 = stackTrace.MethodCalls[2];
            line3.Namespace.Should().Be("Systematic.KVKService.Bll.Storage");
            line3.Class.Should().Be("StorageGateway");
            line3.Method.Should().Be("GetQuantityPriceByType");
            line3.File.Should().Be(@"C:\RTC\WS Main\DMS\src\KVKService\Bll\Storage\StorageGateway.cs");
            line3.Line.GetValueOrDefault().Should().Be(324);
        }
    }
}
