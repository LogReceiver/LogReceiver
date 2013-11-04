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

using AutoMapper;
using LogReceiver.Ui.UserControls.LogEntryList.UserControls.FilterSelection.DOM.Export;

namespace LogReceiver.Ui.UserControls.LogEntryList.UserControls.FilterSelection.DOM.Mapping
{
    public class FilterConfigurationMapper : IFilterConfigurationMapper
    {
        public FilterConfigurationMapper()
        {
            Mapper.CreateMap<PFilter, ExFilter>();

            Mapper.CreateMap<ExFilter, PFilter>()
                .ForMember(dst => dst.LastPathPart, opt => opt.Ignore())
                .ForMember(dst => dst.NamePath, opt => opt.Ignore())
                ;

            Mapper.CreateMap<PFilterProfile, ExFilterProfile>()
                ;
            Mapper.CreateMap<ExFilterProfile, PFilterProfile>()
                ;
        }

        public ExFilterProfile ToExFilterProfile(PFilterProfile pFilterProfile)
        {
            return Mapper.Map<PFilterProfile, ExFilterProfile>(pFilterProfile);
        }

        public PFilterProfile ToPFilterProfile(ExFilterProfile exFilterProfile)
        {
            return Mapper.Map<ExFilterProfile, PFilterProfile>(exFilterProfile);
        }
    }
}
