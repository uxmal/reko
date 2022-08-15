#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

using Reko.Gui.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Gui
{
    /// <summary>
    /// Base class for designers that wrap a "tree node".
    /// </summary>
    public class TreeNodeDesigner : ICommandTarget
    {
        public IServiceProvider Services { get; set; } = default!;
        public ITreeNode? TreeNode { get; set; }
        public ITreeNodeDesignerHost? Host { get ; set; }
        public object? Component { get; set; }
        public TreeNodeDesigner? Parent { get; set; }

        public virtual void Initialize(object obj)
        {
            TreeNode!.Text = obj.ToString()!;
        }

        public virtual void DoDefaultAction()
        {
        }

        public virtual void OnExpanded()
        {

        }

        public virtual bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            return false;
        }

        public virtual ValueTask<bool> ExecuteAsync(CommandID cmdId)
        {
            return ValueTask.FromResult(false);
        }
    }

    public interface ITreeNodeDesignerHost
    {
        /// <summary>
        /// Adds a range of components to the tree designer host.
        /// </summary>
        /// <param name="components"></param>
        void AddComponents(System.Collections.IEnumerable components);

        /// <summary>
        /// Adds a new <paramref name="component" /> as a child of
        /// <paramref name="parent"/>.
        /// </summary>
        /// <param name="parent">Parent component, or null if the component is
        /// a root item.</param>
        /// <param name="component">Component to add</param>
        void AddComponent(object? parent, object component);
        void AddComponents(object? parent, System.Collections.IEnumerable components);
        void RemoveComponent(object component);
        TreeNodeDesigner? GetDesigner(object component);
        TreeNodeDesigner? GetSelectedDesigner();
    }

    public static class TreeNodeDesignerEx
    {
        public static T GetAncestorOfType<T>(this ITreeNodeDesignerHost host, object component)
        {
            var des = host.GetDesigner(component);
            if (des is null)
                return default!;
            for (;;)
            {
                des = des.Parent;
                if (des is null)
                    return default!;
                if (des.Component is T t)
                    return t;
            }
        }
    }
}
