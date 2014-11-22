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
using Decompiler.Core.Lib;
using Decompiler.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Decompiler.Core
{
    public class Project : IProjectFileVisitor<ProjectFile_v2>
    {
        public Project()
        {
            InputFiles = new ObservableRangeCollection<InputFile>();
        }

        public ObservableRangeCollection<InputFile> InputFiles { get; private set; }

        public Project_v2 Save()
        {
            var inputs = this.InputFiles.Select(i => i.Accept(this));
            var sp = new Project_v2()
            {
                Inputs = inputs.ToList()
            };
            return sp;
        }

        public ProjectFile_v2 VisitInputFile(InputFile i)
        {
            var dtSerializer = new DataTypeSerializer();
            return new DecompilerInput_v2
            {
                Address = i.BaseAddress.ToString(),
                Filename = i.Filename,
                UserProcedures = i.UserProcedures
                    .Select(de => { de.Value.Address = de.Key.ToString(); return de.Value; })
                    .ToList(),
                UserCalls = i.UserCalls
                    .Select(uc => uc.Value)
                    .ToList(),
                UserGlobalData = i.UserGlobalData
                    .Select(de => new GlobalDataItem_v2 {
                        Address = de.Key.ToString(),
                        DataType = de.Value.DataType,
                        Name = "g_" + Convert.ToString(de.Key.Linear, 16),
                    })
                    .ToList(),
                DisassemblyFilename = i.DisassemblyFilename,
                IntermediateFilename = i.IntermediateFilename,
                OutputFilename = i.OutputFilename,
                TypesFilename = i.TypesFilename,
                GlobalsFilename = i.GlobalsFilename,
            };
        }

        public ProjectFile_v2 VisitMetadataFile(MetadataFile metadata)
        {
            throw new NotImplementedException();
        }
    }
}
