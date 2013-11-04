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
using System.Collections.ObjectModel;
using System.Linq;
using Blocks.Mvvm.Utilities;
using Blocks.Mvvm.ViewModels;
using LogReceiver.Core.Messages;
using LogReceiver.Core.Receiving;
using LogReceiver.Ui.Models;
using LogReceiver.Ui.Views.Configuration.Tabs.Receivers.Commands;

namespace LogReceiver.Ui.Views.Configuration.Tabs.Receivers
{
    public class ReceiversTabViewModel : SpecificChildViewModelBase<IConfigurationViewModel>, IReceiversTabViewModel
    {
        private readonly IReceiverManager _receiverManager;
        private readonly IDispatcherHelper _dispatcherHelper;

        public ISpawnReceiverCommand SpawnReceiverCommand { get; private set; }

        public IEnumerable<PReceiverInitializer> ReceiverInitializers { get; private set; }
        public ObservableCollection<PReceiver> Receivers { get; private set; }

        #region Property - SelectedReceiverInitializer
        private PReceiverInitializer _selectedReceiverInitializer;
        public PReceiverInitializer SelectedReceiverInitializer
        {
            get { return _selectedReceiverInitializer; }
            set
            {
                if (_selectedReceiverInitializer == value)
                {
                    return;
                }
                _selectedReceiverInitializer = value;
                RaisePropertyChanged(() => SelectedReceiverInitializer);
            }
        }
        #endregion

        #region Property - SelectedReceiver
        private PReceiver _selectedReceiver;
        public PReceiver SelectedReceiver
        {
            get { return _selectedReceiver; }
            set
            {
                if (_selectedReceiver == value)
                {
                    return;
                }
                _selectedReceiver = value;
                RaisePropertyChanged(() => SelectedReceiver);
            }
        }
        #endregion

        public ReceiversTabViewModel(
            IReceiverManager receiverManager,
            IDispatcherHelper dispatcherHelper)
        {
            _receiverManager = receiverManager;
            _dispatcherHelper = dispatcherHelper;

            ReceiverInitializers = _receiverManager.ReceiverInitializers
                .Select(ri => new PReceiverInitializer(ri))
                .ToList();
            Receivers = new ObservableCollection<PReceiver>(_receiverManager.Receivers.Select(r => new PReceiver(r)));

            SpawnReceiverCommand = CreateAndBindChild<ISpawnReceiverCommand, IReceiversTabViewModel>();

            Messenger.Register<ReceiverSpawnedMessage>(this, m => _dispatcherHelper.DispatchToUiThread(() => Receivers.Add(new PReceiver(m.Receiver))));
            Messenger.Register<ReceiverDiedMessage>(this, m => _dispatcherHelper.DispatchToUiThread(() =>
                {
                    var pReceiver = Receivers.SingleOrDefault(r => r.Receiver == m.Receiver);
                    if (pReceiver != null)
                    {
                        Receivers.Remove(pReceiver);
                    }
                }));
        }
    }
}
