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
using Blocks.Mvvm.ViewModels;
using LogReceiver.Ui.Models;
using LogReceiver.Ui.UserControls.LogEntryList.UserControls.FilterSelection.Commands;
using LogReceiver.Ui.UserControls.LogEntryList.UserControls.FilterSelection.DOM;

namespace LogReceiver.Ui.UserControls.LogEntryList.UserControls.FilterSelection
{
    public class FilterSelectionControlModel : SpecificChildViewModelBase<ILogEntryListViewModel>, IFilterSelectionControlModel
    {
        public event FilterSelectionChangedDelegate FilterSelectionChanged;

        public ISelectAllFiltersCommand SelectAllFiltersCommand { get; private set; }
        public ISaveFilterProfileCommand SaveFilterProfileCommand { get; private set; }
        public ILoadFilterProfileCommand LoadFilterProfileCommand { get; private set; }
        public ICreateFilterProfileCommand CreateFilterProfileCommand { get; private set; }
        public IEditFilterProfileCommand EditFilterProfileCommand { get; private set; }
        public IDeleteFilterProfileCommand DeleteFilterProfileCommand { get; private set; }

        #region Property - AllFilterProfiles
        private ObservableCollection<PFilterProfile> _allFilterProfiles;
        public ObservableCollection<PFilterProfile> AllFilterProfiles
        {
            get { return _allFilterProfiles; }
            set
            {
                if (_allFilterProfiles == value)
                {
                    return;
                }
                _allFilterProfiles = value;
                RaisePropertyChanged(() => AllFilterProfiles);
            }
        }
        #endregion

        #region Property - SelectedFilterProfile
        private PFilterProfile _selectedFilterProfile;
        public PFilterProfile SelectedFilterProfile
        {
            get { return _selectedFilterProfile; }
            set
            {
                if (_selectedFilterProfile == value)
                {
                    return;
                }
                _selectedFilterProfile = value;
                RaisePropertyChanged(() => SelectedFilterProfile);
            }
        }
        #endregion

        public FilterSelectionControlModel()
        {
            AllFilterProfiles = new ObservableCollection<PFilterProfile>(new[]{ new PFilterProfile
                {
                    Name = "Default",
                }});
            SelectedFilterProfile = AllFilterProfiles.First();
            SelectedFilterProfile.FilterSelectionChanged += SelectedFilterProfileOnFilterSelectionChanged;

            SelectAllFiltersCommand = CreateAndBindChild<ISelectAllFiltersCommand, IFilterSelectionControlModel>();
            SaveFilterProfileCommand = CreateAndBindChild<ISaveFilterProfileCommand, IFilterSelectionControlModel>();
            LoadFilterProfileCommand = CreateAndBindChild<ILoadFilterProfileCommand, IFilterSelectionControlModel>();
            CreateFilterProfileCommand = CreateAndBindChild<ICreateFilterProfileCommand, IFilterSelectionControlModel>();
            EditFilterProfileCommand = CreateAndBindChild<IEditFilterProfileCommand, IFilterSelectionControlModel>();
            DeleteFilterProfileCommand = CreateAndBindChild<IDeleteFilterProfileCommand, IFilterSelectionControlModel>();
        }

        private void SelectedFilterProfileOnFilterSelectionChanged(PFilterProfile sender)
        {
            if (sender == SelectedFilterProfile && FilterSelectionChanged != null)
            {
                FilterSelectionChanged(sender);
            }
        }

        public bool IsAccepted(PLogEntry pLogEntry)
        {
            return SelectedFilterProfile.IsAccepted(pLogEntry);
        }

        public void AddNamespaces(IEnumerable<string> namespaces)
        {
            SelectedFilterProfile.AddNamespaces(namespaces);
        }

        public void DeselectFilter(string name)
        {
            SelectedFilterProfile.DeselectFilter(name);
        }
    }
}
