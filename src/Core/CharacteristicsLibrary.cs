#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using Reko.Core.Serialization;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Reko.Core
{
    public class CharacteristicsLibrary
    {
        public CharacteristicsLibrary()
        {
            Entries = new Dictionary<string, Serialization.ProcedureCharacteristics>();
        }

        public static CharacteristicsLibrary Load(string filename, IFileSystemService fsSvc)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Serialization.CharacteristicsLibrary_v1));
            using (var stm = fsSvc.CreateFileStream(filename, FileMode.Open, FileAccess.Read))
            {
                var slib = (Serialization.CharacteristicsLibrary_v1) ser.Deserialize(stm);
                return new CharacteristicsLibrary
                {
                    Entries = slib.Entries
                        .ToDictionary(e => e.ProcedureName, e => e.Characteristics)
                };
            }
        }

        public Dictionary<string, ProcedureCharacteristics> Entries { get; set; }

        public ProcedureCharacteristics Lookup(string procName)
        {
            ProcedureCharacteristics ch;
            if (!Entries.TryGetValue(procName, out ch))
                return null;
            return ch;
        }
    }
}
