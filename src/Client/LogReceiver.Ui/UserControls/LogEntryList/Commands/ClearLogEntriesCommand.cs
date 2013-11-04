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
using Blocks.Mvvm.Commands;
using Blocks.Core.Extensions;

namespace LogReceiver.Ui.UserControls.LogEntryList.Commands
{
    public class ClearLogEntriesCommand : ViewModelCommandBase<ILogEntryListViewModel>, IClearLogEntriesCommand
    {
        public override void Execute(object parameter)
        {
            ParentViewModel.LogEntries.Clear();
        }

        protected override void OnParentViewModelChanged()
        {
            if (null == ParentViewModel)
            {
                return;
            }

            CanCommandExecute = ParentViewModel.LogEntries.Count != 0;
            ParentViewModel.LogEntries.CollectionChanged += (s, e) => CanCommandExecute = ParentViewModel.LogEntries.Count != 0;
        }

        protected override void OnParentViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnParentViewModelPropertyChanged(sender, e);

            if (e.PropertyName == ParentViewModel.GetPropertyName(v => v.LogEntries))
            {
                CanCommandExecute = ParentViewModel.LogEntries.Count != 0;
                ParentViewModel.LogEntries.CollectionChanged += (s, ce) => CanCommandExecute = ParentViewModel.LogEntries.Count != 0;
            }
        }
    }
}
