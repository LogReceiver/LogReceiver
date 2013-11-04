﻿﻿/* -----------------------------------------------------------------------------
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

using System;
using Blocks.Core.Configuration;
using Blocks.Core.Utilities;

namespace LogReceiver.Core
{
    [Serializable]
    public enum LogFetchTimeSpanType
    {
        Unknown,
        Everything,
        OneMonth,
        OneWeek,
        FourDays,
        TwoDays,
        OneDay,
    }

    public interface ILogReceiverUserSettings : IUserConfigurationKeys
    {
        [DefaultConfiguration(true)]
        bool WordWrap { get; set; }

        [DefaultConfiguration(true)]
        bool ParseCSharpStackTrace { get; set; }

        [DefaultConfiguration("")]
        string HubServiceHost { get; set; }

        [DefaultConfiguration(50001)]
        int HubServicePort { get; set; }

        [DefaultConfiguration(false)]
        bool IsHubServiceEnabled { get; set; }

        [DefaultConfiguration(5000)]
        int MaxNumberOfLogEntries { get; set; }

        [DefaultConfiguration(LogFetchTimeSpanType.OneWeek)]
        LogFetchTimeSpanType LogFetchTimeSpan { get; set; }
    }
}