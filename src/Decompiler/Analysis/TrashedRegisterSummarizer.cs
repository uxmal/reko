#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Core.Lib;
using Reko.Core.Expressions;
using Reko.Evaluation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Analysis
{
    /// <summary>
    /// Computes a summary of the effects of this code on the processor registers.
    /// </summary>
    public class TrashedRegisterSummarizer
    {
        IProcessorArchitecture arch;
        Procedure proc;
        ProcedureFlow pf;
        SymbolicEvaluationContext ctx;
        private HashSet<RegisterStorage> trashed;
        private HashSet<RegisterStorage> preserved;
        private ExpressionValueComparer cmp;

        public TrashedRegisterSummarizer(Procedure proc, ProcedureFlow pf, SymbolicEvaluationContext ctx)
        {
            this.proc = proc;
            this.arch = proc.Architecture;
            this.pf = pf;
            trashed = new HashSet<RegisterStorage>();
            preserved = new HashSet<RegisterStorage>();
            this.ctx = ctx;
            this.cmp = new ExpressionValueComparer();
        }

        public bool PropagateToProcedureSummary()
        {
            bool changed = false;
            int oldCount;
            if (pf.TerminatesProcess)
            {
                if (pf.TrashedRegisters.Count > 0)
                {
                    changed = true;
                    pf.TrashedRegisters.Clear();
                }
                if (pf.grfTrashed != 0)
                {
                    changed = true;
                    pf.grfTrashed = 0;
                }
                if (pf.ConstantRegisters.Count > 0)
                {
                    pf.ConstantRegisters.Clear();
                    changed = true;
                }
                return changed;
            }
            foreach (var de in ctx.RegisterState)
            {
                var idValue = de.Value as Identifier;
                if (idValue != null)
                {
                    if (de.Key == idValue.Storage ||
                        (de.Key == arch.StackRegister && idValue == proc.Frame.FramePointer))
                    {
                        Preserve(de.Key);
                    }
                    else
                    {
                        Trash(de.Key);
                    }
                }
                else
                {
                    Trash(de.Key);
                    var cNew = de.Value as Constant;
                    if (cNew != null)
                    {
                        if (cNew.IsValid)
                        {
                            Constant cOld;
                            if (!pf.ConstantRegisters.TryGetValue(de.Key, out cOld))
                            {
                                changed = true;
                                pf.ConstantRegisters[de.Key] = cNew;
                            }
                            else if (!cmp.Equals(cOld, cNew))
                            {
                                changed = true;
                                SetConstant(de.Key, Constant.Invalid);
                            }
                        }
                        else
                        {
                            changed |= SetConstant(de.Key, Constant.Invalid);
                        }
                    }
                    else
                    {
                        changed |= SetConstant(de.Key, Constant.Invalid);
                    }
                }
            }

            oldCount = pf.TrashedRegisters.Count;
            pf.TrashedRegisters.UnionWith(trashed);
            if (pf.TrashedRegisters.Count != oldCount)
            {
                changed = true;
            }
            oldCount = pf.PreservedRegisters.Count;
            pf.PreservedRegisters.UnionWith(preserved);
            if (pf.PreservedRegisters.Count != oldCount)
            {
                changed = true;
            }
            uint grfNew = pf.grfTrashed | ctx.TrashedFlags;
            if (grfNew != pf.grfTrashed)
            {
                pf.grfTrashed = grfNew;
                changed = true;
            }
            return changed;
        }

        private bool SetConstant(Storage storage, Constant constant)
        {
            bool changed = false;
            var seq = storage as SequenceStorage;
            if (seq != null)
            {
                changed |= SetConstant(seq.Head, constant);
                changed |= SetConstant(seq.Tail, constant);
            }
            Constant old;
            if (!pf.ConstantRegisters.TryGetValue(storage, out old))
                old = null;
            pf.ConstantRegisters[storage] = constant;
            return changed | !cmp.Equals(old , constant);
        }

        private void Preserve(Storage s)
        {
            var reg = s as RegisterStorage;
            if (reg != null)
            {
                preserved.Add(reg);
                return;
            }
        }

        private void Trash(Storage s)
        {
            var reg = s as RegisterStorage;
            if (reg != null)
            {
                trashed.Add(reg);
                return;
            }
            var seq = s as SequenceStorage;
            if (seq != null)
            {
                Trash(seq.Head);
                Trash(seq.Tail);
            }
        }
    }
}
