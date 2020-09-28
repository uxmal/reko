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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.LLVM
{
    public partial class LLVMParser
    {
        private Instruction ParseAlloca(LocalId result)
        {
            Expect(TokenType.alloca);
            LLVMType type = ParseType();
            LLVMType elcType = null;
            Value elc = null;
            int alignment = 0;
            while (PeekAndDiscard(TokenType.COMMA))
            {
                if (Type_FIRST.Contains(Peek().Type))
                {
                    elcType = ParseType();
                    elc = ParseValue();
                }
                else if (PeekAndDiscard(TokenType.align))
                {
                    alignment = int.Parse(Expect(TokenType.Integer));
                }
            }
            return new Alloca
            {
                Result = result,
                Type = type,
                ElCountType = elcType,
                ElementCount = elc,
                Alignment = alignment,
            };
        }

        private Instruction ParseBinBitOp(LocalId result)
        {
            var op = Get().Type;
            var nuw = PeekAndDiscard(TokenType.nuw);
            var nsw = PeekAndDiscard(TokenType.nsw);
            var type = ParseType();
            var op1 = ParseValue();
            Expect(TokenType.COMMA);
            var op2 = ParseValue();
            return new BitwiseBinary
            {
                Result = result,
                Operator = op,
                NoUnsignedWrap = nuw,
                NoSignedWrap = nsw,
                Type = type,
                Left = op1,
                Right = op2,
            };
        }

        private Instruction ParseBinOp(LocalId result)
        {
            var op = Get().Type;
            var nuw = PeekAndDiscard(TokenType.nuw);
            var nsw = PeekAndDiscard(TokenType.nsw);
            var exa = PeekAndDiscard(TokenType.exact);
            var type = ParseType();
            var op1 = ParseValue();
            Expect(TokenType.COMMA);
            var op2 = ParseValue();
            return new Binary
            {
                Result = result,
                Operator = op,
                NoUnsignedWrap = nuw,
                NoSignedWrap = nsw,
                Type = type,
                Left = op1,
                Right = op2,
            };
        }

        private Instruction ParseConversion(LocalId result)
        {
            var op = Get().Type;
            var typeFrom = ParseType();
            var value = ParseValue();
            Expect(TokenType.to);
            var typeTo = ParseType();
            return new Conversion
            {
                Operator = op,
                Result = result,
                TypeFrom = typeFrom,
                Value = value,
                TypeTo = typeTo,
            };
        }

        private Instruction ParseCall(LocalId result)
        {
            //$TODO: tail
            Expect(TokenType.call);
            var attrs = ParseParameterAttributes();
            var ret = ParseType();
            var fnPtr = ParseValue();
            var args = ParseArgumentList();
            ParseFunctionAttributes();
            return new LLVMCall
            {
                Result = result,
                res_attrs = attrs,
                FnType = ret,
                FnPtr = fnPtr,
                Arguments = args,
            };
        }

        private Instruction ParseExtractvalue(LocalId result)
        {
            Expect(TokenType.extractvalue);
            var aggregateType = ParseType();
            var val = ParseValue();
            var indices = new List<int>();
            Expect(TokenType.COMMA);
            do
            {
                var idx = Convert.ToInt32(ParseInteger().Value);
                indices.Add(idx);
            } while (PeekAndDiscard(TokenType.COMMA));
            return new Extractvalue
            {
                Result = result,
                AggregateType = aggregateType,
                Value = val,
                Indices = indices,
            };
        }

      

        private Instruction ParseGetElementPtr(LocalId result)
        {
            Expect(TokenType.getelementptr);
            PeekAndDiscard(TokenType.inbounds);	//$REVIEW: use this?
            var baseType = ParseType();
            Expect(TokenType.COMMA);
            var ptrType = ParseType();
            var ptrVal = ParseValue();
            var indices = new List<Tuple<LLVMType, Value>>();
            while (PeekAndDiscard(TokenType.COMMA))
            {
                PeekAndDiscard(TokenType.inrange);  //$REVIEW: use this?
                var type = ParseType();
                var idxVal = ParseValue();
                indices.Add(Tuple.Create(type, idxVal));
            }
            return new GetElementPtr
            {
                Result = result,
                BaseType = baseType,
                PtrType = ptrType,
                PtrValue = ptrVal,
                Indices = indices,
            };
        }

        private static HashSet<TokenType> conditionCodes = new HashSet<TokenType>
        {
           TokenType.eq,
           TokenType.ne,
           TokenType.ugt,
           TokenType.uge,
           TokenType.ult,
           TokenType.ule,
           TokenType.sgt,
           TokenType.sge,
           TokenType.slt,
           TokenType.sle,
        };

        private static HashSet<TokenType> fconditionCodes = new HashSet<TokenType>
        {
            TokenType.oeq,
            TokenType.ogt,
            TokenType.oge,
            TokenType.olt,
            TokenType.ole,
            TokenType.one,
            TokenType.ord,
            TokenType.ueq,
            TokenType.ugt,
            TokenType.uge,
            TokenType.ult,
            TokenType.ule,
            TokenType.une,
            TokenType.uno,
        };

        private Instruction ParseIcmp(LocalId result)
        {
            Expect(TokenType.icmp);
            var cc = Get();
            if (!conditionCodes.Contains(cc.Type))
                Unexpected(cc);
            var type = ParseType();
            var op1 = ParseValue();
            Expect(TokenType.COMMA);
            var op2 = ParseValue();
            return new CmpInstruction
            {
                Result = result,
                Operator = TokenType.icmp,
                ConditionCode = cc.Type,
                Type = type,
                Op1 = op1,
                Op2 = op2,
            };
        }

        private Instruction ParseFcmp(LocalId result)
        {
            Expect(TokenType.fcmp);
            var cc = Get();
            if (!fconditionCodes.Contains(cc.Type))
                Unexpected(cc);
            var type = ParseType();
            var op1 = ParseValue();
            Expect(TokenType.COMMA);
            var op2 = ParseValue();
            return new CmpInstruction
            {
                Result = result,
                Operator = TokenType.fcmp,
                ConditionCode = cc.Type,
                Type = type,
                Op1 = op1,
                Op2 = op2,
            };
        }

        private Instruction ParseFence()
        {
            Expect(TokenType.fence);
            Expect(TokenType.seq_cst);
            return new Fence { Type = TokenType.seq_cst };
        }

        private Instruction ParseLoad(LocalId result)
        {
            Expect(TokenType.load);
            if (PeekAndDiscard(TokenType.@volatile))
            {
            }
            var dstType = ParseType();
            Expect(TokenType.COMMA);

            var srcType = ParseType();
            var src = ParseValue();
            int alignment = 0;
            if (PeekAndDiscard(TokenType.COMMA))
            {
                Expect(TokenType.align);
                alignment = Convert.ToInt32(ParseInteger().Value);
            }
            return new Load
            {
                DstType = dstType,
                Dst = result,
                SrcType = srcType,
                Src = src,
                Alignment = alignment
            };
        }

        private Instruction ParseRet()
        {
            Expect(TokenType.ret);
            var type = ParseType();
            Value val = null;
            if (type != LLVMType.Void)
            {
                val = ParseValue();
            }
            return new RetInstr
            {
                Type = type,
                Value = val,
            };
        }

        private Instruction ParseStore()
        {
            Expect(TokenType.store);
            bool @volatile = false;
            int alignment = 0;
            if (PeekAndDiscard(TokenType.@volatile))
            {
                @volatile = true;
            }
            var srcType = ParseType();
            var src = ParseValue();
            Expect(TokenType.COMMA);
            var dstType = ParseType();
            var dst = ParseValue();
            if (PeekAndDiscard(TokenType.COMMA))
            {
                if (PeekAndDiscard(TokenType.align))
                {
                    alignment = Convert.ToInt32(ParseInteger().Value);
                }
            }
            return new Store
            {
                Dst = dst,
                DstType = dstType,
                Src = src,
                SrcType = srcType,
                Volatile = @volatile,
                Alignment = alignment
            };
        }

        private Terminator ParseSwitch()
        {
            Expect(TokenType.@switch);
            var type = ParseType();
            var val = ParseValue();
            Expect(TokenType.COMMA);
            Expect(TokenType.label);
            var defDest = ParseLocalId();
            Expect(TokenType.LBRACKET);
            var destinations = new List<Tuple<LLVMType, Value, LocalId>>();
            while (!PeekAndDiscard(TokenType.RBRACKET))
            {
                var caseType = ParseType();
                var caseVal = ParseValue();
                Expect(TokenType.COMMA);
                Expect(TokenType.label);
                var dest = ParseLocalId();
                destinations.Add(Tuple.Create(caseType, caseVal, dest));
            }
            return new Switch
            {
                Type = type,
                Value = val,
                Default = defDest,
                Destinations = destinations
            };
        }

        private Terminator ParseUnreachable()
        {
            Expect(TokenType.unreachable);
            return new Unreachable();
        }

        private PhiInstruction ParsePhi(LocalId result)
        {
            Expect(TokenType.phi);
            //sExp = "%15 = phi i1 [false, %5], [%13, %8]";
            var type = ParseType();
            var args = new List<Tuple<Value, LocalId>>();
            Expect(TokenType.LBRACKET);
            var value = ParseValue();
            Expect(TokenType.COMMA);
            var label = ParseLocalId();
            Expect(TokenType.RBRACKET);
            var arg = Tuple.Create(value, label);
            args.Add(arg);
            while (PeekAndDiscard(TokenType.COMMA))
            {
                Expect(TokenType.LBRACKET);
                value = ParseValue();
                Expect(TokenType.COMMA);
                label = ParseLocalId();
                Expect(TokenType.RBRACKET);
                arg = Tuple.Create(value, label);
                args.Add(arg);
            }
            return new PhiInstruction
            {
                Result = result,
                Type = type,
                Arguments = args,
            };
        }

        private Select ParseSelect(LocalId result)
        {
            Expect(TokenType.select);
            var condType = ParseType();
            var cond = ParseValue();
            Expect(TokenType.COMMA);
            var trueType = ParseType();
            var trueValue = ParseValue();
            Expect(TokenType.COMMA);
            var falseType = ParseType();
            var falseValue = ParseValue();
            return new Select
            {
                Result = result,
                CondType = condType,
                Cond = cond,
                TrueType = trueType,
                TrueValue = trueValue,
                FalseType = falseType,
                FalseValue = falseValue,
            };
        }
    }
}
