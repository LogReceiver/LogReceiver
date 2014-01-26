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
using Blocks.Core.Configuration;
using Blocks.Mvvm.Commands;
using LogReceiver.Core;
using Blocks.Core.Extensions;

namespace LogReceiver.Ui.Views.Configuration.Tabs.General.Commands
{
    public class SaveSettingsCommand : ViewModelCommandBase<IGeneralTabViewModel>, ISaveSettingsCommand
    {
        private readonly IUserConfiguration<ILogReceiverUserSettings> _configuration;

        public SaveSettingsCommand(IUserConfiguration<ILogReceiverUserSettings> configuration)
        {
            _configuration = configuration;
            CanCommandExecute = false;
        }

        public override void Execute(object parameter)
        {
            if (!CanCommandExecute)
            {
                return;
            }

            _configuration.Set(c => c.WordWrap, ParentViewModel.WordWrap);
            _configuration.Set(c => c.ParseCSharpStackTrace, ParentViewModel.ParseCSharpStackTrace);
            _configuration.Set(c => c.MaxNumberOfLogEntries, ParentViewModel.MaxNumberOfLogEntries);

            ParentViewModel.IsDirty = false;
        }

        protected override void OnParentViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnParentViewModelPropertyChanged(sender, e);

            if (e.PropertyName == ParentViewModel.GetPropertyName(v => v.IsDirty))
            {
                CanCommandExecute = ParentViewModel.IsDirty;
            }
        }
    }
}
