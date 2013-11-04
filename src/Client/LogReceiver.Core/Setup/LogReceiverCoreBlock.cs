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
using Blocks.Core.Configuration.Providers;
using Blocks.Core.Services;
using Blocks.Core.Specifications;
using LogReceiver.Core.Receiving;
using LogReceiver.Core.Receiving.Receivers.LogViewer;
using LogReceiver.Core.StackTraces;

namespace LogReceiver.Core.Setup
{
    public class LogReceiverCoreBlock : BlockBase, ILogReceiverCoreBlock
    {
        private static readonly IEnumerable<BlockSpecification> _requirements = new[]
            {
                new BlockSpecification{ PlugIn = typeof(ICoreBlock) },
            };
        public override IEnumerable<BlockSpecification> Requirements { get { return _requirements; } }

        public override void BeforeInitialization(IServiceRegister register)
        {
            base.BeforeInitialization(register);
            register.AddService<IUserSettingsProvider, LogReceiverSettingsWrapper>(ServiceRegistrationType.Singleton);
        }

        public override void Initialize(IServiceRegister register)
        {
            FireBlockLoadingTextUpdatedEvent("Loading LogReciever core");

            // Receiver - LogViewer
            register.AddService<ILogViewerReceiver, LogViewerReceiver>(ServiceRegistrationType.AlwaysUnique);
            register.AddService<IReceiverInitializer, LogViewerReceiverInitializer>(ServiceRegistrationType.AlwaysUnique);

            register.AddService<IReceiverManager, ReceiverManager>(ServiceRegistrationType.Singleton);

            register.AddService<ICSharpStackTraceParser, CSharpStackTraceParser>(ServiceRegistrationType.AlwaysUnique);
        }
    }
}
