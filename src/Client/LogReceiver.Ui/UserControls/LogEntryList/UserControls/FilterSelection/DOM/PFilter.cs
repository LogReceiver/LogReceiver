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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Blocks.Core.ObservableObjects;
using Blocks.Core.Extensions;

namespace LogReceiver.Ui.UserControls.LogEntryList.UserControls.FilterSelection.DOM
{
    public class PFilter : ObservableObjectBase
    {
        #region Property - Name
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value)
                {
                    return;
                }
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }
        #endregion

        #region Property - IsSelected
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected == value)
                {
                    return;
                }
                _isSelected = value;
                RaisePropertyChanged(() => IsSelected);
            }
        }
        #endregion

        #region Property - IsExpanded
        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (_isExpanded == value)
                {
                    return;
                }
                _isExpanded = value;
                RaisePropertyChanged(() => IsExpanded);
            }
        }
        #endregion

        #region Property - Children
        private ObservableCollection<PFilter> _children;
        public ObservableCollection<PFilter> Children
        {
            get { return _children; }
            set
            {
                if (_children == value)
                {
                    return;
                }
                _children = value;
                RaisePropertyChanged(() => Children);
            }
        }
        #endregion

        #region Property - LastPathPart
        private string _lastPathPart;
        public string LastPathPart
        {
            get { return _lastPathPart; }
            set
            {
                if (_lastPathPart == value)
                {
                    return;
                }
                _lastPathPart = value;
                RaisePropertyChanged(() => LastPathPart);
            }
        }
        #endregion

        public List<string> NamePath { get; set; }

        public PFilter()
        {
            IsSelected = true;
            Children = new ObservableCollection<PFilter>();
            NamePath = new List<string> { };
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == this.GetPropertyName(f => f.Name))
            {
                if (string.IsNullOrEmpty(Name))
                {
                    NamePath = new List<string>();
                    LastPathPart = string.Empty;
                }
                else
                {
                    NamePath = Name.Split('.').ToList();
                    LastPathPart = NamePath.Last();
                }
            }
            if (e.PropertyName == this.GetPropertyName(f => f.IsSelected))
            {
                Children.ToList().ForEach(c => c.IsSelected = IsSelected);
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
