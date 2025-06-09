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

using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Core.Expressions
{
    /// <summary>
    /// Deep-compares expressions; i.e. treats expressions as values.
    /// </summary>
    public class ExpressionValueComparer : IEqualityComparer<Expression>
    {
        private delegate bool EqualsFn(Expression x, Expression y);
        private delegate int HashFn(Expression obj);

        private static Dictionary<Type, EqualsFn> eqs = new Dictionary<Type, EqualsFn>();
        private static Dictionary<Type, HashFn> hashes = new Dictionary<Type, HashFn>();

        static ExpressionValueComparer()
        {
            Add(typeof(Application),
                (ea, eb) =>
                {
                    Application a = (Application) ea, b = (Application) eb;
                    if (a.Arguments.Length != b.Arguments.Length)
                        return false;
                    if (!EqualsImpl(a.Procedure, b.Procedure))
                        return false;
                    for (int i = 0; i != a.Arguments.Length; ++i)
                    {
                        if (!EqualsImpl(a.Arguments[i], b.Arguments[i]))
                            return false;
                    }
                    return true;
                },
                obj =>
                {
                    Application a = (Application) obj;
                    int h = GetHashCodeImpl(a.Procedure);
                    h ^= a.Arguments.Length;
                    foreach (Expression e in a.Arguments)
                    {
                        h *= 47;
                        if (e is not null)
                            h ^= GetHashCodeImpl(e);
                    }
                    return h;
                });
            Add(typeof(BinaryExpression),
                (ea, eb) =>
                {
                    BinaryExpression a = (BinaryExpression) ea, b = (BinaryExpression) eb;
                    if (a.Operator != b.Operator)
                        return false;
                    return (EqualsImpl(a.Left, b.Left) && EqualsImpl(a.Right, b.Right));
                },
                obj =>
                {
                    BinaryExpression b = (BinaryExpression) obj;
                    return b.Operator.GetHashCode() ^ GetHashCodeImpl(b.Left) ^ 47 * GetHashCodeImpl(b.Right);
                });
            Add(typeof(Cast),
                (ea, eb) =>
                {
                    Cast a = (Cast)ea, b = (Cast)eb;
                    return EqualsImpl(a.Expression, b.Expression);
                },
                obj =>
                {
                    Cast c = (Cast)obj;
                    return GetHashCodeImpl(c.Expression) * 43;
                });
            Add(typeof(Conversion),
                (ea, eb) =>
                {
                    Conversion a = (Conversion) ea, b = (Conversion) eb;
                    return EqualsImpl(a.Expression, b.Expression);
                },
                obj =>
                {
                    Conversion c = (Conversion) obj;
                    return GetHashCodeImpl(c.Expression) * 47;
                });
            Add(typeof(ConditionOf),
                (ea, eb) =>
                {
                    ConditionOf a = (ConditionOf) ea, b = (ConditionOf) eb;
                    return EqualsImpl(a.Expression, b.Expression);
                },
                obj =>
                {
                    return 0x10101010 * GetHashCodeImpl(((ConditionOf) obj).Expression);
                });
            Add(typeof(Address),
                addrComp,
                addrHash);
            //Add(typeof(Address16),
            //    addrComp,
            //    addrHash);
            //Add(typeof(Address32),
            //    addrComp,
            //    addrHash);
            //Add(typeof(Address64),
            //    addrComp,
            //    addrHash);
            //Add(typeof(ProtectedSegmentedAddress),
            //    addrComp,
            //    addrHash);
            //Add(typeof(RealSegmentedAddress),
            //    addrComp,
            //    addrHash);

            Add(typeof(ConditionalExpression),
                (ca, cb) =>
                {
                    var a = (ConditionalExpression)ca;
                    var b = (ConditionalExpression)cb;
                    return EqualsImpl(a.Condition, b.Condition) &&
                           EqualsImpl(a.ThenExp, b.ThenExp) &&
                           EqualsImpl(a.FalseExp, b.FalseExp);
                },
                obj =>
                {
                    var self = (ConditionalExpression)obj;
                    return GetHashCodeImpl(self.Condition) ^
                           GetHashCodeImpl(self.ThenExp) * 87 ^
                           GetHashCodeImpl(self.FalseExp) * 33;
                });

            Add(typeof(Constant),
                (ea, eb) =>
                {
                    Constant a = (Constant) ea, b = (Constant) eb;
                    if (!a.IsValid || !b.IsValid)
                        return false;
                    if (a.IsReal)
                    {
                        if (b.IsReal)
                        {
                            return a.ToReal64() == b.ToReal64();
                        }
                        else
                        {
                            return false;
                        }
                    }
                    if (a is BigConstant ba)
                    {
                        return ba.Value == b.ToBigInteger();
                    }
                    else if (b is BigConstant bb)
                    {
                        return a.ToBigInteger() == bb.ToBigInteger();
                    }
                    else
                    {
                        return object.Equals(a.ToUInt64(), b.ToUInt64());
                    }
                },
                obj =>
                {
                    return ((Constant)obj).ToUInt64().GetHashCode();
                });

            Add(typeof(Dereference),
                (ea, eb) =>
                {
                    Dereference a = (Dereference) ea, b = (Dereference) eb;
                    return EqualsImpl(a.Expression, b.Expression);
                },
                obj =>
                {
                    return GetHashCodeImpl(((Dereference) obj).Expression) * 129;
                });
            Add(typeof(FieldAccess),
                (a, b) =>
                {
                    var fa = (FieldAccess)a;
                    var fb = (FieldAccess)b;
                    return EqualsImpl(fa.Structure, fb.Structure) &&
                        fa.Field == fb.Field;
                },
                obj =>
                {
                    var f = (FieldAccess)obj;
                    return GetHashCodeImpl(f.Structure) * 23 ^
                        f.Field.Name.GetHashCode();
                });
            Add(
                typeof(Identifier),
                delegate(Expression x, Expression y)
                {
                    return ((Identifier) x).Name == ((Identifier) y).Name;
                },
                delegate(Expression x)
                {
                    return ((Identifier) x).Name.GetHashCode();
                });
            Add(typeof(MemberPointerSelector),
                (a, b) =>
                {
                    var mpsA = (MemberPointerSelector)a;
                    var mpsB = (MemberPointerSelector)b;
                    return
                        EqualsImpl(mpsA.BasePointer, mpsB.BasePointer) &&
                        EqualsImpl(mpsA.MemberPointer, mpsB.MemberPointer);
                },
                obj =>
                {
                    var mps = (MemberPointerSelector)obj;
                    return GetHashCodeImpl(mps.BasePointer) * 29 ^
                        GetHashCodeImpl(mps.MemberPointer);
                });
            Add(typeof(MemoryAccess),
                (ea, eb) =>
                {
                    MemoryAccess a = (MemoryAccess) ea, b = (MemoryAccess) eb;
                    return EqualsImpl(a.MemoryId, b.MemoryId) &&
                        a.DataType == b.DataType &&
                        EqualsImpl(a.EffectiveAddress, b.EffectiveAddress);
                },
                obj =>
                {
                    MemoryAccess m = (MemoryAccess) obj;
                    return GetHashCodeImpl(m.MemoryId) ^ m.DataType.GetHashCode() ^ 47 * GetHashCodeImpl(m.EffectiveAddress);
                });

            Add(typeof(MkSequence),
                (ea, eb) =>
                {
                    var a = (MkSequence)ea;
                    var b = (MkSequence)eb;
                    if (a.Expressions.Length != b.Expressions.Length)
                        return false;
                    for (int i = 0; i < a.Expressions.Length; ++i)
                    {
                        if (!EqualsImpl(a.Expressions[i], b.Expressions[i]))
                            return false;
                    }
                    return true;
                },
                obj =>
                {
                    var s = (MkSequence)obj;
                    int h = obj.GetType().GetHashCode();
                    foreach (var e in s.Expressions)
                    {
                        h = 32 * h ^ GetHashCodeImpl(e);
                    }
                    return h;
                });
            Add(typeof(OutArgument),
                (ea, eb) =>
                {
                    var a = (OutArgument) ea;
                    var b = (OutArgument) eb;
                    return EqualsImpl(a.Expression, b.Expression);
                },
                obj =>
                {
                    var oa = (OutArgument) obj;
                    return GetHashCodeImpl(oa.Expression);
                });

            Add(typeof(PhiFunction),
                (ea, eb) =>
                {
                    PhiFunction a = (PhiFunction) ea, b = (PhiFunction) eb;
                    if (a.Arguments.Length != b.Arguments.Length)
                        return false;
                    for (int i = 0; i < a.Arguments.Length; ++i)
                    {
                        if (!EqualsImpl(a.Arguments[i].Value, b.Arguments[i].Value))
                            return false;
                    }
                    return true;
                },
                obj =>
                {
                    PhiFunction phi = (PhiFunction) obj;
                    int h = phi.Arguments.Length.GetHashCode();
                    foreach (var arg in phi.Arguments)
                    {
                        // Order of parameters cannot be guaranteed,
                        // so we must form the hash code in an order-
                        // independent fashion.
                        h ^= GetHashCodeImpl(arg.Value);
                    }
                    return h;
                });

            Add(typeof(ProcedureConstant),
                (ea, eb) =>
                {
                    ProcedureConstant a = (ProcedureConstant) ea, b = (ProcedureConstant) eb;
                    return a.Procedure == b.Procedure;
                },
                obj =>
                {
                    return ((ProcedureConstant) obj).GetHashCode();
                });

            Add(typeof(ScopeResolution),
                (sa, sb) =>
                {
                    var a = (ScopeResolution) sa;
                    var b = (ScopeResolution) sb;
                    return a.DataType.ToString() == b.DataType.ToString();
                },
                obj =>
                {
                    return ((ScopeResolution) obj).DataType.ToString().GetHashCode();
                });
            Add(typeof(SegmentedPointer),
                (ea, eb) =>
                {
                    SegmentedPointer a = (SegmentedPointer) ea, b = (SegmentedPointer) eb;
                    return
                        EqualsImpl(a.BasePointer, b.BasePointer) &&
                        a.DataType == b.DataType &&
                        EqualsImpl(a.Offset, b.Offset);
                },
                obj =>
                {
                    SegmentedPointer m = (SegmentedPointer) obj;
                    return 
                        m.DataType.GetHashCode() ^
                        47 * GetHashCodeImpl(m.BasePointer) ^
                        GetHashCodeImpl(m.Offset);
                });

            Add(typeof(ArrayAccess),
                (ea, eb) =>
                {
                    ArrayAccess a = (ArrayAccess)ea, b = (ArrayAccess)eb;
                    return
                        EqualsImpl(a.Array, b.Array) &&
                        EqualsImpl(a.Index, b.Index) &&
                        a.DataType == b.DataType;
                },
                obj =>
                {
                    ArrayAccess m = (ArrayAccess)obj;
                    return GetHashCodeImpl(m.Array) ^
                        m.DataType.GetHashCode() ^
                        47 * GetHashCodeImpl(m.Index);
                });

            Add(typeof(Slice),
                (ea, eb) =>
                {
                    Slice a = (Slice) ea, b = (Slice) eb;
                    return EqualsImpl(a.Expression, b.Expression) &&
                        a.Offset == b.Offset && a.DataType == b.DataType;
                },
                obj =>
                {
                    Slice s = (Slice) obj;
                    return GetHashCodeImpl(s.Expression) ^ s.Offset * 47 ^ s.DataType.GetHashCode() * 23;
                });

            Add(typeof(StringConstant),
                (sa, sb) =>
                {
                    var a = (StringConstant) sa;
                    var b = (StringConstant) sb;
                    return a.DataType == b.DataType;
                },
                obj =>
                {
                    var s = (StringConstant) obj;
                    return s.ToString().GetHashCode();
                });
            Add(
                typeof(TestCondition),
                delegate(Expression x, Expression y)
                {
                    TestCondition tx = (TestCondition) x, ty = (TestCondition) y; 
                    return Equals(tx.ConditionCode, ty.ConditionCode) && EqualsImpl(tx.Expression, ty.Expression);
                },
                delegate(Expression x)
                {
                    TestCondition tx = (TestCondition) x;
                    return tx.ConditionCode.GetHashCode() ^ GetHashCodeImpl(tx.Expression) & 47;
                });

            Add(typeof (UnaryExpression),
                delegate(Expression x, Expression y)
                {
                    UnaryExpression a = (UnaryExpression) x, b = (UnaryExpression) y;
                    return a.Operator == b.Operator && 
                        EqualsImpl(a.Expression, b.Expression);
                },
                obj =>
                {
                    UnaryExpression u = (UnaryExpression) obj;
                    return GetHashCodeImpl(u.Expression) ^ u.Operator.GetHashCode();
                });
        }

        private static bool addrComp(Expression ea, Expression eb)
        {
            Address a = (Address)ea, b = (Address)eb;
            return a.ToLinear() == b.ToLinear();
        }

        private static int addrHash(Expression obj)
        {
            return ((Address)obj).ToLinear().GetHashCode();
        }

        private static void Add(Type t, EqualsFn eq, HashFn hash)
        {
            eqs.Add(t, eq);
            hashes.Add(t, hash);
        }

        #region IEqualityComparer Members

        /// <inheritdoc/>
        public bool Equals(Expression? x, Expression? y)
        {
            if (x is null && y is null)
                return true;
            if (x is null || y is null)
                return false;
            return EqualsImpl(x, y);
        }

        private static bool EqualsImpl(Expression x, Expression y)
        {
            Type tx = x.GetType();
            if (typeof(Constant).IsAssignableFrom(tx))
                tx = typeof(Constant);
            Type ty = y.GetType();
            if (typeof(Constant).IsAssignableFrom(ty))
                ty = typeof(Constant);
            if (tx != ty)
                return false;

            if (!eqs.TryGetValue(tx, out var eqFn))
                throw new NotImplementedException($"No equality implemented for {tx.Name}");
            return eqFn(x, y);
        }

        private static int GetHashCodeImpl(Expression obj)
        {
            var tc = typeof(Constant);
            Type t = obj.GetType();
            if (tc.IsAssignableFrom(t))
                t = tc;
            if (!hashes.ContainsKey(t))
                throw new NotImplementedException($"No hashing implemented for {t.Name}");
            return hashes[t](obj);
        }

        /// <inheritdoc/>
        public int GetHashCode(Expression obj)
        {
            return GetHashCodeImpl(obj);
        }
        #endregion
    }
}
