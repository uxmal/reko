/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Machine;
using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Gui
{
    public class SignatureParser
    {
        private IProcessorArchitecture arch;
        private string str;
        private int idx;

        public SignatureParser(IProcessorArchitecture arch)
        {
            this.arch = arch;
        }

        public void Parse(string signatureString)
        {
            this.str = signatureString;
            idx = 0;

            var ret = ParseReturn();
            var procName = ParseProcedureName();
            var procArgs = ParseProcedureArgs();
            ProcedureName = procName;
            Signature = new SerializedSignature();
            Signature.ReturnValue = ret;
            Signature.Arguments = procArgs;
            IsValid = true;
        }

        public string ProcedureName { get; private set;}

        public SerializedSignature Signature { get; private set; }

        public bool IsValid { get; private set; }

        private SerializedArgument ParseReturn()
        {
            string w = GetNextWord();
            if (w == "void")
                return null;
            Console.WriteLine(w);
            var reg = arch.GetRegister(w);
            if (reg != null)
            {
                var arg = new SerializedArgument();
                arg.Name = reg.Name;
                arg.Kind = new RegisterStorage(reg).Serialize();
                arg.OutParameter = false;
                return arg;
            }
            throw new NotImplementedException();
        }

        private string ParseProcedureName()
        {
            return GetNextWord();
        }

        private SerializedArgument[] ParseProcedureArgs()
        {
            EatWhiteSpace();
            if (idx >= str.Length || str[idx] != '(')
                return null;
            ++idx;
            var args = new List<SerializedArgument>();
            for (; ; )
            {
                EatWhiteSpace();
                if (idx >= str.Length)
                    return null;
                if (str[idx] == ')')
                    return args.ToArray();
                if (args.Count > 0)
                {
                    if (str[idx] != ',')
                        return null;
                    ++idx;
                }
                var arg = ParseArg();
                if (arg == null)
                    return null;
                args.Add(arg);
            }
        }

        private SerializedArgument ParseArg()
        {
            var w = GetNextWord();
            if (w == null)
                return null;

            MachineRegister reg;
            string type = null;
            if (!arch.TryGetRegister(w, out reg))
            {
                type = w;
                w = GetNextWord();
                if (w == null)
                    return null;
                if (!arch.TryGetRegister(w, out reg))
                    throw new NotImplementedException();
            }

            var arg = new SerializedArgument();
            arg.Name = reg.Name;
            arg.Kind = new SerializedRegister(reg.Name);
            arg.OutParameter = false;
            arg.Type = type;
            return arg;
        }

        private string GetNextWord()
        {
            EatWhiteSpace();
            if (idx >= str.Length)
                return null;

            int i = idx;
            char c = str[i];
            while (i < str.Length && (Char.IsLetterOrDigit(c) || c == '_'))
            {
                ++i;
                c = str[i];
            }
            string s = str.Substring(idx, i - idx);
            idx = i;
            return s;
        }

        private void EatWhiteSpace()
        {
            while (idx < str.Length && Char.IsWhiteSpace(str[idx]))
                ++idx;
        }
    }
}
