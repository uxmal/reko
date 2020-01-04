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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Serialization.Json
{
    public class JsonProjectSerializer
    {
        private JsonWriter js;

        public void Serialize(Project project, TextWriter writer)
        {
            this.js = new JsonWriter(writer);
            js.BeginObject();
            js.WriteKeyValue("programs", () => js.WriteList(project.Programs, WriteProgram));
            js.EndObject();
        }

        public void WriteProgram(Program program)
        {
            js.BeginObject();
            js.WriteKeyValue("name", program.Name);
            js.WriteKeyValue("procedures", () => js.WriteList(program.Procedures, WriteProcedure));
            js.EndObject();
        }

        public void WriteProcedure(KeyValuePair<Address, Procedure> proc)
        {
            js.BeginObject();
            js.WriteKeyValue("address", proc.Key.ToString());
            js.WriteKeyValue("name", proc.Value.Name);
            js.EndObject();
        }
    }
}
