/* 
 * Copyright (C) 1999-2009 John Källén.
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

using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Core.Code
{
    /// <summary>
    /// 
    /// </summary>
    public class ExpressionValueComparer : IEqualityComparer<Expression>
    {
        private delegate bool EqualsFn(Expression x, Expression y);
        private delegate int HashFn(Expression obj);

        private Dictionary<Type, EqualsFn> eqs = new Dictionary<Type, EqualsFn>();
        private Dictionary<Type, HashFn> hashes = new Dictionary<Type, HashFn>();
        public ExpressionValueComparer()
        {
            Add(typeof(Application),
                delegate(Expression ea, Expression eb)
                {
                    Application a = (Application) ea, b = (Application) eb;
                    if (a.Arguments.Length != b.Arguments.Length)
                        return false;
                    if (!Equals(a.Procedure, b.Procedure))
                        return false;
                    for (int i = 0; i != a.Arguments.Length; ++i)
                    {
                        if (!Equals(a.Arguments[i], b.Arguments[i]))
                            return false;
                    }
                    return true;
                },
                delegate(Expression obj)
                {
                    Application a = (Application) obj;
                    int h = GetHashCode(a.Procedure);
                    h ^= a.Arguments.Length;
                    foreach (Expression e in a.Arguments)
                    {
                        h *= 47;
                        if (e != null)
                            h ^= GetHashCode(e);
                    }
                    return h;
                });
            Add(typeof(BinaryExpression),
                delegate(Expression ea, Expression eb)
                {
                    BinaryExpression a = (BinaryExpression) ea, b = (BinaryExpression) eb;
                    if (a.op != b.op)
                        return false;
                    return (Equals(a.Left, b.Left) && Equals(a.Left, b.Right));
                },
                delegate(Expression obj)
                {
                    BinaryExpression b = (BinaryExpression) obj;
                    return b.op.GetHashCode() ^ GetHashCode(b.Left) ^ 47 * GetHashCode(b.Right);
                });

            Add(typeof(ConditionOf),
                delegate(Expression ea, Expression eb)
                {
                    ConditionOf a = (ConditionOf) ea, b = (ConditionOf) eb;
                    return Equals(a.Expression, b.Expression);
                },
                delegate(Expression obj)
                {
                    return 0x10101010 * GetHashCode(((ConditionOf) obj).Expression);
                });

            Add(typeof(Constant),
                delegate(Expression ea, Expression eb)
                {
                    Constant a = (Constant) ea, b = (Constant) eb;
                    return object.Equals(a.ToUInt64(), b.ToUInt64());
                },
                delegate(Expression obj)
                {
                    return ((Constant)obj).ToUInt64().GetHashCode();
                });

            Add(typeof(DepositBits),
                delegate(Expression ea, Expression eb)
                {
                    DepositBits a = (DepositBits) ea, b = (DepositBits) eb;
                    return a.BitCount == b.BitCount && a.BitPosition == b.BitPosition &&
                        Equals(a.Source, b.Source) && Equals(a.InsertedBits, b.InsertedBits);
                },
                delegate(Expression obj)
                {
                    DepositBits dpb = (DepositBits) obj;
                    return GetHashCode(dpb.Source) * 67 ^ GetHashCode(dpb.InsertedBits) * 43 ^ dpb.BitPosition * 7 ^ dpb.BitCount;
                });

            Add(typeof(Dereference),
                delegate(Expression ea, Expression eb)
                {
                    Dereference a = (Dereference) ea, b = (Dereference) eb;
                    return Equals(a.Expression, b.Expression);
                },
                delegate(Expression obj)
                {
                    return GetHashCode(((Dereference) obj).Expression) * 129;
                });



            Add(
                typeof(Identifier),
                delegate(Expression x, Expression y)
                {
                    return ((Identifier) x).Number == ((Identifier) y).Number;
                },
                delegate(Expression x)
                {
                    return ((Identifier) x).Number.GetHashCode();
                });

            Add(typeof(MemoryAccess),
                delegate(Expression ea, Expression eb)
                {
                    MemoryAccess a = (MemoryAccess) ea, b = (MemoryAccess) eb;
                    return Equals(a.MemoryId, b.MemoryId) &&
                        a.DataType == b.DataType &&
                        Equals(a.EffectiveAddress, b.EffectiveAddress);
                },
                delegate(Expression obj)
                {
                    MemoryAccess m = (MemoryAccess) obj;
                    return GetHashCode(m.MemoryId) ^ m.DataType.GetHashCode() ^ 47 * GetHashCode(m.EffectiveAddress);
                });


            Add(typeof(PhiFunction),
                delegate(Expression ea, Expression eb)
                {
                    PhiFunction a = (PhiFunction) ea, b = (PhiFunction) eb;
                    if (a.Arguments.Length != b.Arguments.Length)
                        return false;
                    for (int i = 0; i != a.Arguments.Length; ++i)
                    {
                        if (!Equals(a.Arguments[i], b.Arguments[i]))
                            return false;
                    }
                    return true;
                },
                delegate(Expression obj)
                {
                    PhiFunction phi = (PhiFunction) obj;
                    int h = phi.Arguments.Length.GetHashCode();
                    for (int i = 0; i < phi.Arguments.Length; ++i)
                    {
                        h = h * 47 ^ GetHashCode(phi.Arguments[i]);
                    }
                    return h;
                });

            Add(typeof(SegmentedAccess),
                delegate(Expression ea, Expression eb)
                {
                    SegmentedAccess a = (SegmentedAccess) ea, b = (SegmentedAccess) eb;
                    return
                        Equals(a.BasePointer, b.BasePointer) &&
                        Equals(a.MemoryId, b.MemoryId) &&
                        a.DataType == b.DataType &&
                        Equals(a.EffectiveAddress, b.EffectiveAddress);
                },
                delegate(Expression obj)
                {
                    SegmentedAccess m = (SegmentedAccess) obj;
                    return GetHashCode(m.MemoryId) ^
                        m.DataType.GetHashCode() ^
                        47 * GetHashCode(m.EffectiveAddress) ^
                        GetHashCode(m.BasePointer);
                });

            Add(typeof(Slice),
                delegate(Expression ea, Expression eb)
                {
                    Slice a = (Slice) ea, b = (Slice) eb;
                    return Equals(a.Expression, b.Expression) &&
                        a.Offset == b.Offset && a.DataType == b.DataType;
                },
                delegate(Expression obj)
                {
                    Slice s = (Slice) obj;
                    return GetHashCode(s.Expression) ^ s.Offset * 47 ^ s.DataType.GetHashCode() * 23;
                }); 


            Add(
                typeof(TestCondition),
                delegate(Expression x, Expression y)
                {
                    TestCondition tx = (TestCondition) x, ty = (TestCondition) y; 
                    return Equals(tx.ConditionCode, ty.ConditionCode) && Equals(tx.Expression, ty.Expression);
                },
                delegate(Expression x)
                {
                    TestCondition tx = (TestCondition) x;
                    return tx.ConditionCode.GetHashCode() ^ GetHashCode(tx.Expression) & 47;
                });

            Add(typeof (UnaryExpression),
                delegate(Expression x, Expression y)
                {
                    UnaryExpression a = (UnaryExpression) x, b = (UnaryExpression) y;
                    return a.op == b.op && 
                        Equals(a.Expression, b.Expression);
                },
                delegate(Expression obj)
                {
                    UnaryExpression u = (UnaryExpression) obj;
                    return GetHashCode(u.Expression) ^ u.op.GetHashCode();
                });
        }

        private void Add(Type t, EqualsFn eq, HashFn hash)
        {
            eqs.Add(t, eq);
            hashes.Add(t, hash);
        }

        #region IEqualityComparer Members

        public bool Equals(Expression x, Expression y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;

            Type tx = x.GetType();
            Type ty = y.GetType();
            if (tx != ty)
                return false;

            return eqs[tx](x, y);
        }

        public int GetHashCode(Expression obj)
        {
            if (obj == null)
                throw new ArgumentNullException();
            return hashes[obj.GetType()](obj);
        }
        #endregion
    }
}
