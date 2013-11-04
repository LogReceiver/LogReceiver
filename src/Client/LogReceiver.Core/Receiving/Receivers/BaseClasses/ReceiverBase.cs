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
using System.Reactive.Subjects;
using Blocks.Core.ObservableObjects;
using Blocks.Core.Utilities;
using LogReceiver.Core.Models;
using Microsoft.Practices.ServiceLocation;

namespace LogReceiver.Core.Receiving.Receivers.BaseClasses
{
    public abstract class ReceiverBase : ObservableObjectBase, IReceiver
    {
        public abstract string Name { get; }

        public virtual bool SupportsMachineProfile { get { return false; } }
        public virtual bool SupportsInstanceProfile { get { return false; } }
        public bool SupportsBothMachineAndInstanceProfile { get { return SupportsInstanceProfile && SupportsMachineProfile; } }

        public ISubject<ICollection<CLogEntry>> LogEntryStream { get; private set; }

        #region Property - State
        private ReceiverStateType _state;
        public ReceiverStateType State
        {
            get { return _state; }
            set
            {
                if (_state == value)
                {
                    return;
                }
                _state = value;
                RaisePropertyChanged(() => State);
            }
        }
        #endregion

        #region Property - Description
        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description == value)
                {
                    return;
                }
                _description = value;
                RaisePropertyChanged(() => Description);
            }
        }
        #endregion

        protected IThreadHelper ThreadHelper { get; private set; }

        protected ReceiverBase()
        {
            LogEntryStream = new Subject<ICollection<CLogEntry>>();
            State = ReceiverStateType.InitialState;
            ThreadHelper = ServiceLocator.Current.GetInstance<IThreadHelper>();
        }

        public abstract void Start();

        public abstract void Stop();

        protected void RaiseLogEntryReceived(CLogEntry logEntry)
        {
            RaiseLogEntryReceived(new List<CLogEntry>{logEntry});
        }

        protected void RaiseLogEntryReceived(List<CLogEntry> logEntryEntries)
        {
            LogEntryStream.OnNext(logEntryEntries);
        }
    }
}
