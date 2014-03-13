using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Decompiler.Core.Serialization
{
    public class ProjectSerializer
    {
        public void Save(Project_v2 project, TextWriter sw)
        {
            XmlSerializer ser = SerializedLibrary.CreateSerializer_v1(typeof(Project_v2));
            ser.Serialize(sw, project);
        }

        public Project LoadProject(string filename)
        {
            using (FileStream stm = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return LoadProject(stm);
            }
        }

        public Project LoadProject(Stream stm)
        {
            var rdr = new XmlTextReader(stm);
            XmlSerializer ser = SerializedLibrary.CreateSerializer_v1(typeof(Project_v2));
            if (ser.CanDeserialize(rdr))
                return LoadProject((Project_v2) ser.Deserialize(rdr));
            ser = SerializedLibrary.CreateSerializer_v1(typeof(SerializedProject_v1));
            if (ser.CanDeserialize(rdr))
                return LoadProject((SerializedProject_v1) ser.Deserialize(rdr));
            return null;
        }

        public Project LoadProject(Project_v2 sp)
        {
            var inputFiles = sp.Inputs.Select(s => LoadInputFile(s));
            var project = new Project();
            project.InputFiles.AddRange(inputFiles);
            return project;
        }

        private InputFile LoadInputFile(DecompilerInput_v1 sInput)
        {
            var file = new InputFile
            {
                BaseAddress = Address.Parse(sInput.Address, 16),
                Filename = sInput.Filename,
            };
            if (sInput.UserProcedures != null)
            {
                file.UserProcedures = sInput.UserProcedures
                        .Select(sup => new KeyValuePair<Address, SerializedProcedure>(
                            Address.Parse(sup.Address, 16),
                            sup))
                        .Where(kv => kv.Key != null)
                        .ToSortedList(kv => kv.Key, kv => kv.Value);
            }
            return file;
        }

        public Project LoadProject(SerializedProject_v1 sp)
        {
            InputFile inputFile = new InputFile
            {
                BaseAddress = Address.Parse(sp.Input.Address, 16),
                Filename = sp.Input.Filename,
                DisassemblyFilename = sp.Output.DisassemblyFilename,
                IntermediateFilename = sp.Output.IntermediateFilename,
                OutputFilename = sp.Output.OutputFilename,
                TypesFilename = sp.Output.TypesFilename,
                UserProcedures = sp.UserProcedures
                    .Select(sup => new KeyValuePair<Address, SerializedProcedure>(
                        Address.Parse(sup.Address, 16),
                        sup))
                    .Where(kv => kv.Key != null)
                    .ToSortedList(kv => kv.Key, kv => kv.Value)
            };

            foreach (var uc in sp.UserCalls)
            {
                var addr = Address.Parse(uc.InstructionAddress, 16);
                if (addr != null)
                    inputFile.UserCalls.Add(addr, uc);
            }
            return new Project
            {
                InputFiles = { inputFile }
            };
        }
    }
}
