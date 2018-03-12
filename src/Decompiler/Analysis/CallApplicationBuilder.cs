#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using System.Linq;
using System.Text;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Core.Code;

namespace Reko.Analysis
{
    /// <summary>
    /// Builds an application from a call instruction.
    /// </summary>
    public class CallApplicationBuilder : ApplicationBuilder, StorageVisitor<Expression>
    {
        private IProcessorArchitecture arch;
        private Dictionary<StorageDomain, CallBinding> defs;
        private Dictionary<StorageDomain, CallBinding> uses;
        private Dictionary<StorageDomain, CallBinding> map;

        public CallApplicationBuilder(IProcessorArchitecture arch, CallInstruction call, Expression callee) : base(call.CallSite, callee)
        {
            this.arch = arch;
            this.defs = call.Definitions.ToDictionary(u => u.Storage.Domain);
            var uses = new Dictionary<StorageDomain, CallBinding>();
            foreach (var u in call.Uses)
            {
                uses.Add(u.Storage.Domain, u);
            }
            this.uses = call.Uses.ToDictionary(u => u.Storage.Domain);
        }

        public override Expression Bind(Identifier id)
        {
            return With(uses, id.Storage);
        }

        public override OutArgument BindOutArg(Identifier id)
        {
            var exp = With(defs, id.Storage);
            return new OutArgument(arch.FramePointerType, exp);
        }

        public override Expression BindReturnValue(Identifier id)
        {
            return With(defs, id.Storage);
        }

        private Expression With(Dictionary<StorageDomain, CallBinding> map, Storage stg)
        {
            this.map = map;
            var exp = stg.Accept(this);
            this.map = null;
            return exp;
        }

        public Expression VisitFlagGroupStorage(FlagGroupStorage grf)
        {
            return map[grf.Domain].Expression;
        }

        public Expression VisitFpuStackStorage(FpuStackStorage fpu)
        {
            foreach (var de in this.map
              .Where(d => d.Value.Storage is FpuStackStorage))
            {
                if (((FpuStackStorage)de.Value.Storage).FpuStackOffset == fpu.FpuStackOffset)
                    return de.Value.Expression;
            }
            throw new NotImplementedException(string.Format("Offsets not matching? SP({0})", fpu.FpuStackOffset));
        }

        public Expression VisitMemoryStorage(MemoryStorage global)
        {
            throw new NotImplementedException();
        }

        public Expression VisitOutArgumentStorage(OutArgumentStorage arg)
        {
            return defs[arg.OriginalIdentifier.Storage.Domain].Expression;
        }

        public Expression VisitRegisterStorage(RegisterStorage reg)
        {
            CallBinding cb;
            return map.TryGetValue(reg.Domain, out cb)
                ? cb.Expression 
                : null;
        }

        public Expression VisitSequenceStorage(SequenceStorage seq)
        {
            throw new NotImplementedException();
        }

        public Expression VisitStackArgumentStorage(StackArgumentStorage stack)
        {
            int localOff = stack.StackOffset - site.StackDepthOnEntry;
            foreach (var de in this.map
                .Where(d => d.Value.Storage is StackStorage))
            {
                if (((StackStorage)de.Value.Storage).StackOffset == localOff)
                    return de.Value.Expression;
            }
            throw new NotImplementedException(string.Format("Argument {0} not found in map: {1}",
                stack,
                string.Join(",",map.Select(de => string.Format("{0}{1}: {2}{3}", "{", de.Key, de.Value, "}")))));
        }

        public Expression VisitStackLocalStorage(StackLocalStorage local)
        {
            throw new NotImplementedException();
        }

        public Expression VisitTemporaryStorage(TemporaryStorage temp)
        {
            throw new NotImplementedException();
        }
    }
}
