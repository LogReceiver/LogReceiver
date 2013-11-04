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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Blocks.Core.Extensions;
using Blocks.Core.ObservableObjects;
using LogReceiver.Ui.Models;

namespace LogReceiver.Ui.UserControls.LogEntryList.UserControls.FilterSelection.DOM
{
    public class PFilterProfile : ObservableObjectBase
    {
        public event FilterSelectionChangedDelegate FilterSelectionChanged;

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

        #region Property - Filters
        private ObservableCollection<PFilter> _filters;
        public ObservableCollection<PFilter> Filters
        {
            get { return _filters; }
            set
            {
                if (_filters == value)
                {
                    return;
                }
                _filters = value;
                RaisePropertyChanged(() => Filters);
            }
        }
        #endregion

        public PFilterProfile()
        {
            PropertyChanged += OnPropertyChanged;
            Filters = new ObservableCollection<PFilter>();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == this.GetPropertyName(v => v.Filters))
            {
                RegisterOnFilterPropertyChanged(Filters);
            }
        }

        private void RegisterOnFilterPropertyChanged(IEnumerable<PFilter> filters)
        {
            foreach (var filter in filters)
            {
                filter.PropertyChanged += OnFilterPropertyChanged;
                RegisterOnFilterPropertyChanged(filter.Children);
            }
        }

        public void AddNamespaces(IEnumerable<string> namespaces)
        {
            lock (Filters)
            {
                foreach (var pFilter in namespaces.Where(ns => !string.IsNullOrEmpty(ns)).Select(ns => new PFilter
                    {
                        Name = ns,
                    }))
                {
                    FindOrCreateParentFilter(pFilter, Filters, new List<string>(), true);
                }
            }
        }

        public void DeselectFilter(string name)
        {
            ApplyActionToFilter(name, pFilter => pFilter.IsSelected = false);
        }

        private void ApplyActionToFilter(string filterName, Action<PFilter> action)
        {
            ApplyActionToFilters(Filters, pFilter =>
                {
                    if (!string.Equals(filterName, pFilter.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                    action(pFilter);
                    return false;
                });
        }

        private static bool ApplyActionToFilters(IEnumerable<PFilter> filters, Func<PFilter, bool> action)
        {
            foreach (var pFilter in filters)
            {
                var doContinue = action(pFilter);
                if (!doContinue)
                {
                    return false;
                }
                doContinue = ApplyActionToFilters(pFilter.Children, action);
                if (!doContinue)
                {
                    return false;
                }
            }
            return true;
        }

        public void FindOrCreateParentFilter(PFilter pFilter, ICollection<PFilter> filters, List<string> currentNamePath, bool isParentSelected)
        {
            if (IsParentPath(currentNamePath, pFilter.NamePath))
            {
                if (filters.Any(f => f.Name == pFilter.Name))
                {
                    return;
                }
                pFilter.PropertyChanged += OnFilterPropertyChanged;
                filters.Add(pFilter);
                return;
            }

            if (filters.Count == 0)
            {
                var pParentFilter = new PFilter
                {
                    Name = string.Join(".", pFilter.NamePath.Take(currentNamePath.Count + 1)),
                    IsSelected = isParentSelected,
                };
                pParentFilter.PropertyChanged += OnFilterPropertyChanged;
                filters.Add(pParentFilter);
                FindOrCreateParentFilter(pFilter, pParentFilter.Children, pParentFilter.NamePath, pParentFilter.IsSelected);
                return;
            }

            foreach (var filter in filters)
            {
                if (filter.Name == pFilter.Name)
                {
                    return;
                }
                if (IsAncestor(filter.NamePath, pFilter.NamePath))
                {
                    FindOrCreateParentFilter(pFilter, filter.Children, filter.NamePath, filter.IsSelected);
                    return;
                }
            }

            {
                var pParentFilter = new PFilter
                {
                    Name = string.Join(".", pFilter.NamePath.Take(currentNamePath.Count + 1)),
                    IsSelected = isParentSelected,
                };
                pParentFilter.PropertyChanged += OnFilterPropertyChanged;
                filters.Add(pParentFilter);
                FindOrCreateParentFilter(pFilter, pParentFilter.Children, pParentFilter.NamePath, pParentFilter.IsSelected);
            }
        }

        private void OnFilterPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var pFilter = sender as PFilter;
            if (pFilter == null)
            {
                return;
            }

            if (e.PropertyName == pFilter.GetPropertyName(f => f.IsSelected) && FilterSelectionChanged != null)
            {
                FilterSelectionChanged(this);
            }
        }

        public bool IsAncestor(List<string> ancestorPath, List<string> path)
        {
            var commonPath = FindCommonNamePath(ancestorPath, path);
            return commonPath.Count == ancestorPath.Count;
        }

        public bool IsParentPath(List<string> parentPath, List<string> childPath)
        {
            if (parentPath.Count != childPath.Count - 1)
            {
                return false;
            }

            var commonNamePath = FindCommonNamePath(parentPath, childPath);
            return commonNamePath.Count == parentPath.Count;
        }

        public List<string> FindCommonNamePath(List<string> path1, List<string> path2)
        {
            var result = new List<string>();
            var max = Math.Min(path1.Count, path2.Count);

            for (var i = 0; i < max; i++)
            {
                if (path1[i] == path2[i])
                {
                    result.Add(path1[i]);
                }
                else
                {
                    break;
                }
            }
            return result;
        }

        public bool IsAccepted(PLogEntry pLogEntry)
        {
            return IsAccepted(
                pLogEntry.LoggerText,
                pLogEntry.LoggerText.Split('.').ToList(),
                Filters);
        }

        public bool IsAccepted(string pathStr, List<string> path, ICollection<PFilter> filters)
        {
            if (filters.Count == 0)
            {
                return true;
            }
            foreach (var filter in filters)
            {
                if (filter.Name == pathStr)
                {
                    return filter.IsSelected;
                }
                if (IsAncestor(filter.NamePath, path))
                {
                    return filter.Children.Count == 0
                        ? filter.IsSelected
                        : IsAccepted(pathStr, path, filter.Children);
                }
            }

            return true;
        }


        public override string ToString()
        {
            return Name;
        }
    }
}
