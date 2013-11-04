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
using System.Linq;
using System.Text.RegularExpressions;
using LogReceiver.Core.Models;

namespace LogReceiver.Core.StackTraces
{
    public class CSharpStackTraceParser : ICSharpStackTraceParser
    {
        private readonly Regex _regexMethodCall = new Regex(
            @"at (?<namespace>.*)\.(?<class>.*)\.(?<method>.*)(?<arguments>\(.*\))( in (?<file>[a-z]:[a-å0-9_\-\\ \.]*)(:line (?<line>\d*)){0,1}){0,1}",
            RegexOptions.IgnoreCase |
            RegexOptions.Singleline |
            RegexOptions.CultureInvariant |
            RegexOptions.Compiled);
        private readonly Regex _regexLineSplit = new Regex(@"[\n\r]");

        public CStackTrace Parse(string str)
        {
            var matches = (
                from line in _regexLineSplit.Split(str)
                let match = _regexMethodCall.Match(line.Trim())
                where match.Length > 0
                select match
                ).ToList();

            var methodCalls = new List<CMethodCall>();
            foreach (var match in matches)
            {
                var methodCall = new CMethodCall
                    {
                        Namespace = match.Groups["namespace"].Value,
                        Class = match.Groups["class"].Value,
                        Method = match.Groups["method"].Value,
                        Arguments = match.Groups["arguments"].Value,
                        File = match.Groups["file"].Value,
                        Line = string.IsNullOrEmpty(match.Groups["line"].Value)
                            ? null as int?
                            : int.Parse(match.Groups["line"].Value)
                    };
                methodCalls.Add(methodCall);
            }

            return new CStackTrace
                {
                    MethodCalls = methodCalls,
                };
        }
    }
}
