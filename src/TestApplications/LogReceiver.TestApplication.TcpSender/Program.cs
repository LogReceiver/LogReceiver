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
using System.Net.Sockets;
using System.Threading;

namespace LogReceiver.TestApplication.TcpSender
{
    class Program
    {
        private static readonly Random Random = new Random();

        private static readonly List<string> LoggerNames = new List<string>
            {
                "LogReceiver.Ui.Views.Main.Commands.TestSendLogEventCommand",
                "LogReceiver.Ui.Views.Main.MainViewModel",
                "LogReceiver.Ui.Views.About.AboutViewModel",
                "LogReceiver.Core.Receiving.ReceiverManager",
                "LogReceiver.Ui.UserControls.FilterSelection.FilterSelectionViewModel",
            };

        static void Main()
        {
            try
            {
                var client = new TcpClient("127.0.0.1", 50000);
                var stream = client.GetStream();

                Enumerable.Range(0, 100).ToList().ForEach(i =>
                    {
                        var message = string.Format("{0}|TcpAppender|{3}|{1}|1|{2}|Log test\nLog test\nLog test|LOG_END|\n",
                            i,
                            GetLevel(i),
                            GetLoggerName(),
                            DateTime.Now.ToString("d MMM yyyy HH:mm:ss,fff"));
                        var data = System.Text.Encoding.ASCII.GetBytes(message);
                        stream.Write(data, 0, data.Length);
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                    });
                stream.Flush();
                stream.Close();
                client.Close();
            }
            catch (Exception exception)
            {
                Console.WriteLine("Could not send test events: {0}", exception.Message);
            }
        }

        private static readonly List<string> Levels = new List<string> {"INFO", "DEBUG", "WARN", "ERROR", "FATAL",};
        private static string GetLevel(int i)
        {
            return Levels[i%Levels.Count];
        }

        private static string GetLoggerName()
        {
            return LoggerNames[Random.Next(0, LoggerNames.Count)];
        }
    }
}
