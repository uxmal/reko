#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Decompiler.Core;
using Decompiler.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace Decompiler.Core
{
    public class Project
    {
        public Project()
        {
            UserProcedures = new SortedList<Address, SerializedProcedure>();
            UserCalls = new SortedList<Address, SerializedCall>();
            UserGlobalData = new SortedList<Address, SerializedType>();
        }

        public Address BaseAddress { get; set; }
        public string DisassemblyFilename { get; set; }
        public string InputFilename { get; set; }
        public string IntermediateFilename { get; set; }
        public string OutputFilename { get; set; }
        public string TypesFilename { get; set; }

        /// <summary>
        /// Locations that have been identified as Procedures by the user.
        /// </summary>
        public SortedList<Address, SerializedProcedure> UserProcedures { get;  set; }

        /// <summary>
        /// Locations that have been identified as calls by the user, complete with 
        /// their signatures.
        /// </summary>
        public SortedList<Address, SerializedCall> UserCalls { get; private set; }

        /// <summary>
        /// Global data identified by the user.
        /// </summary>
        public SortedList<Address, SerializedType> UserGlobalData { get; private set; }

        public SerializedProject_v1 Save()
        {
            var sp = new SerializedProject_v1()
            {
                Input = new DecompilerInput_v1
                {
                    Address = BaseAddress.ToString(),
                    Filename = InputFilename,
                },
                Output = new DecompilerOutput_v1
                {
                    DisassemblyFilename = DisassemblyFilename,
                    IntermediateFilename = IntermediateFilename,
                    OutputFilename = OutputFilename,
                    TypesFilename = TypesFilename,
                }
            };
            foreach (var de in UserProcedures)
            {
                de.Value.Address = de.Key.ToString();
                sp.UserProcedures.Add(de.Value);
            }
            foreach (var de in UserCalls)
            {
                sp.UserCalls.Add(de.Value);
            }
            foreach (var de in UserGlobalData)
            {
            }
            return sp;
        }

        public void Load(SerializedProject_v1 sp)
        {
            var serializer = new ProjectSerializer();
            var project = serializer.LoadProject(sp);
        }
        public void SetDefaultFileNames(string inputFilename)
        {
            InputFilename = inputFilename;

            DisassemblyFilename = Path.ChangeExtension(inputFilename, ".asm");
            IntermediateFilename = Path.ChangeExtension(inputFilename, ".dis");
            OutputFilename = Path.ChangeExtension(inputFilename, ".c");
            TypesFilename = Path.ChangeExtension(inputFilename, ".h");
        }
    }
}
