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
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using LogReceiver.Core.Exceptions;

// http://stackoverflow.com/questions/365370/proper-way-to-stop-tcplistener

namespace LogReceiver.Core.Receiving.Receivers.BaseClasses
{
    public abstract class TcpReceiverBase : ReceiverBase
    {
        protected abstract Encoding StreamEncoding { get; }
        protected abstract int Port { get; }

        private TcpListener _listener;
        private Thread _clientListenerThread;
        private bool _shouldListen = true;
        private readonly object _syncRoot = new object();
        private readonly List<Thread> _threads = new List<Thread>();

        protected abstract void ProcessMessage(StringBuilder stringBuilder);

        private static readonly List<SocketError> IgnoredSocketErrors = new List<SocketError>
            {
                SocketError.ConnectionAborted,
                SocketError.ConnectionReset,
                SocketError.Shutdown,
            };

        public override void Start()
        {
            State = ReceiverStateType.Initializing;

            if (null != _listener)
            {
                return;
            }

            Description = string.Format("Port: {0}", Port);

            try
            {
                State = ReceiverStateType.Initializing;

                _listener = new TcpListener(IPAddress.Any, Port);
                _listener.Start();
                _clientListenerThread = new Thread(ListenForClients) { IsBackground = true };
                _clientListenerThread.Start();
            }
            catch (Exception exception)
            {
                State = ReceiverStateType.Faulted;
                throw new ReceiverInitializationFailedException(exception);
            }

            State = ReceiverStateType.Running;
        }

        public override void Stop()
        {
            _shouldListen = false;

            _threads.ForEach(t =>
                {
                    try
                    {
                        t.Abort();
                        t.Join(TimeSpan.FromSeconds(0.25d));
                    }
                    catch (Exception exception)
                    {
                        Logger.Warn("Unexpected exception while stopping receiver thread", exception);
                    }
                });

            if (null != _clientListenerThread)
            {
                try
                {
                    _clientListenerThread.Abort();
                    _clientListenerThread.Join(TimeSpan.FromSeconds(0.25d));
                }
                catch (Exception exception)
                {
                    Logger.Warn("Unexpected exception while stopping thread listening for clients", exception);
                }
            }

            if (null != _listener)
            {
                try
                {
                    _listener.Stop();
                }
                catch (Exception exception)
                {
                    Logger.Warn("Unexpected exception while closing TCP listener", exception);
                }
            }

            State = ReceiverStateType.Stoppped;
        }

        private void ListenForClients()
        {
            try
            {
                while (_shouldListen)
                {
                    try
                    {
                        if (!_listener.Pending())
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(0.25d));
                            continue;
                        }

                        var client = _listener.AcceptTcpClient();
                        var thread = new Thread(() => HandleClientCommunication(client)) {IsBackground = true};
                        thread.Start();
                        _threads.Add(thread);
                    }
                    catch (SocketException exception)
                    {
                        Logger.ErrorFormat(
                            exception,
                            "Unexpected SocketException {0} ({1}) while handling client communication.",
                            exception.ErrorCode,
                            exception.SocketErrorCode);
                    }
                }
            }
            catch (ThreadAbortException)
            {
                // Ignore, as we are merely stopping
            }
            catch (Exception exception)
            {
                Logger.ErrorFormat(exception, "Unexpected exception while listening for clients: {0}", exception.Message);
                State = ReceiverStateType.Faulted;
            }
            finally
            {
                Logger.Debug("Closing TCP listener thread");
            }
        }

        private void HandleClientCommunication(TcpClient tcpClient)
        {
            var buffer = new byte[4096];

            try
            {
                using (tcpClient)
                using (var clientStream = tcpClient.GetStream())
                {
                    try
                    {
                        var sb = new StringBuilder();
                        while (true)
                        {
                            var bytesRead = clientStream.Read(buffer, 0, buffer.Length);
                            if (bytesRead == 0)
                            {
                                break;
                            }

                            sb.Append(Encoding.ASCII.GetString(buffer, 0, bytesRead));

                            lock (_syncRoot)
                            {
                                ProcessMessage(sb);
                            }
                        }
                    }
                    finally
                    {
                        clientStream.Close();
                        tcpClient.Close();
                    }
                }
            }
            catch (ThreadAbortException)
            {
                // Expected when receiver is terminated
            }
            catch (IOException)
            {
                // Expected when client terminates
            }
            catch (SocketException exception)
            {
                if (IgnoredSocketErrors.Contains(exception.SocketErrorCode))
                {
                    Logger.DebugFormat(
                        exception,
                        "Ignored SocketException {0} ({1}) while handling client communication.",
                        exception.ErrorCode,
                        exception.SocketErrorCode);
                }
                else
                {
                    Logger.ErrorFormat(
                        exception,
                        "Unexpected SocketException {0} ({1}) while handling client communication.",
                        exception.ErrorCode,
                        exception.SocketErrorCode);
                }
            }
            catch (Exception exception)
            {
                Logger.Error("Unexpected exception while handling client communication", exception);
            }

            try
            {
                _threads.Remove(Thread.CurrentThread);
            }
            catch (Exception)
            {
                // Ignore
            }
        }
    }
}
