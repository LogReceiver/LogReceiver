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
using AutoMapper;
using LogReceiver.Core.Models;
using LogReceiver.Ui.Models.Export;

namespace LogReceiver.Ui.Models.Mapping
{
    public class UiMapper : IUiMapper
    {
        public UiMapper()
        {
            Mapper.CreateMap<PLogEntry, CLogEntry>()
                .ForMember(dst => dst.Message, opt => opt.MapFrom(src => src.MessageText))
                .ForMember(dst => dst.Logger, opt => opt.MapFrom(src => src.LoggerText))
                .ForMember(dst => dst.Thread, opt => opt.MapFrom(src => src.ThreadText))
                .ForMember(dst => dst.Exception, opt => opt.MapFrom(src => src.ExceptionText))
                ;
            Mapper.CreateMap<CLogEntry, PLogEntry>()
                .ForMember(dst => dst.ThreadText, opt => opt.MapFrom(src => src.Thread))
                .ForMember(dst => dst.LoggerText, opt => opt.MapFrom(src => src.Logger))
                .ForMember(dst => dst.MessageText, opt => opt.MapFrom(src => src.Message))
                .ForMember(dst => dst.ExceptionText, opt => opt.MapFrom(src => src.Exception))
                .ForMember(dst => dst.ImageSource, opt => opt.Ignore())
                .ForMember(dst => dst.MessageHeaderText, opt => opt.Ignore())
                .ForMember(dst => dst.LoggerTextShort, opt => opt.Ignore())
                ;
            Mapper.CreateMap<PLogEntry, ELogEntry>()
                .ForMember(dst => dst.Thread, opt => opt.MapFrom(src => src.ThreadText))
                .ForMember(dst => dst.Logger, opt => opt.MapFrom(src => src.LoggerText))
                .ForMember(dst => dst.Message, opt => opt.MapFrom(src => src.MessageText))
                .ForMember(dst => dst.Exception, opt => opt.MapFrom(src => src.ExceptionText))
                ;

            Mapper.CreateMap<CStackTrace, PStackTrace>()
                ;
            Mapper.CreateMap<CMethodCall, PMethodCall>()
                ;
            Mapper.CreateMap<ICollection<CMethodCall>, ICollection<PMethodCall>>()
                ;
        }

        public PLogEntry ToPLogEntry(CLogEntry cLogEntry)
        {
            return Mapper.Map<CLogEntry, PLogEntry>(cLogEntry);
        }

        public CLogEntry ToCLogEntry(PLogEntry pLogEntry)
        {
            return Mapper.Map<PLogEntry, CLogEntry>(pLogEntry);
        }

        public ELogEntry ToELogEntry(PLogEntry pLogEntry)
        {
            return Mapper.Map<PLogEntry, ELogEntry>(pLogEntry);
        }

        public PStackTrace ToPStackTrace(CStackTrace cStackTrace)
        {
            return Mapper.Map<CStackTrace, PStackTrace>(cStackTrace);
        }
    }
}
