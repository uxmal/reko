#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Reko.Core
{
    /// <summary>
    /// A characteristics library contains <see cref="ProcedureCharacteristics"/>
    /// of procedures.
    /// </summary>
    public class CharacteristicsLibrary
    {
        /// <summary>
        /// Constructs an empty characteristics library.
        /// </summary>
        public CharacteristicsLibrary()
        {
            Entries = new Dictionary<string, ProcedureCharacteristics>();
        }

        /// <summary>
        /// Loads a characteristics library from the specified file.
        /// </summary>
        /// <param name="filename">Name of the file containing the characteristics library.
        /// </param>
        /// <param name="fsSvc">Instance of <see cref="IFileSystemService"/> used to access
        /// the file.
        /// </param>
        /// <returns>The loaded characteristcs library.</returns>
        public static CharacteristicsLibrary Load(string filename, IFileSystemService fsSvc)
        {
            var ser = new XmlSerializer(typeof(Serialization.CharacteristicsLibrary_v1));
            using (var stm = fsSvc.CreateFileStream(filename, FileMode.Open, FileAccess.Read))
            {
                var slib = (CharacteristicsLibrary_v1) ser.Deserialize(stm)!;
                if (slib.Entries is null)
                {
                    return new CharacteristicsLibrary();
                }
                else
                {
                    return new CharacteristicsLibrary
                    {
                        Entries = slib.Entries
                            .Where(e => e.ProcedureName is not null && e.Characteristics is not null)
                            .ToDictionary(e => e.ProcedureName!, e => e.Characteristics!)
                    };
                }
            }
        }

        /// <summary>
        /// The entries in the characteristics library. The key is the name of the procedure.
        /// </summary>
        public Dictionary<string, ProcedureCharacteristics> Entries { get; private init; }

        /// <summary>
        /// Looks up the characteristics of a procedure by name.
        /// </summary>
        /// <param name="procName">The name of the procedure.</param>
        /// <returns>If found, a <see cref="ProcedureCharacteristics"/> instance, otherwise null.
        /// </returns>
        public ProcedureCharacteristics? Lookup(string procName)
        {
            if (!Entries.TryGetValue(procName, out ProcedureCharacteristics? ch))
                return null;
            return ch;
        }
    }
}
