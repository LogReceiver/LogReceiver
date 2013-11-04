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
using Blocks.Core;
using Blocks.Core.Blocks;
using Blocks.Core.Services;
using Blocks.Core.Specifications;
using Blocks.Mvvm;
using Blocks.Mvvm.Views.Defaults.Master;
using LogReceiver.Ui.Models.Mapping;
using LogReceiver.Ui.UserControls.LogEntryList;
using LogReceiver.Ui.UserControls.LogEntryList.Commands;
using LogReceiver.Ui.UserControls.LogEntryList.UserControls.FilterSelection;
using LogReceiver.Ui.UserControls.LogEntryList.UserControls.FilterSelection.Commands;
using LogReceiver.Ui.UserControls.LogEntryList.UserControls.FilterSelection.DOM.Mapping;
using LogReceiver.Ui.Views.About;
using LogReceiver.Ui.Views.Configuration;
using LogReceiver.Ui.Views.Configuration.Tabs.General;
using LogReceiver.Ui.Views.Configuration.Tabs.General.Commands;
using LogReceiver.Ui.Views.Configuration.Tabs.Receivers;
using LogReceiver.Ui.Views.Configuration.Tabs.Receivers.Commands;
using LogReceiver.Ui.Views.LogEntryDetails;
using LogReceiver.Ui.Views.LogEntryDetails.Commands;
using LogReceiver.Ui.Views.Main;
using LogReceiver.Ui.Views.Main.Commands;

namespace LogReceiver.Ui.Setup
{
    public class LogReceiverUiBlock : BlockBase, ILogReceiverUiBlock
    {
        private static readonly IEnumerable<BlockSpecification> _requirements = new[]
            {
                new BlockSpecification{ PlugIn = typeof(ICoreBlock) },
                new BlockSpecification{ PlugIn = typeof(IMvvmBlock) },
            };
        public override IEnumerable<BlockSpecification> Requirements { get { return _requirements; } }

        public override void Initialize(IServiceRegister register)
        {
            FireBlockLoadingTextUpdatedEvent("Loading LogReceiver UI");

            register.AddService<IUiMapper, UiMapper>(ServiceRegistrationType.Singleton);

            // UserControls - FilterSelection
            register.AddService<IFilterSelectionControlModel, FilterSelectionControlModel>(ServiceRegistrationType.AlwaysUnique);
            register.AddService<ISelectAllFiltersCommand, SelectAllFiltersCommand>(ServiceRegistrationType.AlwaysUnique);
            register.AddService<ISaveFilterProfileCommand, SaveFilterProfileCommand>(ServiceRegistrationType.AlwaysUnique);
            register.AddService<ILoadFilterProfileCommand, LoadFilterProfileCommand>(ServiceRegistrationType.AlwaysUnique);
            register.AddService<IFilterConfigurationMapper, FilterConfigurationMapper>(ServiceRegistrationType.Singleton);
            register.AddService<ICreateFilterProfileCommand, CreateFilterProfileCommand>(ServiceRegistrationType.AlwaysUnique);
            register.AddService<IEditFilterProfileCommand, EditFilterProfileCommand>(ServiceRegistrationType.AlwaysUnique);
            register.AddService<IDeleteFilterProfileCommand, DeleteFilterProfileCommand>(ServiceRegistrationType.AlwaysUnique);

            // UserControls - LogEntryList
            register.AddService<ILogEntryListViewModel, LogEntryListViewModel>(ServiceRegistrationType.AlwaysUnique);
            register.AddService<IShowLogEntryDetailsCommand, ShowLogEntryDetailsCommand>(ServiceRegistrationType.AlwaysUnique);
            register.AddService<IDeselectFilterCommand, DeselectFilterCommand>(ServiceRegistrationType.AlwaysUnique);
            register.AddService<IDeselectLogEntryCommand, DeselectLogEntryCommand>(ServiceRegistrationType.AlwaysUnique);

            // Main view
            register.AddService<IMasterView, MainView>(ServiceRegistrationType.AlwaysUnique);
            register.AddService<IMasterViewModel, MainViewModel>(ServiceRegistrationType.AlwaysUnique);
            register.AddService<ITestSendLogEventCommand, TestSendLogEventCommand>(ServiceRegistrationType.AlwaysUnique);
            register.AddService<IClearLogEntriesCommand, ClearLogEntriesCommand>(ServiceRegistrationType.AlwaysUnique);
            register.AddService<IShowAboutCommand, ShowAboutCommand>(ServiceRegistrationType.AlwaysUnique);
            register.AddService<IExportLogEntriesCommand, ExportLogEntriesCommand>(ServiceRegistrationType.AlwaysUnique);
            register.AddService<IShowSettingsCommand, ShowSettingsCommand>(ServiceRegistrationType.AlwaysUnique);
            register.AddService<ICloseReceiverTabCommand, CloseReceiverTabCommand>(ServiceRegistrationType.AlwaysUnique);
            register.AddService<IOpenFeaturesLinkCommand, OpenFeaturesLinkCommand>(ServiceRegistrationType.AlwaysUnique);

            // View - LogEntryDetails
            register.AddService<ILogEntryDetailsView, LogEntryDetailsView>(ServiceRegistrationType.AlwaysUnique);
            register.AddService<ILogEntryDetailsViewModel, LogEntryDetailsViewModel>(ServiceRegistrationType.AlwaysUnique);
            register.AddService<ICopyFilePathCommand, CopyFilePathCommand>(ServiceRegistrationType.AlwaysUnique);

            // View - About
            register.AddService<IAboutView, AboutView>(ServiceRegistrationType.AlwaysUnique);
            register.AddService<IAboutViewModel, AboutViewModel>(ServiceRegistrationType.AlwaysUnique);

            // View - Settings
            register.AddService<IConfigurationView, ConfigurationView>(ServiceRegistrationType.AlwaysUnique);
            register.AddService<IConfigurationViewModel, ConfigurationViewModel>(ServiceRegistrationType.AlwaysUnique);
            register.AddService<ISaveSettingsCommand, SaveSettingsCommand>(ServiceRegistrationType.AlwaysUnique);
            register.AddService<IResetSettingsCommand, ResetSettingsCommand>(ServiceRegistrationType.AlwaysUnique);
            // View - Settings - GeneralTab
            register.AddService<IGeneralTabViewModel, GeneralTabViewModel>(ServiceRegistrationType.AlwaysUnique);
            // View - Settings - ReceiversTab
            register.AddService<IReceiversTabViewModel, ReceiversTabViewModel>(ServiceRegistrationType.AlwaysUnique);
            register.AddService<ISpawnReceiverCommand, SpawnReceiverCommand>(ServiceRegistrationType.AlwaysUnique);
        }
    }
}
