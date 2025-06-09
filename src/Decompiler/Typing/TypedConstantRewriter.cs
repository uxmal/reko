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
using Reko.Core.Expressions;
using Reko.Core.Loading;
using Reko.Core.Operators;
using Reko.Core.Types;
using Reko.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Reko.Typing
{
    /// <summary>
    /// Rewrites a constant based on its type.
    /// </summary>
    public class TypedConstantRewriter : IDataTypeVisitor<Expression>
	{
        private readonly Program program;
        private readonly ITypeStore store;
        private readonly IPlatform platform;
		private readonly Identifier globals;
        private readonly Dictionary<ushort, Identifier> mpSelectorToSegId;
        private readonly IDecompilerEventListener eventListener;
        private Constant? c;
        private Expression? basePtr;
		private PrimitiveType? pOrig;
        private DataType? dtResult;

        public TypedConstantRewriter(Program program, ITypeStore store, IDecompilerEventListener eventListener)
		{
            this.program = program;
            this.store = store;
            this.eventListener = eventListener;
            this.platform = program.Platform;
            this.globals = program.Globals;
            if (program.SegmentMap is not null)
            {
                this.mpSelectorToSegId = program.SegmentMap.Segments.Values
                    .Where(s => s.Identifier is not null && s.Address.Selector.HasValue)
                    .ToDictionary(s => s.Address.Selector!.Value, s => s.Identifier)!;
            }
            else
            {
                this.mpSelectorToSegId = new Dictionary<ushort, Identifier>();
            }
        }

        private bool Dereferenced => dtResult is not null;

        /// <summary>
        /// Rewrites a machine word constant depending on its data type.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="dereferenced"></param>
        /// <returns></returns>
        public Expression Rewrite(Constant c, Expression? basePtr, DataType? dtResult)
        {
            this.c = c;
            this.basePtr = basePtr;
            DataType dtInferred = c.DataType;
            if (dtInferred is null)
            {
                eventListener.Warn(
                    $"The equivalence class {store.GetTypeVariable(c).Name} has a null data type");
                dtInferred = store.GetDataTypeOf(c)!;
            }
            else
            {
                this.pOrig = (c.DataType as PrimitiveType)!;
                if (store.TryGetTypeVariable(c, out var tvConst))
                {
                    dtInferred = tvConst.DataType;
                    this.pOrig = (tvConst.OriginalDataType as PrimitiveType)!;
                }
            }
            var dt = dtInferred.ResolveAs<DataType>()!;
            this.dtResult = dtResult;
            return dt.Accept(this);
        }

        public Expression Rewrite(Address addr, Expression? basePtr, DataType? dtResult)
        {
            this.dtResult = dtResult;
            if (addr.Selector.HasValue)
            {
                if (!TryGetIdentifierForSelector(addr.Selector.Value, out Identifier? segId))
                {
                    eventListener.Warn(
                        "Selector {0:X4} has no known segment.",
                        addr.Selector.Value);
                    return addr;
                }
                var ptrSeg = store.GetTypeVariable(segId).DataType.ResolveAs<Pointer>();
                if (ptrSeg is null)
                {
                    //$TODO: what should the warning be?
                    //$BUG: create a fake field for now.
                    var field = new StructureField((int)addr.Offset, new UnknownType());
                    Expression x = new FieldAccess(new UnknownType(), new Dereference(segId.DataType, segId), field);
                    if (!Dereferenced)
                    {
                        x = new UnaryExpression(Operator.AddrOf, addr.DataType, x);
                    }
                    return x;
                }
                var baseType = ptrSeg.Pointee.ResolveAs<StructureType>()!;
                var dt = store.GetTypeVariable(addr).DataType.ResolveAs<Pointer>()!;
                this.c = Constant.Create(
                    PrimitiveType.CreateWord(addr.DataType.BitSize - ptrSeg.BitSize),
                    addr.Offset);
                DataType pointee = dt?.Pointee ?? new UnknownType();

                var f = EnsureFieldAtOffset(baseType, pointee, c.ToInt32());
                Expression ex = new FieldAccess(f.DataType, new Dereference(ptrSeg, segId), f);
                if (Dereferenced || pointee is ArrayType)
                {
                    return ex;
                }
                else
                {
                    var un = new UnaryExpression(Operator.AddrOf, store.GetTypeVariable(addr).DataType, ex);
                    return un;
                }
            }
            else
            {
                this.c = addr.ToConstant();
                var tvAddr = store.GetTypeVariable(addr);
                store.SetTypeVariable(this.c, tvAddr); //$REVIEW: iffy, we take over the type variable, is this ok?
                var dtInferred = tvAddr.DataType!.ResolveAs<DataType>()!;
                this.pOrig = tvAddr.OriginalDataType as PrimitiveType;
                return dtInferred.Accept(this);
            }
        }

        private bool TryGetIdentifierForSelector(ushort selector, [MaybeNullWhen(false)] out Identifier segId)
        {
            if (this.program.SegmentMap is not null &&
                this.program.SegmentMap.Selectors.TryGetValue(selector, out var segment))
            {
                segId = segment.Identifier;
                return true;
            }
            return mpSelectorToSegId.TryGetValue(selector, out segId);
        }

        private StructureType? GlobalVars
		{
            get
            {
                if (globals is not null && store.TryGetTypeVariable(globals, out var tvGlobals))
                {
                    if (tvGlobals.DataType is Pointer pGlob)
                    {
                        return pGlob.Pointee.ResolveAs<StructureType>();
                    }
                    if (globals.DataType is Pointer pGlob2)
                    {
                        return pGlob2.Pointee as StructureType;
                    }
                }
                return null;
            }
		}

        //$REVIEW: special cased code; we need to handle segments appropriately and remove this function.
        private static bool IsSegmentPointer(Pointer ptr)
        {
            if (ptr.Pointee is not EquivalenceClass eq)
                return false;
            if (eq.DataType is StructureType str)
                return str.IsSegment;
            else
                return false;
        }
        
        public Expression VisitArray(ArrayType at)
		{
			throw new ArgumentException("Constants cannot have array values yet.");
		}

        public Expression VisitClass(ClassType ct)
        {
            throw new NotImplementedException();
        }

        public Expression VisitCode(CodeType c)
        {
            throw new NotImplementedException();
        }

        public Expression VisitEnum(EnumType e)
        {
            var item = e.Members.FirstOrDefault(de => de.Value == c!.ToInt64());
            if (item.Key is not null)
                return new Identifier(item.Key, e, RegisterStorage.None);
            return new Cast(e, c!);
        }

        public Expression VisitEquivalenceClass(EquivalenceClass eq)
		{
			throw new NotImplementedException();
		}

		public Expression VisitFunctionType(FunctionType ft)
		{
			throw new NotImplementedException();
		}

		public Expression VisitMemberPointer(MemberPointer memptr)
		{
			// The constant is a member pointer.

			Pointer p = (Pointer) memptr.BasePointer;
			EquivalenceClass eq = (EquivalenceClass) p.Pointee;
			StructureType baseType = (StructureType) eq.DataType;
			Expression baseExpr = this.basePtr is not null
				? new Dereference(this.basePtr.DataType, this.basePtr)
                : (Expression) new ScopeResolution(baseType);

            var dt = memptr.Pointee.ResolveAs<DataType>()!;
            var f = EnsureFieldAtOffset(baseType, dt, (int)c!.ToUInt32());
            Expression ex = new FieldAccess(memptr.Pointee, baseExpr, f);
			if (Dereferenced)
			{
				ex.DataType = memptr.Pointee;
			}
			else
			{
                if (f.DataType is ArrayType array)
                {
                    ex.DataType = new MemberPointer(p, array.ElementType, platform.PointerType.BitSize);
                }
                else
                {
                    ex = new UnaryExpression(Operator.AddrOf, memptr, ex);
                }
            }
            return ex;
		}

		public Expression VisitPointer(Pointer ptr)
		{
			Expression e = c!;
            if (IsSegmentPointer(ptr))
            {
                if (mpSelectorToSegId.TryGetValue(c!.ToUInt16(), out Identifier? segID))
                    return segID;
                return e;
            } 
            else if (GlobalVars is not null)
            {
                // Null pointer.
                if (c!.IsZero)
                {
                    var np = program.Platform.MakeAddressFromConstant(c, false)!;
                    if (np is null)
                        return c;
                    var addrG = np.Value;
                    store.SetTypeVariable(np, store.GetTypeVariable(c));
                    addrG.DataType = c.DataType;
                    return addrG;
                }

                var addr = program.Platform.MakeAddressFromConstant(c, false);
                
                // An invalid pointer -- often used as sentinels in code.
                if (addr is null || !program.Memory.IsValidAddress(addr.Value))
                {
                    //$TODO: probably should emit a reinterpret_cast here.
                    e = new Cast(ptr, c);
                    store.SetTypeVariable(e, store.GetTypeVariable(c));
                    if (Dereferenced)
                    {
                        e = new Dereference(ptr.Pointee, e);
                    }
                    return e;
                }
                

                var dt = ptr.Pointee.ResolveAs<DataType>()!;
                var charType = MaybeCharType(dt);
                if (charType is not null && program.IsPtrToReadonlySection(addr.Value))
                {
                    PromoteToCString(c, charType);
                    if (!program.TryCreateImageReader(program.Architecture, addr.Value, out var rdr))
                        return e;
                    return rdr.ReadCString(charType, program.TextEncoding);
                }
                if (Dereferenced &&
                    TryReadReal(addr.Value, dt, out var cReal) &&
                    program.IsPtrToReadonlySection(addr.Value))
                {
                    return cReal;
                }
                e = RewriteGlobalFieldAccess(dt, c.ToInt32());
            }
			return e;
		}

        /// <summary>
        /// Drill into dt to see if it could be the beginning of a character string.
        /// </summary>
        public static PrimitiveType? MaybeCharType(DataType dt)
        {
            var pr = dt.ResolveAs<PrimitiveType>();
            if (pr is null)
            {
                if (dt is not ArrayType at)
                    return null;
                pr = at.ElementType.ResolveAs<PrimitiveType>();
                if (pr is null)
                    return null;
            }
            if (pr.Domain != Domain.Character)
                return null;
            return pr;
        }

        public Expression VisitReference(ReferenceTo refTo)
        {
            throw new NotImplementedException();
        }

        public DataType PromoteToCString(Constant c, DataType charType)
        {
            // Note that it's OK if there is no global field corresponding to a string constant.
            // It means that the string will be emitted "inline" in the code and not
            // as a separate global character array.
            var dt = StringType.NullTerminated(charType);
            var field = GlobalVars!.Fields.AtOffset(c.ToInt32());
            if (field is not null)
            {
                field.DataType = dt;
            }
            return dt;
        }

        private bool TryReadReal(
            Address addr,
            DataType dt,
            [MaybeNullWhen(false)] out Constant cReal)
        {
            cReal = null;
            var pt = dt.ResolveAs<PrimitiveType>();
            if (pt is null)
                return false;
            if (pt.Domain != Domain.Real)
                return false;
            // $BUGBUG: ImageReader throws NotImplementedException when
            // reading of 48-bit real at subjects/Raw/1750A/fff and
            // subjects/Raw/1750A/trigtst.
            // Not all floating point is IEEE 754. See
            // https://github.com/uxmal/reko/issues/1100 for details.
            if (pt.BitSize != 32 && pt.BitSize != 64)
                return false;
            return program.Architecture.TryRead(program.Memory, addr, pt, out cReal);
        }

        private static bool IsInsideField(int offset, StructureField field)
        {
            if (offset < field.Offset)
                return false;
            if (offset == field.Offset)
                return true;
            if (offset < field.Offset + field.DataType.Size)
                return true;
            var str = field.DataType.ResolveAs<StructureType>();
            if (str is not null && offset < field.Offset + str.GetInferredSize())
                return true;
            return false;
        }

        private Expression RewriteGlobalFieldAccess(
            DataType dt,
            int offset)
        {
            var f = GlobalVars!.Fields.LowerBound(offset);
            if (f is null || !IsInsideField(offset, f))
            {
                f = new StructureField(offset, dt);
                GlobalVars.Fields.Add(f);
            }
            if (Dereferenced || f.Offset != offset)
            {
                var ceb = new ComplexExpressionBuilder(
                    program, store, null, globals, null, offset);
                return ceb.BuildComplex(dtResult);
            }
            //$REVIEW: We can't use ComplexExpresionBuilder to rewrite pointers to
            // global variable. It's too aggressive now
            // (e.g. &globals->var.ptr00.ptr00 instead of &globals->var)
            var name = program.NamingPolicy.GlobalName(f);
            var globalVar = Identifier.Global(name, f.DataType);
            return CreateAddrOf(globalVar, dt);
        }

        private Expression CreateAddrOf(Expression e, DataType dt)
        {
            if (Dereferenced)
            {
                e.DataType = dt;
                return e;
            }
            if (e.DataType is ArrayType array) // C language rules 'promote' arrays to pointers.
            {
                e.DataType = program.TypeFactory.CreatePointer(
                    array.ElementType,
                    platform.PointerType.BitSize);
                return e;
            }
            var ptr = new Pointer(e.DataType, platform.PointerType.BitSize);
            return new UnaryExpression(Operator.AddrOf, ptr, e);
        }

        private StructureField EnsureFieldAtOffset(StructureType str, DataType dt, int offset)
        {
            StructureField? f = str.Fields.AtOffset(offset);
            if (f is null)
            {
                //$TODO: overlaps and conflicts.
                //$TODO: strings.
                f = new StructureField(offset, dt);
                str.Fields.Add(f);
            }
            return f;
#if TODO
            var f = GlobalVars.Fields.LowerBound(c.ToInt32());
            //StructureField f = str.Fields.AtOffset(offset);
            if (f is not null)
            {
                Unifier u = new Unifier();
                if (u.AreCompatible(f.DataType, dt))
                {
                    return f;
                }

                // Check for special case when an array ends at the offset.
                f = GlobalVars.Fields.LowerBound(c.ToInt32() - 1);
                var array = f.DataType.ResolveAs<ArrayType>();
                if (array is not null && u.AreCompatible(array.ElementType, dt))
                {
                    return f;
                }
            }
            //$TODO: overlaps and conflicts.
            //$TODO: strings.
            f = new StructureField(offset, dt);
            str.Fields.Add(f);
            return f;
#endif
        }

		public Expression VisitPrimitive(PrimitiveType pt)
		{
			if (pt.Domain == Domain.Real && (pOrig!.Domain & Domain.Integer) != 0)
			{
				return Constant.RealFromBitpattern(pt, c!);
			}
			else if (pt.Domain == Domain.Pointer)
            {
                var ptr = new Pointer(new UnknownType(), pt.BitSize);
                return VisitPointer(ptr);
            }
            else
            {
                return c!;
			}
		}

        public Expression VisitString(StringType str)
        {
            throw new NotImplementedException();
        }

		public Expression VisitStructure(StructureType str)
		{
            store.GetTypeVariable(c!).DataType = str;
            return c!;
		}

		public Expression VisitUnion(UnionType ut)
		{
			// A constant can't have a union value, so we coerce it to the appropriate type.
			UnionAlternative? a = ut.FindAlternative(pOrig!);
            if (a is null)
            {
                // This is encountered when the original type is a [[word]] but
                // the alternatives are, say, [[int32]] and [[uint32]]. In this case
                // we pick an arbitrary type and go with it.
                //$REVIEW: should emit a warning.
                a = ut.Alternatives.Values[0];
            }
			store.GetTypeVariable(c!).DataType = a.DataType;
            return c!;
		}

        public Expression VisitTypeReference(TypeReference typeref)
        {
            return typeref.Referent.Accept(this);
        }

		public Expression VisitTypeVariable(TypeVariable tv)
		{
			throw new NotImplementedException();
		}

        public Expression VisitUnknownType(UnknownType ut)
		{
            return c!;
		}

        public Expression VisitVoidType(VoidType vt)
        {
            throw new NotImplementedException();
        }
	}
}
