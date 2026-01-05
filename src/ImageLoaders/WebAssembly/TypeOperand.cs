#region License
/* Copyright (C) 1999-2026 John Källén.
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

using Reko.Core.Machine;
using Reko.Core.Types;
using System;

namespace Reko.ImageLoaders.WebAssembly
{
    public class TypeOperand : MachineOperand
    {
        public TypeOperand(int typeID)
        {
            this.TypeID = typeID;
        }

        public DataType DataType { 
            get => VoidType.Instance;
            set => throw new NotSupportedException();
        }
        
        public int TypeID { get; }

        public void Render(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            var platform = (WasmPlatform?) options.Platform;
            if (TypeID < 0)
            {
                if (platform is null)
                    renderer.WriteFormat("0x{0}", -TypeID);
                else
                    renderer.WriteString(WasmPlatform.RenderValueType(-TypeID));
            }

            throw new NotImplementedException();
        }

        public string ToString(MachineInstructionRendererOptions options)
        {
            var s = new StringRenderer();
            Render(s, options);
            return s.ToString();
        }
    }
}
