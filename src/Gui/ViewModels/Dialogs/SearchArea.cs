#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Gui.ViewModels.Dialogs
{
    public class SearchArea
    {

        public SearchArea()
        {
            this.Areas = new List<ProgramAddressRange>();
        }

        public SearchArea(List<ProgramAddressRange> areas)
        {
            this.Areas = areas;
        }

        private enum State
        {
            Start,
            InBeginRange,
            InEndRange,
        }

        public static bool TryParse(Program program, string freeFormAreas, [MaybeNullWhen(false)] out SearchArea result)
        {
            var arch = program.Architecture;
            var i = 0;
            var iBegin = -1;
            result = null;
            Address? addrBegin = null;
            State state = State.Start;
            var areas = new List<ProgramAddressRange>();
            for (; ; )
            {
                i = SkipWs(freeFormAreas, i);
                if (i >= freeFormAreas.Length)
                    break;
                char c = freeFormAreas[i++];
                switch (state)
                {
                case State.Start:
                    switch (c)
                    {
                    case '[':
                        state = State.InBeginRange;
                        iBegin = i;
                        break;
                    case ',':
                        break;
                    default:
                        return false;
                    }
                    break;
                case State.InBeginRange:
                    switch (c)
                    {
                    case '-':
                        if (!TryParseAddress(arch, freeFormAreas, iBegin, i - 1, out addrBegin))
                            return false;
                        state = State.InEndRange;
                        iBegin = i;
                        break;
                    default:
                        break;
                    }
                    break;
                case State.InEndRange:
                    switch (c)
                    {
                    case ']':
                        int adjustment = 1;
                        Debug.Assert(addrBegin is not null);
                        if (!TryParseAddress(arch, freeFormAreas, iBegin, i - 1, out var addrEnd))
                            return false;
                        if (addrBegin >= addrEnd)
                            return false;
                        areas.Add(ProgramAddressRange.Create(
                            program,
                            addrBegin!,
                            (addrEnd - addrBegin) + adjustment));
                        state = State.Start;
                        break;
                    case ')':
                        if (!TryParseAddress(arch, freeFormAreas, iBegin, i - 1, out addrEnd))
                            return false;
                        if (addrBegin! >= addrEnd)
                            return false;
                        areas.Add(ProgramAddressRange.Create(
                            program,
                            addrBegin!,
                            (addrEnd - addrBegin!)));
                        break;
                    default:
                        break;
                    }
                    break;
                }
            }
            if (state != State.Start)
                return false;

            result = new SearchArea(areas);
            return true;
        }

        public List<ProgramAddressRange> Areas { get; }

        public override bool Equals(object? obj)
        {
            if (obj is not SearchArea that)
                return false;
            if (this == that)
                return true;
            if (this.Areas.Count != that.Areas.Count)
                return false;
            for (int i = 0; i < Areas.Count; ++i)
            {
                if (!this.Areas[i].Equals(that.Areas[i]))
                    return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            var h = new HashCode();
            h.Add(Areas.Count);
            for (int i = 0; i < Areas.Count; ++i)
            {
                h.Add(Areas[i]);
            }
            return h.ToHashCode();
        }

        private static int SkipWs(string str, int i)
        {
            for (; i < str.Length; ++i)
            {
                char c = str[i];
                if (!char.IsWhiteSpace(c))
                    return i;
            }
            return i;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendJoin(", ", this.Areas
                .Select(a => $"[{a.Address}-{a.Address + (a.Length - 1)}]"));
            return sb.ToString();
        }

        private static bool TryParseAddress(IProcessorArchitecture arch, string freeFormAreas, int iBegin, int iEnd, [MaybeNullWhen(false)] out Address addr)
        {
            return arch.TryParseAddress(
                freeFormAreas.Substring(iBegin, iEnd - iBegin),
                out addr);
        }

    }
}
