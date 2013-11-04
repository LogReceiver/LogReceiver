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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Media;
using Blocks.Core.Configuration;
using Blocks.Mvvm.Utilities;
using Blocks.Mvvm.ViewModels;
using LogReceiver.Core;
using LogReceiver.Core.Models;
using LogReceiver.Core.Receiving;
using LogReceiver.Ui.Models;
using Blocks.Core.Extensions;
using LogReceiver.Ui.Models.Mapping;
using LogReceiver.Ui.UserControls.LogEntryList.Commands;
using LogReceiver.Ui.UserControls.LogEntryList.UserControls.FilterSelection;

namespace LogReceiver.Ui.UserControls.LogEntryList
{
    public class LogEntryListViewModel : ViewModelBase, ILogEntryListViewModel
    {
        private readonly IDispatcherHelper _dispatcherHelper;
        private readonly IUserConfiguration<ILogReceiverUserSettings> _configuration;
        private readonly IUiMapper _coreToUi;
        private Regex _regex;

        public CollectionViewSource LogEntriesViewSource { get; private set; }
        public event LogEntryListViewClosedDelegate LogEntryListViewClosed;
        public IShowLogEntryDetailsCommand ShowLogEntryDetailsCommand { get; private set; }
        public IClearLogEntriesCommand ClearLogEntriesCommand { get; private set; }
        public IDeselectFilterCommand DeselectFilterCommand { get; private set; }
        public IDeselectLogEntryCommand DeselectLogEntryCommand { get; private set; }
        public IExportLogEntriesCommand ExportLogEntriesCommand { get; private set; }
        public IFilterSelectionControlModel FilterSelectionControlModel { get; private set; }

        #region Property - IsDebugEnabled
        private bool _isDebugEnabled;
        public bool IsDebugEnabled
        {
            get { return _isDebugEnabled; }
            set
            {
                if (_isDebugEnabled == value)
                {
                    return;
                }
                _isDebugEnabled = value;
                RaisePropertyChanged(() => IsDebugEnabled);
            }
        }
        #endregion

        #region Property - IsInformationEnabled
        private bool _isInformationEnabled;
        public bool IsInformationEnabled
        {
            get { return _isInformationEnabled; }
            set
            {
                if (_isInformationEnabled == value)
                {
                    return;
                }
                _isInformationEnabled = value;
                RaisePropertyChanged(() => IsInformationEnabled);
            }
        }
        #endregion

        #region Property - IsWarningEnabled
        private bool _isWarningEnabled;
        public bool IsWarningEnabled
        {
            get { return _isWarningEnabled; }
            set
            {
                if (_isWarningEnabled == value)
                {
                    return;
                }
                _isWarningEnabled = value;
                RaisePropertyChanged(() => IsWarningEnabled);
            }
        }
        #endregion

        #region Property - IsErrorEnabled
        private bool _isErrorEnabled;
        public bool IsErrorEnabled
        {
            get { return _isErrorEnabled; }
            set
            {
                if (_isErrorEnabled == value)
                {
                    return;
                }
                _isErrorEnabled = value;
                RaisePropertyChanged(() => IsErrorEnabled);
            }
        }
        #endregion

        #region Property - IsFatalEnabled
        private bool _isFatalEnabled;
        public bool IsFatalEnabled
        {
            get { return _isFatalEnabled; }
            set
            {
                if (_isFatalEnabled == value)
                {
                    return;
                }
                _isFatalEnabled = value;
                RaisePropertyChanged(() => IsFatalEnabled);
            }
        }
        #endregion

        #region Property - IsActive
        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive == value)
                {
                    return;
                }
                _isActive = value;
                RaisePropertyChanged(() => IsActive);
            }
        }
        #endregion

        #region Property - IsFilterPopupOpen
        private bool _isFilterPopupOpen;
        public bool IsFilterPopupOpen
        {
            get { return _isFilterPopupOpen; }
            set
            {
                if (_isFilterPopupOpen == value)
                {
                    return;
                }
                _isFilterPopupOpen = value;
                RaisePropertyChanged(() => IsFilterPopupOpen);
            }
        }
        #endregion

        #region Property - RegexExpression
        private string _regexExpression;
        public string RegexExpression
        {
            get { return _regexExpression; }
            set
            {
                if (_regexExpression == value)
                {
                    return;
                }
                _regexExpression = value;
                RaisePropertyChanged(() => RegexExpression);
            }
        }
        #endregion

        #region Property - IsRegexExpressionValid
        private bool _isRegexExpressionValid;
        public bool IsRegexExpressionValid
        {
            get { return _isRegexExpressionValid; }
            set
            {
                if (_isRegexExpressionValid == value)
                {
                    return;
                }
                _isRegexExpressionValid = value;
                RaisePropertyChanged(() => IsRegexExpressionValid);
            }
        }
        #endregion

        #region Property - RegexExpressionBackground
        private Brush _regexExpressionBackground;
        public Brush RegexExpressionBackground
        {
            get { return _regexExpressionBackground; }
            set
            {
                if (_regexExpressionBackground == value)
                {
                    return;
                }
                _regexExpressionBackground = value;
                RaisePropertyChanged(() => RegexExpressionBackground);
            }
        }
        #endregion

        public ObservableCollection<PLogEntry> LogEntries { get; private set; }

        #region Property - SelectedLogEntry
        private PLogEntry _selectedLogEntry;
        public PLogEntry SelectedLogEntry
        {
            get { return _selectedLogEntry; }
            set
            {
                if (_selectedLogEntry == value)
                {
                    return;
                }
                _selectedLogEntry = value;
                RaisePropertyChanged(() => SelectedLogEntry);
            }
        }
        #endregion

        #region Property - Receiver
        private IReceiver _receiver;
        public IReceiver Receiver
        {
            get { return _receiver; }
            set
            {
                if (_receiver == value)
                {
                    return;
                }
                _receiver = value;
                RaisePropertyChanged(() => Receiver);
            }
        }
        #endregion

        public LogEntryListViewModel(
            IDispatcherHelper dispatcherHelper,
            IUserConfiguration<ILogReceiverUserSettings> configuration,
            IUiMapper coreToUi)
        {
            LogEntries = new ObservableCollection<PLogEntry>();
            LogEntries.CollectionChanged += OnLogEntriesCollectionChanged;
            LogEntriesViewSource = new CollectionViewSource()
                {
                    Source = LogEntries,
                };

            FilterSelectionControlModel = CreateAndBindChild<IFilterSelectionControlModel, ILogEntryListViewModel>();
            FilterSelectionControlModel.FilterSelectionChanged += s => LogEntriesViewSource.View.Refresh();

            _dispatcherHelper = dispatcherHelper;
            _configuration = configuration;
            _coreToUi = coreToUi;

            LogEntriesViewSource.SortDescriptions.Add(new SortDescription(PLogEntry.PropertyTime, ListSortDirection.Descending));
            LogEntriesViewSource.SortDescriptions.Add(new SortDescription(PLogEntry.PropertyId, ListSortDirection.Descending));
            LogEntriesViewSource.Filter += LogEntriesViewSourceFilter;

            IsDebugEnabled = true;
            IsInformationEnabled = true;
            IsWarningEnabled = true;
            IsErrorEnabled = true;
            IsFatalEnabled = true;
            RegexExpressionBackground = Brushes.White;

            ShowLogEntryDetailsCommand = CreateAndBindChild<IShowLogEntryDetailsCommand, ILogEntryListViewModel>();
            ClearLogEntriesCommand = CreateAndBindChild<IClearLogEntriesCommand, ILogEntryListViewModel>();
            DeselectFilterCommand = CreateAndBindChild<IDeselectFilterCommand, ILogEntryListViewModel>();
            DeselectLogEntryCommand = CreateAndBindChild<IDeselectLogEntryCommand, ILogEntryListViewModel>();
            ExportLogEntriesCommand = CreateAndBindChild<IExportLogEntriesCommand, ILogEntryListViewModel>();

            PropertyChanged += OnPropertyChanged;
        }

        private void OnLogEntriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                FilterSelectionControlModel.AddNamespaces(e.NewItems
                    .OfType<PLogEntry>()
                    .Select(l => l.LoggerText));
            }
        }

        public void Close()
        {
            if (Receiver.State == ReceiverStateType.Running)
            {
                Receiver.Stop();
            }

            if (LogEntryListViewClosed != null)
            {
                LogEntryListViewClosed(this);
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == this.GetPropertyName(v => v.IsDebugEnabled) ||
                e.PropertyName == this.GetPropertyName(v => v.IsInformationEnabled) ||
                e.PropertyName == this.GetPropertyName(v => v.IsWarningEnabled) ||
                e.PropertyName == this.GetPropertyName(v => v.IsErrorEnabled) ||
                e.PropertyName == this.GetPropertyName(v => v.IsFatalEnabled))
            {
                LogEntriesViewSource.View.Refresh();
            }
            else if (e.PropertyName == this.GetPropertyName(v => v.RegexExpression))
            {
                IsRegexExpressionValid = IsRegexValid(RegexExpression);
                if (IsRegexExpressionValid)
                {
                    _regex = new Regex(RegexExpression,
                        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                }
                else
                {
                    _regex = null;
                }
                LogEntriesViewSource.View.Refresh();
            }
            else if (e.PropertyName == this.GetPropertyName(v => v.IsRegexExpressionValid))
            {
                RegexExpressionBackground = IsRegexExpressionValid
                    ? Brushes.White
                    : Brushes.IndianRed;
            }
            else if (e.PropertyName == this.GetPropertyName(v => v.Receiver))
            {
                AddDisposable(Receiver.LogEntryStream.Subscribe(OnReceiverLogEntriesReceived));
                Receiver.PropertyChanged += OnReceiverPropertyChanged;
                IsActive = Receiver.State == ReceiverStateType.Running;
            }
        }

        private void OnReceiverPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Receiver.GetPropertyName(r => r.State))
            {
                IsActive = Receiver.State == ReceiverStateType.Running;
            }
        }

        private void OnReceiverLogEntriesReceived(ICollection<CLogEntry> logEntries)
        {
            var mapped = logEntries.Select(le => _coreToUi.ToPLogEntry(le)).ToList();
            _dispatcherHelper.DispatchToUiThread(() =>
                {
                    if (mapped.Count > _configuration.Get(c => c.MaxNumberOfLogEntries))
                    {
                        LogEntries.Clear();
                        mapped = mapped
                            .OrderByDescending(le => le.ReceivedTime)
                            .Take(_configuration.Get(c => c.MaxNumberOfLogEntries))
                            .ToList();
                    }
                    else if (LogEntries.Count + mapped.Count > _configuration.Get(c => c.MaxNumberOfLogEntries))
                    {
                        var toRemoveCount = (LogEntries.Count + mapped.Count) - _configuration.Get(c => c.MaxNumberOfLogEntries);
                        var toRemove = LogEntries
                            .OrderBy(le => le.ReceivedTime)
                            .Take(toRemoveCount)
                            .ToList();
                        toRemove.ForEach(le => LogEntries.Remove(le));
                    }
                    mapped.ForEach(le => LogEntries.Add(le));
                });
        }

        public void LogEntriesViewSourceFilter(object sender, FilterEventArgs e)
        {
            var pLogEntry = e.Item as PLogEntry;
            if (null == pLogEntry)
            {
                e.Accepted = false;
                return;
            }

            if (null != _regex && !_regex.IsMatch(pLogEntry.MessageText))
            {
                e.Accepted = false;
                return;
            }

            if (!FilterSelectionControlModel.IsAccepted(pLogEntry))
            {
                e.Accepted = false;
                return;
            }

            switch (pLogEntry.Level)
            {
                case LogEntryLevelType.None:
                    e.Accepted = true;
                    break;
                case LogEntryLevelType.Debug:
                    e.Accepted = IsDebugEnabled;
                    break;
                case LogEntryLevelType.Information:
                    e.Accepted = IsInformationEnabled;
                    break;
                case LogEntryLevelType.Warning:
                    e.Accepted = IsWarningEnabled;
                    break;
                case LogEntryLevelType.Error:
                    e.Accepted = IsErrorEnabled;
                    break;
                case LogEntryLevelType.Fatal:
                    e.Accepted = IsFatalEnabled;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static bool IsRegexValid(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return true;
            }

            try
            {
                new Regex(str, RegexOptions.Compiled);
            }
            catch (ArgumentException)
            {
                return false;
            }
            return true;
        }


    }
}
