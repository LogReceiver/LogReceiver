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
using System.ComponentModel;
using Blocks.Core;
using Blocks.Core.Messaging;
using Blocks.Core.Utilities;
using LogReceiver.Core.Messages;
using Microsoft.Practices.ServiceLocation;
using System.Linq;
using Blocks.Core.Extensions;

namespace LogReceiver.Core.Receiving
{
    public class ReceiverManager : IReceiverManager
    {
        private readonly IMessenger _messenger;
        private readonly IThreadHelper _threadHelper;
        private readonly ILogger _logger;
        private readonly Guid _defaultReceiverInitializer = Guid.Parse("D2927F9E-F644-4F1F-A35F-529365B551F1");

        public List<IReceiverInitializer> ReceiverInitializers { get; private set; }
        public List<IReceiver> Receivers { get; private set; }

        public ReceiverManager(
            IMessenger messenger,
            IThreadHelper threadHelper,
            ILogger logger)
        {
            _messenger = messenger;
            _threadHelper = threadHelper;
            _logger = logger;

            ReceiverInitializers = ServiceLocator.Current.GetAllInstances<IReceiverInitializer>().ToList();
            Receivers = new List<IReceiver>();

            _messenger.Register<ReceiverDiedMessage>(this, m =>
                {
                    if (Receivers.Contains(m.Receiver))
                    {
                        Receivers.Remove(m.Receiver);
                    }
                });
        }

        public void Spawn(IReceiverInitializer receiverInitializer)
        {
            Spawn<IReceiverInitializer, IReceiver>(receiverInitializer, r => { });
        }

        public void Spawn<TReceiverInitializer, TReceiver>(TReceiverInitializer receiverInitializer, Action<TReceiver> configure)
            where TReceiverInitializer : IReceiverInitializer
            where TReceiver : IReceiver
        {
            var receiver = receiverInitializer.Spawn();

            try
            {
                configure((TReceiver)receiver);
            }
            catch (Exception exception)
            {
                _logger.Error("Could not configure receiver", exception);
            }

            Receivers.Add(receiver);
            _messenger.Send<ReceiverSpawnedMessage>(m => m.Receiver = receiver);

            _threadHelper.QueueUserWorkItem(() =>
                {
                    try
                    {
                        receiver.Start();
                        if (receiver.State != ReceiverStateType.Running)
                        {
                            throw new InvalidOperationException("Receiver isn't in running state after being started!");
                        }
                        receiver.PropertyChanged += OnReceiverPropertyChanged;
                    }
                    catch (Exception exception)
                    {
                        Receivers.Remove(receiver);
                        _logger.ErrorFormat(exception, "Could not start receiver '{0}'", receiver.GetType());
                        _messenger.Send<ReceiverDiedMessage>(m =>
                            {
                                m.Exception = exception;
                                m.Reason = ReceiverDiedMessage.ReasonType.Exception;
                                m.Receiver = receiver;
                            });
                    }
                });
        }

        public void Spawn<TReceiverInitializer, TReceiver>(Action<TReceiver> configure) where TReceiverInitializer : IReceiverInitializer where TReceiver : IReceiver
        {
            var receiverInitializer = ServiceLocator.Current.GetInstance<TReceiverInitializer>();
            Spawn(receiverInitializer, configure);
        }

        public void Initialize()
        {
            var receiverInitializer = ReceiverInitializers.Single(ri => ri.Id == _defaultReceiverInitializer);
            Spawn(receiverInitializer);
        }

        public void StopAllReceivers()
        {
            foreach (var receiver in Receivers.ToList())
            {
                try
                {
                    receiver.Stop();
                }
                catch (Exception exception)
                {
                    _logger.ErrorFormat(exception, "Could not stop receiver '{0}'.", receiver.Name);
                }
            }
        }

        private void OnReceiverPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var receiver = (IReceiver) sender;
            if (e.PropertyName == receiver.GetPropertyName(r => r.State) &&
                receiver.State != ReceiverStateType.Running)
            {
                _messenger.Send<ReceiverDiedMessage>(m =>
                    {
                        m.Reason = ReceiverDiedMessage.ReasonType.Stopped;
                        m.Receiver = receiver;
                    });
                Receivers.Remove(receiver);
            }
        }
    }
}
