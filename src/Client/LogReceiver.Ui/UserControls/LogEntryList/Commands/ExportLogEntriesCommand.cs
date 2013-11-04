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
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Blocks.Mvvm.Commands;
using Blocks.Mvvm.Services;
using LogReceiver.Ui.Models.Export;
using LogReceiver.Ui.Models.Mapping;

namespace LogReceiver.Ui.UserControls.LogEntryList.Commands
{
    public class ExportLogEntriesCommand : ViewModelCommandBase<ILogEntryListViewModel>, IExportLogEntriesCommand
    {
        private readonly IDialogService _dialogService;
        private readonly IUiMapper _uiMapper;

        public ExportLogEntriesCommand(
            IDialogService dialogService,
            IUiMapper uiMapper)
        {
            _dialogService = dialogService;
            _uiMapper = uiMapper;
        }

        public override void Execute(object parameter)
        {
            var path = _dialogService.SaveFile(
                ResidingWindowViewViewModel,
                "xml");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            var eLogEntries = ParentViewModel.LogEntries
                .Select(l => _uiMapper.ToELogEntry(l))
                .ToList();
            var eLogEntriesCollection = new ELogEntryCollection
                {
                    LogEntries = eLogEntries,
                };

            try
            {
                using (var file = File.OpenWrite(path))
                {
                    var se = new XmlSerializer(typeof (ELogEntryCollection));
                    se.Serialize(file, eLogEntriesCollection);
                }
            }
            catch (Exception exception)
            {
                Logger.ErrorFormat(exception, "Could not save file to '{0}'", path);
                _dialogService.ShowError(
                    ResidingWindowViewViewModel,
                    "Export log entries",
                    string.Format("Could not export log entries due to: {0}", exception.Message));
            }
        }
    }
}
