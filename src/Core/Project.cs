#region License
/* 
 * Copyright (C) 1999-2012 John Källén.
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
        }

        public Address BaseAddress { get; set; }
        public string DisassemblyFilename { get; set; }
        public string InputFilename { get; set; }
        public string IntermediateFilename { get; set; }
        public string OutputFilename { get; set; }
        public string TypesFilename { get; set; }
        public SortedList<Address, SerializedProcedure> UserProcedures { get; private set; }
        public SortedList<Address, SerializedCall> UserCalls { get; private set; }

        public SerializedProject Save()
        {
            var sp = new SerializedProject();
            sp.Input.Address = BaseAddress.ToString();
            sp.Input.Filename = InputFilename;
            sp.Output.DisassemblyFilename = DisassemblyFilename;
            sp.Output.IntermediateFilename = IntermediateFilename;
            sp.Output.OutputFilename = OutputFilename;
            sp.Output.TypesFilename = TypesFilename;
            foreach (var de in UserProcedures)
            {
                de.Value.Address = de.Key.ToString();
                sp.UserProcedures.Add(de.Value);
            }
            foreach (var de in UserCalls)
            {
                sp.UserCalls.Add(de.Value);
            }
            return sp;
        }

        public void Load(SerializedProject sp)
        {
            BaseAddress = Address.ToAddress(sp.Input.Address, 16);
            InputFilename = sp.Input.Filename;
            DisassemblyFilename = sp.Output.DisassemblyFilename;
            IntermediateFilename = sp.Output.IntermediateFilename;
            OutputFilename = sp.Output.OutputFilename;
            TypesFilename = sp.Output.TypesFilename;
            foreach (var up in sp.UserProcedures)
            {
                var addr = Address.ToAddress(up.Address, 16);
                if (addr != null)
                {
                    UserProcedures.Add(addr, up);
                }
            }
            foreach (var uc in sp.UserCalls)
            {
                var addr = Address.ToAddress(uc.InstructionAddress, 16);
                if (addr != null)
                    UserCalls.Add(addr, uc);
            }
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
