/* 
 * Copyright (C) 1999-2009 John Källén.
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Decompiler.Core.Serialization
{
    [XmlRoot(ElementName = "project", Namespace = "http://schemata.jklnet.org/Decompiler")]
    public class DecompilerProject
    {
        public const string FileExtension = ".dcproject";

        public DecompilerProject()
        {
            this.Input = new DecompilerInput();
            this.Output = new DecompilerOutput();
            this.UserProcedures = new List<SerializedProcedure>();
            this.UserCalls = new List<SerializedCall>();
        }

        [XmlElement("input")]
        public DecompilerInput Input;

        [XmlElement("output")]
        public DecompilerOutput Output;

        [XmlElement("procedure", typeof(SerializedProcedure))]
        public List<SerializedProcedure> UserProcedures;

        [XmlElement("call", typeof(SerializedCall))]
        public List<SerializedCall> UserCalls;

        public static DecompilerProject Load(string file)
        {
            using (FileStream stm = new FileStream(file, FileMode.Open))
            {
                XmlSerializer ser = new XmlSerializer(typeof(DecompilerProject));
                return (DecompilerProject) ser.Deserialize(stm);
            }
        }

        public void Save(TextWriter sw)
        {
            XmlSerializer ser = new XmlSerializer(typeof(DecompilerProject));
            ser.Serialize(sw, this);
        }

        public void SetDefaultFileNames(string inputFilename)
        {
            Input.Filename = inputFilename;

            Output.DisassemblyFilename = Path.ChangeExtension(inputFilename, ".asm");
            Output.IntermediateFilename = Path.ChangeExtension(inputFilename, ".dis");
            Output.OutputFilename = Path.ChangeExtension(inputFilename, ".c");
            Output.TypesFilename = Path.ChangeExtension(inputFilename, ".h");
        }
    }
}
