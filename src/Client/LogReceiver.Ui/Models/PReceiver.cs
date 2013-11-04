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

using System.ComponentModel;
using Blocks.Core.ObservableObjects;
using LogReceiver.Core.Receiving;
using Blocks.Core.Extensions;

namespace LogReceiver.Ui.Models
{
    public class PReceiver : ObservableObjectBase
    {
        public IReceiver Receiver { get; private set; }

        public string Name { get { return Receiver.Name; } }

        #region Property - IsLoading
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading == value)
                {
                    return;
                }
                _isLoading = value;
                RaisePropertyChanged(() => IsLoading);
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

        public PReceiver(IReceiver receiver)
        {
            Receiver = receiver;

            receiver.PropertyChanged += ReceiverOnPropertyChanged;
            CalculateIsLoading();
            Description = Receiver.Description;
        }

        private void ReceiverOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Receiver.GetPropertyName(r => r.State))
            {
                CalculateIsLoading();
            }
            else if (e.PropertyName == Receiver.GetPropertyName(r => r.Description))
            {
                Description = Receiver.Description;
            }
        }

        private void CalculateIsLoading()
        {
            IsLoading =
                Receiver.State == ReceiverStateType.Initializing ||
                Receiver.State == ReceiverStateType.InitialState;
        }
    }
}
