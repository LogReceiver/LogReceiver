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
using System.Windows.Media;
using Blocks.Core.Exceptions;
using Blocks.Core.Utilities;
using Blocks.Mvvm.Commands.BuiltIn;
using Blocks.Mvvm.Services;
using Blocks.Mvvm.Utilities;
using Blocks.Mvvm.ViewModels;
using Blocks.Mvvm.Views;
using LogReceiver.Core.Messages;
using LogReceiver.Core.Receiving;
using LogReceiver.Ui.UserControls.LogEntryList;
using LogReceiver.Ui.Views.Main.Commands;

namespace LogReceiver.Ui.Views.Main
{
    public class MainViewModel : WindowViewViewModelBase, IMainViewModel
    {
        private readonly IDispatcherHelper dispatcherHelper;
        private readonly IReceiverManager receiverManager;
        private readonly IDialogService dialogService;
        private readonly IReflectionHelper reflectionHelper;
        private readonly Dictionary<IReceiver, IViewModel> receiverViewModels = new Dictionary<IReceiver, IViewModel>(); 

        public IQuitApplicationCommand QuitApplicationCommand { get; private set; }
        public ITestSendLogEventCommand TestSendLogEventCommand { get; private set; }
        public IShowAboutCommand ShowAboutCommand { get; private set; }
        public IShowSettingsCommand ShowSettingsCommand { get; private set; }
        public ICloseReceiverTabCommand CloseReceiverTabCommand { get; private set; }
        public IOpenFeaturesLinkCommand OpenFeaturesLinkCommand { get; private set; }

        public event SpawnReceiverTabDeledate ReceiverSpawned;

        #region Property - StatusBarText
        public const string PropertyStatusBarText = "StatusBarText";
        private string _statusBarText;
        public string StatusBarText
        {
            get { return _statusBarText; } 
            private set
            {
                if (_statusBarText == value)
                {
                    return;
                }
                _statusBarText = value;
                RaisePropertyChanged(PropertyStatusBarText);
            }
        }
        #endregion

        #region Property - StatusBarBrush
        public const string PropertyStatusBarBrush = "StatusBarBrush";
        private Brush _statusBarBrush = Brushes.DarkGray;
        public Brush StatusBarBrush
        {
            get { return _statusBarBrush; }
            private set
            {
                if (_statusBarBrush == value)
                {
                    return;
                }
                _statusBarBrush = value;
                RaisePropertyChanged(PropertyStatusBarBrush);
            }
        }
        #endregion

        public MainViewModel(
            IDispatcherHelper dispatcherHelper,
            IReceiverManager receiverManager,
            IDialogService dialogService,
            IReflectionHelper reflectionHelper)
        {
            QuitApplicationCommand = CreateAndBindChild<IQuitApplicationCommand>();
            TestSendLogEventCommand = CreateAndBindChild<ITestSendLogEventCommand, IMainViewModel>();
            ShowAboutCommand = CreateAndBindChild<IShowAboutCommand, IMainViewModel>();
            ShowSettingsCommand = CreateAndBindChild<IShowSettingsCommand, IMainViewModel>();
            CloseReceiverTabCommand = CreateAndBindChild<ICloseReceiverTabCommand, IMainViewModel>();
            OpenFeaturesLinkCommand = CreateAndBindChild<IOpenFeaturesLinkCommand, IMainViewModel>();

            this.dispatcherHelper = dispatcherHelper;
            this.receiverManager = receiverManager;
            this.dialogService = dialogService;
            this.reflectionHelper = reflectionHelper;

            StatusBarText = string.Format("v{0}", reflectionHelper.GetVersionOfAssemblyOwningType<MainViewModel>());

            Messenger.Register<ReceiverSpawnedMessage>(this, m => RaiseReceiverSpawned(m.Receiver));

            Messenger.Register<ReceiverDiedMessage>(this, m =>
                {
                    if (!receiverViewModels.ContainsKey(m.Receiver))
                    {
                        return;
                    }
                    RemoveDisposable(receiverViewModels[m.Receiver]);
                    receiverViewModels.Remove(m.Receiver);
                    switch (m.Reason)
                    {
                        case ReceiverDiedMessage.ReasonType.Exception:
                            this.dispatcherHelper.DispatchToUiThread(() =>
                                {
                                    if (m.Exception is UserExplainableException)
                                    {
                                        this.dialogService.ShowError(
                                            ResidingWindowViewViewModel,
                                            "Receiver died",
                                            ((UserExplainableException)m.Exception).UserMessage);
                                    }
                                    else
                                    {
                                        this.dialogService.ShowErrorFormat(
                                            ResidingWindowViewViewModel,
                                            "Receiver died",
                                            "Receiver '{0}' died due to exception: {1}",
                                            m.Receiver.Name,
                                            m.Exception.Message);
                                    }
                                });
                            break;
                    }
                });

            dispatcherHelper.DispatchToUiThread(() => this.receiverManager.Initialize());
        }

        public void ViewReady()
        {
            foreach (var receiver in receiverManager.Receivers.Where(r => !receiverViewModels.ContainsKey(r)))
            {
                RaiseReceiverSpawned(receiver);
            }
        }

        public override void OnViewClosed(IWindowView windowView)
        {
            receiverManager.StopAllReceivers();
            QuitApplicationCommand.Execute(null);
        }

        private void RaiseReceiverSpawned(IReceiver receiver)
        {
            if (ReceiverSpawned == null)
            {
                return;
            }

            var viewModel = CreateAndBindChild<ILogEntryListViewModel>();
            viewModel.Receiver = receiver;
            receiverViewModels.Add(receiver, viewModel);
            ReceiverSpawned(receiver.Name, viewModel);
        }
    }
}
