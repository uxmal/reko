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
using Decompiler.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Decompiler.Environments.Win32
{
    public class SignatureGuesser
    {
        public static ProcedureSignature SignatureFromName(string fnName, TypeLibraryLoader loader, IProcessorArchitecture arch)
        {
            int argBytes;
            if (fnName[0] == '_')
            {
                int lastAt = fnName.LastIndexOf('@');
                if (lastAt < 0)
                    return CdeclSignature(fnName.Substring(1), arch);
                string name = fnName.Substring(1, lastAt - 1);
                if (!Int32.TryParse(fnName.Substring(lastAt + 1), out argBytes))
                    return CdeclSignature(name, arch);
                else
                    return StdcallSignature(name, argBytes, arch);
            }
            else if (fnName[0] == '@')
            {
                int lastAt = fnName.LastIndexOf('@');
                if (lastAt <= 0)
                    return CdeclSignature(fnName.Substring(1), arch);
                string name = fnName.Substring(1, lastAt - 1);
                if (!Int32.TryParse(fnName.Substring(lastAt + 1), out argBytes))
                    return CdeclSignature(name, arch);
                else
                    return FastcallSignature(name, argBytes, arch);
            }
            else if (fnName[0] == '?')
            {
                var pmnp = new MsMangledNameParser(fnName);
                SerializedStructField field = null;
                try
                {
                    field = pmnp.Parse();
                }
                catch (Exception ex)
                {
                    Debug.Print("*** Error parsing {0}. {1}", fnName, ex.Message);
                    pmnp.ToString();
                    return null;
                }
                var sproc = field.Type as SerializedSignature;
                if (sproc != null)
                {
                    Debug.Print("Deserializing: {0}", sproc);
                    var sser = new ProcedureSerializer(arch, loader, "__cdecl");
                    return sser.Deserialize(sproc, arch.CreateFrame());    //$BUGBUG: catch dupes?   
                }
                else
                {
                    return null;
                }
            }
            else
                return null;
        }

        private static ProcedureSignature CdeclSignature(string name, IProcessorArchitecture arch)
        {
            return new ProcedureSignature()
            {
                ReturnAddressOnStack = arch.PointerType.Size,
            };
        }

        private static ProcedureSignature StdcallSignature(string name, int argBytes, IProcessorArchitecture arch)
        {
            return new ProcedureSignature()
            {
                ReturnAddressOnStack = arch.PointerType.Size,
                StackDelta = argBytes + arch.PointerType.Size,
            };
        }

        private static ProcedureSignature FastcallSignature(string name, int argBytes, IProcessorArchitecture arch)
        {
            return new ProcedureSignature
            {
                ReturnAddressOnStack = arch.PointerType.Size,
                StackDelta = argBytes - 2 * arch.PointerType.Size, // ecx, edx
            };
        }
    }
}
