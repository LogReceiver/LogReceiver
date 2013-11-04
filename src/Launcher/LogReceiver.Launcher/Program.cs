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
using System.Reflection;
using System.Windows;
using Lawnch.Launcher;
using Lawnch.Launcher.Enums;
using Lawnch.Launcher.Lawnchers.Wpf.Mvvm.Views.Main;
using LogReceiver.Launcher.Views.Main;

namespace LogReceiver.Launcher
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;

            var assembly = typeof(Program).Assembly;
            var version = assembly.GetName().Version;

            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
                {
                    MessageBox.Show(
                        eventArgs.ExceptionObject.ToString(),
                        "Unhanled exception");
                };

            Lawncher.Launch(
                version,
                "LogReceiver",
                lc =>
                    {
                        lc.RegisterServies(sr =>
                            {
                                sr.SetImplementation<IMainView, MainView>();
                            });

                        lc.SetLawncher(BuiltInLawncherType.Wpf);
                    });
        }

        private static Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var assemblyName = new AssemblyName(args.Name);

            var path = assemblyName.Name + ".dll";

            using (var stream = executingAssembly.GetManifestResourceStream(path))
            {
                if (stream == null)
                {
                    return null;
                }
                var assemblyRawBytes = new byte[stream.Length];
                stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);
                return Assembly.Load(assemblyRawBytes);
            }
        }
    }
}
