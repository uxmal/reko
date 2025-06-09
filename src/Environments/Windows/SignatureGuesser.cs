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

using Reko.Core;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Environments.Windows
{
    /// <summary>
    /// Guesses a procedure signature based on the name of a procedure.
    /// </summary>
    /// <remarks>
    /// Various name mangling schemes can give hints on the number of arguments,
    /// their types, and even the type of the return value.
    /// </remarks>
    public class SignatureGuesser
    {
        public static (string?, SerializedType?, SerializedType?) InferTypeFromName(string fnName)
        {
            if (fnName[0] == '?')
            {
                // Microsoft-mangled signatures begin with '?'
                var pmnp = new MsMangledNameParser(fnName);
                try
                {
                    var field = pmnp.Parse();
                    return field;
                }
                catch (Exception ex)
                {
                    Debug.Print("*** Error parsing {0}. {1}", fnName, ex.Message);
                    return (null,null,null);
                }
            }
            return (null,null,null);
        }

        /// <summary>
        /// Guesses the signature of a procedure based on its name.
        /// </summary>
        /// <param name="fnName"></param>
        /// <param name="loader"></param>
        /// <param name="arch"></param>
        /// <returns></returns>
        public static ProcedureBase_v1? SignatureFromName(string fnName, IPlatform platform)
        {
            int argBytes;
            if (fnName[0] == '_')
            {
                // Win32 prefixes cdecl and stdcall functions with '_'. Stdcalls will have @<nn> 
                // where <nn> is the number of bytes pushed on the stack. If 0 bytes are pushed
                // the result is indistinguishable from the corresponding cdecl call, which is OK.
                int lastAt = fnName.LastIndexOf('@');
                if (lastAt < 0)
                    return CdeclSignature(fnName.Substring(1), platform.Architecture);
                string name = fnName.Substring(1, lastAt - 1);
                if (!Int32.TryParse(fnName.Substring(lastAt + 1), out argBytes))
                    return CdeclSignature(name, platform.Architecture);
                else
                    return StdcallSignature(name, argBytes, platform.Architecture);
            }
            else if (fnName[0] == '@')
            {
                // Borland-mangled signatures begin with '@'.
                var bmnp = new BorlandMangledNamedParser(fnName);
                var field = bmnp.Parse();
                if (field.Item1 is not null)
                {
                    var sproc = field.Item2 as SerializedSignature;
                    if (sproc is not null)
                    {
                        return new Procedure_v1
                        {
                            Name = field.Item1,
                            Signature = sproc,
                        };
                    }
                    throw new NotImplementedException();
                }
                // Win32 prefixes fastcall functions with '@'.
                int lastAt = fnName.LastIndexOf('@');
                if (lastAt <= 0)
                    return CdeclSignature(fnName.Substring(1), platform.Architecture);
                string name = fnName.Substring(1, lastAt - 1);
                if (!Int32.TryParse(fnName.Substring(lastAt + 1), out argBytes))
                    return CdeclSignature(fnName, platform.Architecture);
                else
                    return FastcallSignature(name, argBytes, platform.Architecture);
            }
            else if (fnName[0] == '?')
            {
                // Microsoft-mangled signatures begin with '?'
                var pmnp = new MsMangledNameParser(fnName);
                (string?,SerializedType?,SerializedType?) field;
                try
                {
                    field = pmnp.Parse();
                }
                catch (Exception ex)
                {
                    Debug.Print("*** Error parsing {0}. {1}", fnName, ex.Message);
                    return null;
                }
                var sproc = field.Item2 as SerializedSignature;
                if (sproc is not null)
                {
                    return new Procedure_v1 {
                        Name = field.Item1,
                        Signature = sproc,
                    };
                }
            }
            return null;
        }

        private static ProcedureBase_v1 CdeclSignature(string name, IProcessorArchitecture arch)
        {
            var sproc = new SerializedSignature
            {
                ParametersValid = false,
                StackDelta = (short)arch.PointerType.Size,
            };
            return new Procedure_v1
            {
                Name = name,
                Signature = sproc,
            };
        }

        private static ProcedureBase_v1 StdcallSignature(string name, int argBytes, IProcessorArchitecture arch)
        {
            var sproc = new SerializedSignature
            {
                ParametersValid = false,
                StackDelta = (short)(argBytes + arch.PointerType.Size),
            };
            return new Procedure_v1
            {
                Name = name,
                Signature = sproc,
            };
        }

        private static ProcedureBase_v1 FastcallSignature(string name, int argBytes, IProcessorArchitecture arch)
        {
            var sproc = new SerializedSignature
            {
                ParametersValid = false,
                StackDelta = argBytes - 2 * arch.PointerType.Size, // ecx, edx
            };
            return new Procedure_v1
            {
                Name = name,
                Signature = sproc,
            };
        }
    }
}
