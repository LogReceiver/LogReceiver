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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blocks.Core.Extensions;
using LogReceiver.Core.Models;
using LogReceiver.Core.Receiving.Receivers.BaseClasses;

namespace LogReceiver.Core.Receiving.Receivers.LogViewer
{
    public class LogViewerReceiver : TcpReceiverBase, ILogViewerReceiver
    {
        public override string Name { get { return "Legacy TCP receiver"; } }

        protected override Encoding StreamEncoding { get { return Encoding.ASCII; } }
        protected override int Port { get { return 50000; } }

        private readonly IDictionary<string, LogEntryLevelType> _levelMap = new Dictionary<string, LogEntryLevelType>
            {
                {"DEBUG", LogEntryLevelType.Debug},
                {"INFO", LogEntryLevelType.Information},
                {"WARN", LogEntryLevelType.Warning},
                {"ERROR", LogEntryLevelType.Error},
                {"FATAL", LogEntryLevelType.Fatal},
            };

        protected override void ProcessMessage(StringBuilder stringBuilder)
        {
            const string end = "|LOG_END|";
            var str = stringBuilder.ToString();
            var idx = str.LastIndexOf(end, StringComparison.Ordinal);

            if (idx <= 0)
            {
                return;
            }

            var entryStr = str.Substring(0, idx + end.Length);
            entryStr
                .Split(new[] { end }, StringSplitOptions.None)
                .Select(s => s.Trim(new[] { '\n', ' ', '\r' }))
                .ToList()
                .ForEach(ProcessLogEntry);
            stringBuilder.Remove(0, idx + end.Length);
        }

        private void ProcessLogEntry(string logEntryStr)
        {
            //1|TcpAppender|28 Apr 2012 21:15:46,074|DEBUG|1|Systematic.KVK.Shared.Service.KVKServiceLogger|hej

            if (string.IsNullOrEmpty(logEntryStr))
            {
                return;
            }

            var parts = logEntryStr.Split('|');
            if (parts.Length < 7)
            {
                return;
            }

            var logEntry = new CLogEntry
                {
                    Id = parts[0].TryParseLong().GetValueOrDefault(),
                    Time = MapTime(parts[2]),
                    Level = MapLevel(parts[3]),
                    Logger = parts[5],
                    Message = parts[6],
                    ReceivedTime = DateTime.Now,
                };

            ThreadHelper.QueueUserWorkItem(() => RaiseLogEntryReceived(logEntry));
        }

        private static DateTime MapTime(string str)
        {
            // 28 Apr 2012 21:15:46,074
            const string format = "d MMM yyyy HH:mm:ss,fff";
            var time = str.TryParseDateTime(format);
            return time.HasValue
                ? time.Value
                : DateTime.Now;
        }

        private LogEntryLevelType MapLevel(string levelStr)
        {
            return _levelMap.ContainsKey(levelStr.ToUpper())
                ? _levelMap[levelStr]
                : LogEntryLevelType.None;
        }
    }
}
