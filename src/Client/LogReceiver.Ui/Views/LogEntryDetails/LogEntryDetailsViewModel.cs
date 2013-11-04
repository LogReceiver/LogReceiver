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
using System.Globalization;
using System.Windows;
using Blocks.Core.Configuration;
using Blocks.Core.Messaging.Messages;
using Blocks.Core.Units;
using Blocks.Mvvm.ViewModels;
using LogReceiver.Core;
using LogReceiver.Core.Receiving;
using LogReceiver.Core.StackTraces;
using LogReceiver.Ui.Models;
using LogReceiver.Ui.Models.Mapping;
using LogReceiver.Ui.Views.LogEntryDetails.Commands;
using Blocks.Core.Extensions;

namespace LogReceiver.Ui.Views.LogEntryDetails
{
    public class LogEntryDetailsViewModel : WindowViewViewModelBase, ILogEntryDetailsViewModel
    {
        private readonly IUserConfiguration<ILogReceiverUserSettings> _configuration;
        private readonly IUiMapper _mapper;
        private readonly ICSharpStackTraceParser _cSharpStackTraceParser;

        public ICopyFilePathCommand CopyFilePathCommand { get; private set; }
        public IReceiver Receiver { get; set; }

        #region Property - Details
        private IEnumerable<Tuple<string,string>>  _internalDetails;
        public IEnumerable<Tuple<string,string>>  Details
        {
            get { return _internalDetails; }
            set
            {
                if (_internalDetails == value)
                {
                    return;
                }
                _internalDetails = value;
                RaisePropertyChanged(() => Details);
            }
        }
        #endregion

        #region Property - SelectedMethodCall
        private PMethodCall _selectedMethodCall;
        public PMethodCall SelectedMethodCall
        {
            get { return _selectedMethodCall; }
            set
            {
                if (_selectedMethodCall == value)
                {
                    return;
                }
                _selectedMethodCall = value;
                RaisePropertyChanged(() => SelectedMethodCall);
            }
        }
        #endregion

        #region Property - Title
        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title == value)
                {
                    return;
                }
                _title = value;
                RaisePropertyChanged(() => Title);
            }
        }
        #endregion

        #region Property - LogEntry
        private PLogEntry _logEntry;
        public PLogEntry LogEntry
        {
            get { return _logEntry; }
            set
            {
                if (_logEntry == value)
                {
                    return;
                }
                _logEntry = value;
                RaisePropertyChanged(() => LogEntry);
            }
        }
        #endregion

        #region Property - WordWrap
        private TextWrapping _wordWrap;
        public TextWrapping WordWrap
        {
            get { return _wordWrap; }
            set
            {
                if (_wordWrap == value)
                {
                    return;
                }
                _wordWrap = value;
                RaisePropertyChanged(() => WordWrap);
            }
        }
        #endregion

        #region Property - StackTrace
        private PStackTrace _stackTrace;
        public PStackTrace StackTrace
        {
            get { return _stackTrace; }
            set
            {
                if (_stackTrace == value)
                {
                    return;
                }
                _stackTrace = value;
                RaisePropertyChanged(() => StackTrace);
            }
        }
        #endregion

        #region Property - IsStackTraceVisible
        private bool _isStackTraceVisible;
        public bool IsStackTraceVisible
        {
            get { return _isStackTraceVisible; }
            set
            {
                if (_isStackTraceVisible == value)
                {
                    return;
                }
                _isStackTraceVisible = value;
                RaisePropertyChanged(() => IsStackTraceVisible);
            }
        }
        #endregion

        public LogEntryDetailsViewModel(
            IUserConfiguration<ILogReceiverUserSettings> configuration,
            IUiMapper mapper,
            ICSharpStackTraceParser cSharpStackTraceParser)
        {
            _configuration = configuration;
            _mapper = mapper;
            _cSharpStackTraceParser = cSharpStackTraceParser;

            CopyFilePathCommand = CreateAndBindChild<ICopyFilePathCommand, ILogEntryDetailsViewModel>();

            PropertyChanged += OnPropertyChanged;
            Messenger.Register<UserConfigurationChangedMessage<ILogReceiverUserSettings>>(this, m =>
                {
                    UpdateSettings();
                    UpdateStackTrace();
                });

            UpdateSettings();
        }

        private void UpdateSettings()
        {
            WordWrap = _configuration.Get(c => c.WordWrap)
                ? TextWrapping.WrapWithOverflow
                : TextWrapping.NoWrap;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == this.GetPropertyName(t => t.LogEntry))
            {
                Title = string.Format("{0} - {1} - {2}",
                    LogEntry.LoggerText,
                    LogEntry.Time.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    LogEntry.Level);
                UpdateStackTrace();
                UpdateDetails();
            }
        }

        private void UpdateDetails()
        {
            var details = new List<Tuple<string, string>>
                {
                    new Tuple<string, string>("ID", LogEntry.Id.ToString(CultureInfo.InvariantCulture)),
                    new Tuple<string, string>("Logger", LogEntry.LoggerText),
                    new Tuple<string, string>("Time", LogEntry.Time.ToString("yyyy-MM-dd HH:mm:ss,fff")),
                    new Tuple<string, string>("Level", LogEntry.Level.ToString()),
                    new Tuple<string, string>("Thread", LogEntry.ThreadText),
                };

            Details = details;
        }

        private void UpdateStackTrace()
        {
            if (!_configuration.Get(c => c.ParseCSharpStackTrace))
            {
                return;
            }

            try
            {
                StackTrace = _mapper.ToPStackTrace(_cSharpStackTraceParser.Parse(LogEntry.MessageText));
                IsStackTraceVisible = StackTrace.MethodCalls.Count > 0;
            }
            catch (Exception) { }
        }
    }
}
