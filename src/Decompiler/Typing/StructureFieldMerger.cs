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

using Reko.Core;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Typing
{
    /// <summary>
    /// Merges structure fields that overlap.
    /// </summary>
    public class StructureFieldMerger
    {
        private StructureType str;

        public StructureType Merge(StructureType str)
        {
            this.str = str;
            StructureType strNew = new StructureType(str.Name, str.Size);
            strNew.IsSegment = str.IsSegment;
            foreach (List<StructureField> cluster in GetOverlappingClusters(str.Fields))
            {
                if (cluster.Count == 1)
                {
                    strNew.Fields.Add(cluster[0]);
                }
                else
                {
                    int offset = CommonOffset(cluster);
                    strNew.Fields.Add(new StructureField(offset, BuildOverlappedStructure(cluster)));
                }
            }
            return strNew;
        }

        public DataType BuildOverlappedStructure(List<StructureField> fields)
        {
            List<StructureType> types = new List<StructureType>();
            int commonOffset = CommonOffset(fields);
            StructureField field;
            WorkList<StructureField> worklist = new WorkList<StructureField>(fields);
            while (worklist.GetWorkItem(out field))
            {
                StructureType s = FindStructureToFitIn(field, commonOffset, types);
                if (s == null)
                {
                    s = new StructureType();
                    types.Add(s);
                }
                s.Fields.Add(new StructureField(field.Offset - commonOffset, field.DataType));
            }
            return Normalize(types);
        }

        private int CommonOffset(List<StructureField> fields)
        {
            int offset = fields[0].Offset;
            for (int i = 1; i < fields.Count; ++i)
            {
                offset = Math.Min(offset, fields[i].Offset);
            }
            return offset;
        }

        private DataType Normalize(List<StructureType> structs)
        {
            List<DataType> types = new List<DataType>();
            foreach (StructureType s in structs)
            {
                if (s.Fields.Count == 0)
                    continue;
                else
                    types.Add(s.Simplify());
            }
            if (types.Count == 1)
                return types[0];
            else
            {
                return new UnionType(
                    null,
                    null,
                    types.ToArray());
            }
        }

        private StructureType FindStructureToFitIn(StructureField field,int commonOffset, List<StructureType> types)
        {
            foreach (StructureType type in types)
            {
                int offset = field.Offset - commonOffset;
                StructureField low = type.Fields.LowerBound(offset);
                if (low == null)
                    return type;
                if (low.DataType.Size + low.Offset <= offset)
                    return type;
            }
            return null;
        }

        public IEnumerable<List<StructureField>> GetOverlappingClusters(StructureFieldCollection fields)
        {
            int clusterEndOffset = 0;
            List<StructureField> overlappingFields = new List<StructureField>();
            foreach (StructureField field in fields)
            {
                if (overlappingFields.Count == 0)
                {
                    clusterEndOffset = field.Offset + field.DataType.Size;
                }
                else if (FieldOverlaps(field, clusterEndOffset))
                {
                    clusterEndOffset = Math.Max(
                        clusterEndOffset,
                        field.Offset + field.DataType.Size);
                }
                else 
                {
                    yield return overlappingFields;
                    overlappingFields = new List<StructureField>();
                }
                //$REVIEW: what happens if a field has a user-given name?
                AddFieldToCluster(new StructureField(field.Offset,  field.DataType), overlappingFields);
            }
            if (overlappingFields.Count > 0)
                yield return overlappingFields;
        }

        private void AddFieldToCluster(StructureField field, List<StructureField> overlappingFields)
        {
            EquivalenceClass eq = field.DataType as EquivalenceClass;
            if (eq == null)
            {
                overlappingFields.Add(field);
                return;
            }
            UnionType u = eq.DataType as UnionType;
            if (u != null)
            {
                foreach (UnionAlternative alt in u.Alternatives.Values)
                {
                    StructureField f = new StructureField(field.Offset, alt.DataType);
                    overlappingFields.Add(f);
                }
            }
            else
            {
                overlappingFields.Add(field);
                return;
            }
        }

        private bool FieldOverlaps(StructureField field, int clusterEndOffset)
        {
            return field.Offset < clusterEndOffset;
        }
    }
}
