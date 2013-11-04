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
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Linq;

namespace LogReceiver.Core.Extensions
{
    public static class NotifyPropertyChangeExtensions
    {
        public static IObservable<TResult> ToObservable<TTarget, TResult>(this TTarget target, Expression<Func<TTarget, TResult>> property) where TTarget : INotifyPropertyChanged
        {
            var body = property.Body;
            string propertyName;

            if (body is MemberExpression)
            {
                propertyName = (body as MemberExpression).Member.Name;
            }
            else if (body is MethodCallExpression)
            {
                propertyName = (body as MethodCallExpression).Method.Name;
            }
            else
            {
                throw new NotSupportedException("Only use expressions that call a single property or method");
            }

            var getValueFunc = property.Compile();
            return Observable.Create<TResult>(o =>
                {
                    PropertyChangedEventHandler eventHandler = (s, pce) =>
                        {
                            if (pce.PropertyName == null || pce.PropertyName == propertyName)
                            {
                                o.OnNext(getValueFunc(target));
                            }
                        };
                    target.PropertyChanged += eventHandler;
                    return () => target.PropertyChanged -= eventHandler;
                });
        }
    }
}
