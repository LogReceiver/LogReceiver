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
using Microsoft.Practices.ServiceLocation;

namespace LogReceiver.Core.Receiving.Receivers.LogViewer
{
    public class LogViewerReceiverInitializer : ILogViewerReceiverInitializer
    {
        public string Name { get { return "Legacy LogViewer receiver"; } }
        public bool CanSpawn { get { return true; } }
        public Guid Id { get { return Guid.Parse("D2927F9E-F644-4F1F-A35F-529365B551F1"); } }

        public IReceiver Spawn()
        {
            return ServiceLocator.Current.GetInstance<ILogViewerReceiver>();
        }
    }
}
