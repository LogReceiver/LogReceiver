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
using System.Windows.Controls;
using LogReceiver.Ui.UserControls.LogEntryList;

namespace LogReceiver.Ui.Views.Main
{
    public partial class MainView : IMainView
    {
        public MainView()
        {
            InitializeComponent();
            DataContextChanged += OnMyDataContextChanged;
        }

        private void OnMyDataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (!(DataContext is IMainViewModel))
            {
                return;
            }

            var viewModel = (IMainViewModel) DataContext;
            viewModel.ReceiverSpawned += (h, d) =>
                {
                    var tabItem = new TabItem
                        {
                            Header = h,
                            DataContext = d,
                            Content = new LogEntryListControl(),
                        };
                    TabControl.Items.Add(tabItem);
                    ((ILogEntryListViewModel) d).LogEntryListViewClosed += s =>
                        {
                            TabControl.Items.Remove(tabItem);
                            var disposableDataContext = tabItem.DataContext as IDisposable;
                            if (disposableDataContext != null)
                            {
                                disposableDataContext.Dispose();
                            }
                            TabControl.SelectedItem = TabControl.Items.Count > 0
                                ? TabControl.Items[0]
                                : null;
                        };
                    if (TabControl.Items.Count == 1)
                    {
                        TabControl.SelectedItem = TabControl.Items[0];
                    }
                };
            viewModel.ViewReady();
        }
    }
}
