#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Machine;
using Reko.Core.Types;
using System;

namespace Reko.Analysis
{
    public class CallingConventionMatcher
    {
        private readonly IPlatform platform;

        public CallingConventionMatcher(IPlatform platform)
        {
            this.platform = platform;
        }


        public ICallingConvention? DetermineCallingConvention(FunctionType signature, IProcessorArchitecture arch)
        {
            // Give IPlatform the first chance.
            var cconv = this.platform.DetermineCallingConvention(signature, arch);
            var score = ComputeScore(signature, cconv);
            var ccs = platform.CallingConventions;
            if (arch is not null && ccs is not null && ccs.TryGetValue(arch.Name, out var archCcs))
            {
                foreach (var ccName in archCcs)
                {
                    var cc = platform.GetCallingConvention(ccName);
                    (cconv, score) = AggregateScore(signature, cc, cconv, score);
                }
            }
            if (!string.IsNullOrEmpty(platform.DefaultCallingConvention))
            {
                var cc = platform.GetCallingConvention(platform.DefaultCallingConvention);
                (cconv, _) = AggregateScore(signature, cc, cconv, score);
            }
            return cconv;
        }

        private (ICallingConvention?, int) AggregateScore(
            FunctionType func,
            ICallingConvention? cc,
            ICallingConvention? bestCconv,
            int bestScore)
        {
            if (cc is not null)
            {
                var score = ComputeScore(func, cc);
                if (score > bestScore)
                {
                    return (cc, score);
                }
            }
            return (bestCconv, bestScore);
        }

        private int ComputeScore(FunctionType func, ICallingConvention? cc)
        {
            if (cc is null || !func.ParametersValid)
                return -1;
            int score = 0;
            foreach (var inParam in func.Parameters!)
            {
                var stg = inParam.Storage;
                if (!cc.IsArgument(stg))
                    return -1;
                // We prefer calling conventions with many registers. For instance,
                // on Windows we will prefer __fastcall over __stdcall.
                if (stg is RegisterStorage)
                    ++score;
            }
            return score;
        }
    }
}
