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

namespace Reko.Core.Serialization
{
    /// <summary>
    /// Utility base class principally used for dispatching.
    /// </summary>
    public abstract class SerializedProject
    {
        public abstract T Accept<T>(ISerializedProjectVisitor<T> visitor);
    }

    public interface ISerializedProjectVisitor<T>
    {
        T VisitProject_v2(Project_v2 sProject);
        T VisitProject_v3(Project_v3 sProject);
        T VisitProject_v4(Project_v4 sProject);
        T VisitProject_v5(Project_v5 sProject);
    }
}
