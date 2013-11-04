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

using System.Collections.Generic;
using System.Linq;
using Blocks.TestHelpers;
using FluentAssertions;
using LogReceiver.Ui.UserControls.LogEntryList.UserControls.FilterSelection.DOM;
using NUnit.Framework;

namespace LogReceiver.Test.Ui.UserControls.FilterSelection
{
    [TestFixture]
    public class PFilterProfileTest : TestBase<PFilterProfile>
    {
        [Test]
        public void T()
        {
            ClassUnderTest.AddNamespaces(new []
                {
                    "LogReceiver.Test.Ui.Models",
                    "LogReceiver.Test.Ui.UserControls",
                    "LogReceiver.Test.Ui.UserControls",
                });
        }

        [Test]
        public void FindOrCreateParentFilter_Empty()
        {
            var pFilter = new PFilter
                {
                    Name = "LogReceiver.Test.Ui",
                };
            var filters = new List<PFilter>();
            ClassUnderTest.FindOrCreateParentFilter(pFilter, filters, new List<string>(), true);

            //filters.Count.Should().Be()
        }
        /*
        [TestCase("LogReceiver", "LogReceiver", "LogReceiver")]
        [TestCase("LogReceiver.Test", "LogReceiver", "LogReceiver")]
        [TestCase("LogReceiver", "LogReceiver.Test", "LogReceiver")]
        [TestCase("LogReceiver.Test.Ui.UserControls", "LogReceiver.Test", "LogReceiver.Test")]
        [TestCase("LogReceiver.Test.Ui", "LogReceiver.Test.Ui", "LogReceiver.Test.Ui")]
        public void FindCommonNamePath(string path1Str, string path2Str, string expectedStr)
        {
            var path1 = path1Str.Split('.').ToList();
            var path2 = path2Str.Split('.').ToList();

            var result = ClassUnderTest.FindCommonNamePath(path1, path2);
            var resultStr = string.Join(".", result);
            Assert.AreEqual(expectedStr, resultStr);
        }*/
    }
}
