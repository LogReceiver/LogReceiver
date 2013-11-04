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
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using log4net.Core;
using log4net.Layout;

namespace LogReceiver.Senders.Log4NetAppenders
{
    public class LegacyTcpAppender : log4net.Appender.AppenderSkeleton
    {
        private const int MaxEvents = 5000;
        private NetworkStream _stream;
        private TcpClient _client;
        private long _messageCount;
        private readonly AutoResetEvent _resetEvent = new AutoResetEvent(false);
        private readonly List<LoggingEvent> _events = new List<LoggingEvent>();
        private readonly Thread _thread;
 
        protected override bool RequiresLayout { get { return true; } }
 
        public string Host { get; set; }
        public string Port { get; set; }
        public string SenderName { get; set; }
        public PatternLayout Subject { get; set; }
 
        public LegacyTcpAppender()
        {
            Port = 50000.ToString(CultureInfo.InvariantCulture);
            Host = "localhost";
            SenderName = GetType().Name;

            _thread = new Thread(HandleEvents)
                {
                    IsBackground = true,
                };
            _thread.Start();
        }
 
        protected override void Append(LoggingEvent loggingEvent)
        {
            lock (_events)
            {
                if (_events.Count >= MaxEvents)
                {
                    var first = _events.First();
                    _events.Remove(first);
                }
                _events.Add(loggingEvent);
            }
            _resetEvent.Set();
        }
 
        private void HandleEvents()
        {
            try
            {
                while (true)
                {
                    if (_stream == null || !_events.Any())
                    {
                        _resetEvent.WaitOne(5000);
                    }
 
                    if (!_events.Any())
                    {
                        continue;
                    }
 
                    if (_stream == null)
                    {
                        try
                        {
                            _client = new TcpClient(Host, Int32.Parse(Port));
                            _stream = _client.GetStream();
                        }
                        catch (Exception) { continue; }
                    }
 
                    while (_events.Any())
                    {
                        LoggingEvent loggingEvent;
                        lock (_events)
                        {
                            loggingEvent = _events.First();
                            _events.Remove(loggingEvent);
                        }
 
                        // Format and transmit the message
                        var message = string.Format("{0}|{1}|{2}|LOG_END|\n",
                            ++_messageCount,
                            "TcpAppender",
                            RenderLoggingEvent(loggingEvent));
                        var data = Encoding.ASCII.GetBytes(message);
 
                        try
                        {
                            _stream.Write(data, 0, data.Length);
                        }
                        catch (Exception)
                        {
                            try { _stream.Close(); }
                            catch (Exception) { }
                            try { _client.Close(); }
                            catch (Exception) { }
                            _stream = null;
                            _client = null;
                            break;
                        }
                    }
                }
            }
            catch (Exception) { }
        }
    }
}
