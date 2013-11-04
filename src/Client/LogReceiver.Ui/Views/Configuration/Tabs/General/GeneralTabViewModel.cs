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
using System.ComponentModel;
using System.Linq;
using Blocks.Core.Configuration;
using Blocks.Mvvm.ViewModels;
using LogReceiver.Core;
using LogReceiver.Ui.Views.Configuration.Tabs.General.Commands;
using Blocks.Core.Extensions;
using LogReceiver.Ui.Views.Configuration.Tabs.General.DOM;

namespace LogReceiver.Ui.Views.Configuration.Tabs.General
{
    public class GeneralTabViewModel : SpecificChildViewModelBase<IConfigurationViewModel>, IGeneralTabViewModel
    {
        private readonly IUserConfiguration<ILogReceiverUserSettings> _configuration;

        public ISaveSettingsCommand SaveSettingsCommand { get; private set; }
        public IResetSettingsCommand ResetSettingsCommand { get; private set; }

        public IEnumerable<int> AllMaxNumberOfLogEntries { get; private set; }

        public IReadOnlyCollection<PLogFetchTimeSpanType> AllLogFetchTimeSpans { get; private set; }

        #region Property - SelectedLogFetchTimeSpan
        private PLogFetchTimeSpanType _internalSelectedLogFetchTimeSpan;
        public PLogFetchTimeSpanType SelectedLogFetchTimeSpan
        {
            get { return _internalSelectedLogFetchTimeSpan; }
            set
            {
                if (_internalSelectedLogFetchTimeSpan == value)
                {
                    return;
                }
                _internalSelectedLogFetchTimeSpan = value;
                RaisePropertyChanged(() => SelectedLogFetchTimeSpan);
            }
        }
        #endregion

        #region Property - WordWrap
        private bool _wordWrap;
        public bool WordWrap
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

        #region Property - ParseCSharpStackTrace
        private bool _parseCSharpStackTrace;
        public bool ParseCSharpStackTrace
        {
            get { return _parseCSharpStackTrace; }
            set
            {
                if (_parseCSharpStackTrace == value)
                {
                    return;
                }
                _parseCSharpStackTrace = value;
                RaisePropertyChanged(() => ParseCSharpStackTrace);
            }
        }
        #endregion

        #region Property - MaxNumberOfLogEntries
        private int _maxNumberOfLogEntries;
        public int MaxNumberOfLogEntries
        {
            get { return _maxNumberOfLogEntries; }
            set
            {
                if (_maxNumberOfLogEntries == value)
                {
                    return;
                }
                _maxNumberOfLogEntries = value;
                RaisePropertyChanged(() => MaxNumberOfLogEntries);
            }
        }
        #endregion

        #region Property - IsDirty
        private bool _isDirty;
        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (_isDirty == value)
                {
                    return;
                }
                _isDirty = value;
                RaisePropertyChanged(() => IsDirty);
            }
        }
        #endregion

        public GeneralTabViewModel(
            IUserConfiguration<ILogReceiverUserSettings> configuration)
        {
            _configuration = configuration;

            SaveSettingsCommand = CreateAndBindChild<ISaveSettingsCommand, IGeneralTabViewModel>();
            ResetSettingsCommand = CreateAndBindChild<IResetSettingsCommand, IGeneralTabViewModel>();

            AllMaxNumberOfLogEntries = new List<int>{ 5, 100, 500, 1000, 5000, 10000 };
            AllLogFetchTimeSpans = PLogFetchTimeSpanType.GetAll();

            PropertyChanged += OnPropertyChanged;

            LoadSettings();
        }

        public void LoadSettings()
        {
            WordWrap = _configuration.Get(c => c.WordWrap);
            ParseCSharpStackTrace = _configuration.Get(c => c.ParseCSharpStackTrace);
            MaxNumberOfLogEntries = _configuration.Get(c => c.MaxNumberOfLogEntries);

            SelectedLogFetchTimeSpan = AllLogFetchTimeSpans.Single(ts => ts.Value == _configuration.Get(c => c.LogFetchTimeSpan));

            IsDirty = false;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != this.GetPropertyName(v => v.IsDirty) &&
                e.PropertyName != this.GetPropertyName(v => v.ParentViewModel))
            {
                IsDirty = true;
            }
        }
    }
}
