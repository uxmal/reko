#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core.Serialization
{
    public class ProjectSaver 
    {
        public Project_v3 Save(Project project)
        {
            var inputs = new List<ProjectFile_v3>();
            inputs.AddRange(project.Programs.Select(p => VisitProgram(p)));
            inputs.AddRange(project.MetadataFiles.Select(m => VisitMetadataFile(m)));
            var sp = new Project_v3()
            {
                Inputs = inputs
            };
            return sp;
        }

        public ProjectFile_v3 VisitProgram(Program program)
        {
            var dtSerializer = new DataTypeSerializer();
            return new DecompilerInput_v3
            {
                Address = program.Image != null
                    ? program.Image.BaseAddress.ToString()
                    : null,
                Filename = program.Filename,
                User = new UserData_v3
                {
                    Procedures = program.User.Procedures
                        .Select(de => { de.Value.Address = de.Key.ToString(); return de.Value; })
                        .ToList(),
                    Calls = program.User.Calls
                        .Select(uc => uc.Value)
                        .ToList(),
                    GlobalData = program.User.Globals
                        .Select(de => new GlobalDataItem_v2
                        {
                            Address = de.Key.ToString(),
                            DataType = de.Value.DataType,
                            Name = string.Format("g_{0:X}", de.Key.ToLinear())
                        })
                        .ToList(),
                    OnLoadedScript = program.User.OnLoadedScript,
                    Heuristics = program.User.Heuristics.Select(h => new Heuristic_v3 { Name = h }).ToList(),
                },
                DisassemblyFilename = program.DisassemblyFilename,
                IntermediateFilename = program.IntermediateFilename,
                OutputFilename = program.OutputFilename,
                TypesFilename = program.TypesFilename,
                GlobalsFilename = program.GlobalsFilename,
            };
        }

        public ProjectFile_v3 VisitMetadataFile(MetadataFile metadata)
        {
            return new MetadataFile_v3
            {
                 Filename = metadata.Filename,
                  ModuleName = metadata.ModuleName,
            };
        }
    }
}
