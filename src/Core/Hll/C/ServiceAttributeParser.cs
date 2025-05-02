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
using System;
using System.Collections.Generic;

namespace Reko.Core.Hll.C
{
    /// <summary>
    ///  Parses the Reko [[reko::service]] attribute.
    /// </summary>
    public class ServiceAttributeParser
    {
        private readonly List<CToken>? tokens;
        private int iToken;

        /// <summary>
        /// Creates an instance of the <see cref="ServiceAttributeParser"/> class.
        /// </summary>
        /// <param name="attrService"></param>
        public ServiceAttributeParser(CAttribute attrService)
        {
            this.tokens = attrService.Tokens;
        }

        /// <summary>
        /// Parses the tokens in the [[reko::service]] attribute,
        /// extracting information to create a <see cref="SyscallInfo_v1"/>
        /// instance.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="CParserException"></exception>
        public SyscallInfo_v1 Parse()
        {
            int? nVector = null;
            SerializedRegValue[]? regs = null;
            do
            {
                var id = Expect<string>(CTokenType.Id, "identifier");
                switch (id)
                {
                case "vector":
                    Expect(CTokenType.Assign, "=");
                    nVector = Expect<int>(CTokenType.NumericLiteral, "number");
                    break;
                case "regs":
                    regs = ParseRegs();
                    break;
                default:
                    throw new NotImplementedException($"Unexpected '{id}' in reko::service attribute.");
                }
            } while (PeekAndDiscard(CTokenType.Comma));
            if (!nVector.HasValue)
                throw new CParserException("Expected a vector number.");
            return new SyscallInfo_v1
            {
                Vector = nVector.Value.ToString("X"),
                RegisterValues = regs,
            };
        }

        // regs '=' '{' id ':' number (',' id ':' number)* '}'
        private SerializedRegValue[] ParseRegs()
        {
            var regvalues = new List<SerializedRegValue>();
            Expect(CTokenType.Assign, "=");
            Expect(CTokenType.LBrace, "{");
            if (PeekAndDiscard(CTokenType.RBrace))
                return regvalues.ToArray();
            do
            {
                var regName = Expect<string>(CTokenType.Id, "identifier");
                Expect(CTokenType.Colon, ":");
                var value = Expect<int>(CTokenType.NumericLiteral, "number");
                regvalues.Add(new SerializedRegValue { Register = regName, Value = value.ToString("X") });
            } while (PeekAndDiscard(CTokenType.Comma));
            Expect(CTokenType.RBrace, "}");
            return regvalues.ToArray();
        }

        private bool PeekAndDiscard(CTokenType type)
        {
            if (tokens is { } && iToken < tokens.Count && tokens[iToken].Type == type)
            {
                ++iToken;
                return true;
            }
            else
            {
                return false;
            }

        }
        private void Expect(CTokenType type, string errorMsg)
        {
            if (tokens is null || iToken >= tokens.Count || tokens[iToken++].Type != type)
                throw new CParserException($"Expected '{errorMsg}'.");
        }

        private T Expect<T>(CTokenType type, string errorMsg)
        {
            if (tokens is null || iToken >= tokens.Count || tokens[iToken].Type != type)
                throw new CParserException($"Expected '{errorMsg}'.");
            return (T) tokens[iToken++].Value!;
        }

    }
}