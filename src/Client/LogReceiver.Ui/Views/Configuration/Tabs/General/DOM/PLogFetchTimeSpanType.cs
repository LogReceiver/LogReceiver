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
using Blocks.Core.Extensions;
using LogReceiver.Core;

namespace LogReceiver.Ui.Views.Configuration.Tabs.General.DOM
{
    public class PLogFetchTimeSpanType
    {
        public static IReadOnlyCollection<PLogFetchTimeSpanType> GetAll()
        {
            return EnumExtensions.GetValues<LogFetchTimeSpanType>()
                .Where(e => e != LogFetchTimeSpanType.Unknown)
                .Select(e => new PLogFetchTimeSpanType(e))
                .ToList();
        }

        public LogFetchTimeSpanType Value { get; private set; }

        public PLogFetchTimeSpanType(LogFetchTimeSpanType val)
        {
            Value = val;
        }

        public override string ToString()
        {
            switch (Value)
            {
                case LogFetchTimeSpanType.Everything:
                    return "Everything";
                case LogFetchTimeSpanType.OneMonth:
                    return "One month";
                case LogFetchTimeSpanType.OneWeek:
                    return "One week";
                case LogFetchTimeSpanType.FourDays:
                    return "Four days";
                case LogFetchTimeSpanType.TwoDays:
                    return "Two days";
                case LogFetchTimeSpanType.OneDay:
                    return "One day";
                default:
                    return "<unknown>";
            }
        }
    }
}
