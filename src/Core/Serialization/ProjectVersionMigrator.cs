using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Core.Serialization
{
    public static class ProjectVersionMigrator
    {
        public static Project_v5 MigrateProject(Project_v4 v4) => new Project_v5()
        {
            ArchitectureName = v4.ArchitectureName,
            PlatformName = v4.PlatformName,
            InputFiles = v4.InputFiles.Select(i => MigrateDecompilerInput(i)).ToList(),
            MetadataFiles = v4.MetadataFiles,
            AssemblerFiles = v4.AssemblerFiles,
        };

        /// <summary>
        /// Ignored fields (They existed in v4 but are unused in current code):
        ///     IntermediateFilename
        ///     GlobalsFilename
        /// </summary>
        /// <param name="v4"></param>
        /// <returns></returns>
        public static DecompilerInput_v5 MigrateDecompilerInput(DecompilerInput_v4 v4)
        {
            var v5 = new DecompilerInput_v5()
            {
                Filename = v4.Filename,
                Comment = v4.Comment,
                DisassemblyDirectory = v4.DisassemblyFilename != null ? Path.GetDirectoryName(v4.DisassemblyFilename) : null,
                SourceDirectory = v4.OutputFilename != null ? Path.GetDirectoryName(v4.OutputFilename) : null,
                IncludeDirectory = v4.TypesFilename != null ? Path.GetDirectoryName(v4.TypesFilename) : null,
                ResourcesDirectory = v4.ResourcesDirectory,
                User = v4.User ?? new UserData_v4
                {
                    ExtractResources = true,
                }
            };

            if (string.IsNullOrWhiteSpace(v5.User.OutputFilePolicy))
                v5.User.OutputFilePolicy = Program.SingleFilePolicy;

            return v5;
        }
    }
}
