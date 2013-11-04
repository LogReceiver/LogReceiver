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

using System.Windows;
using Blocks.Mvvm.ViewModels;
using LogReceiver.Core.Receiving;
using LogReceiver.Ui.Models;
using LogReceiver.Ui.Views.LogEntryDetails.Commands;

namespace LogReceiver.Ui.Views.LogEntryDetails
{
    public interface ILogEntryDetailsViewModel : IWindowViewViewModel
    {
        ICopyFilePathCommand CopyFilePathCommand { get; }

        PMethodCall SelectedMethodCall { get; set; }

        string Title { get; }
        PLogEntry LogEntry { get; set; }
        IReceiver Receiver { get; set; }
        PStackTrace StackTrace { get; }
        TextWrapping WordWrap { get; }
        bool IsStackTraceVisible { get; }
    }
}
