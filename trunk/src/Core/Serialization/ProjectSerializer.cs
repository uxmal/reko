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
        public void Save(SerializedProject_v1 project, TextWriter sw)
        {
            XmlSerializer ser = new XmlSerializer(typeof(SerializedProject_v1));
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
            XmlSerializer ser = new XmlSerializer(typeof(SerializedProject_v1));
            return LoadProject((SerializedProject_v1) ser.Deserialize(stm));
        }

        public Project LoadProject(SerializedProject_v1 sp)
        {
            Project project = new Project
            {
                BaseAddress = Address.ToAddress(sp.Input.Address, 16),
                InputFilename = sp.Input.Filename,
                DisassemblyFilename = sp.Output.DisassemblyFilename,
                IntermediateFilename = sp.Output.IntermediateFilename,
                OutputFilename = sp.Output.OutputFilename,
                TypesFilename = sp.Output.TypesFilename,
                UserProcedures = sp.UserProcedures
                    .Select(sup => new KeyValuePair<Address, SerializedProcedure>(
                        Address.ToAddress(sup.Address, 16),
                        sup))
                    .Where(kv => kv.Key != null)
                    .ToSortedList(kv => kv.Key, kv => kv.Value)
            };
            
            foreach (var uc in sp.UserCalls)
            {
                var addr = Address.ToAddress(uc.InstructionAddress, 16);
                if (addr != null)
                    project.UserCalls.Add(addr, uc);
            }
            return project;
        }
    }
}
