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

using Blocks.Core.Resources;
using Blocks.Core.Utilities;
using Blocks.Mvvm.ViewModels;

namespace LogReceiver.Ui.Views.About
{
    public class AboutViewModel : WindowViewViewModelBase, IAboutViewModel
    {
        public string Text { get; private set; }
        public string BuildTimeText { get; private set; }

        public AboutViewModel(
            IResourceManager resourceManager,
            IReflectionHelper reflectionHelper)
        {
            Text = resourceManager.DefaultStore.GetExistingResource("LogReceiver.Ui;logreceiver_changelog.txt").ReadAsString();
            BuildTimeText = reflectionHelper.GetCompileTime(this.GetType().Assembly).ToString("yyyy-MM-dd HH:mm");
        }
    }
}
