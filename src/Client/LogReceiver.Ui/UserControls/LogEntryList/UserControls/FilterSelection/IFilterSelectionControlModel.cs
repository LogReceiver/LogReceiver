﻿﻿/* -----------------------------------------------------------------------------
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
using Blocks.Mvvm.ViewModels;
using LogReceiver.Ui.Models;
using LogReceiver.Ui.UserControls.LogEntryList.UserControls.FilterSelection.Commands;
using LogReceiver.Ui.UserControls.LogEntryList.UserControls.FilterSelection.DOM;

namespace LogReceiver.Ui.UserControls.LogEntryList.UserControls.FilterSelection
{
    public delegate void FilterSelectionChangedDelegate(PFilterProfile sender);

    public interface IFilterSelectionControlModel : IViewModel, ISpecificViewModelChild<ILogEntryListViewModel>
    {
        event FilterSelectionChangedDelegate FilterSelectionChanged;

        ISelectAllFiltersCommand SelectAllFiltersCommand { get; }
        ISaveFilterProfileCommand SaveFilterProfileCommand { get; }
        ILoadFilterProfileCommand LoadFilterProfileCommand { get; }
        ICreateFilterProfileCommand CreateFilterProfileCommand { get; }
        IEditFilterProfileCommand EditFilterProfileCommand { get; }
        IDeleteFilterProfileCommand DeleteFilterProfileCommand { get; }

        ObservableCollection<PFilterProfile> AllFilterProfiles { get; set; }
        PFilterProfile SelectedFilterProfile { get; set; }

        bool IsAccepted(PLogEntry pLogEntry);
        void AddNamespaces(IEnumerable<string> namespaces);
        void DeselectFilter(string name);
    }
}
