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
using Reko.Gui;
using Reko.Gui.Forms;
using System;
using System.ComponentModel.Design;
using System.Linq;

namespace Reko.Gui.Forms
{
    public class CallHierarchyInteractor : ICallHierarchyService, ICommandTarget, IWindowPane
    {
        private ICallHierarchyView view;
        private ITreeNodeDesignerHost host;

        public CallHierarchyInteractor(ICallHierarchyView view)
        {
            this.view = view;
            this.view.DeleteButton.Click += DeleteButton_Click;
        }

        public IServiceProvider Services { get; private set; }

        public IWindowFrame Frame { get; set; }

        public ITreeNodeDesignerHost Host { get { return host ?? new TreeNodeDesignerHost(view.CallTree, Services); } }

        public void Close()
        {
            this.view = null;
        }

        public object CreateControl()
        {
            view.Services = this.Services;
            return view;
        }

        public bool Execute(CommandID cmdId)
        {
            return false;
        }

        public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            return false;
        }

        public void SetSite(IServiceProvider services)
        {
            this.Services = services;
        }

        public void Show(Program program, Procedure proc)
        {
            AddProcedure(program, proc);
            Services.RequireService<IWindowFrame>().Show();
        }

        /// <summary>
        /// Adds the procedure <paramref name="proc"/> of the <see cref="Program"/> <paramref name="program"/> to the
        /// call hierarchy tree.
        /// </summary>
        public void AddProcedure(Program program, Procedure proc)
        {
            if (Host.GetDesigner(proc) != null)
                return; // Already there!
            Host.AddComponent(null, new CHProcedureDesigner(program, proc));
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            var des = Host.GetSelectedDesigner();
            if (des != null)
            {
                Host.RemoveComponent(des);
            }
        }

        private class CHProcedureDesigner : TreeNodeDesigner
        {
            private readonly Program program;
            private readonly Procedure proc;

            public CHProcedureDesigner(Program program, Procedure proc)
            {
                this.program = program;
                this.proc = proc;
            }

            public override void Initialize(object obj)
            {
                base.Initialize(obj);
                base.TreeNode.Text = proc.Name;
                this.Host.AddComponent(this, new CHCallsDesigner(program, proc));
                this.Host.AddComponent(this, new CHCalleesDesigner(program, proc));
            }

            public override bool Equals(object obj)
            {
                if (obj is CHProcedureDesigner that)
                    return this.proc == that.proc;
                return false;
            }

            public override int GetHashCode()
            {
                return proc.GetHashCode();
            }
        }

        private class CHCallsDesigner : TreeNodeDesigner
        {
            private readonly Program program;
            private readonly Procedure proc;
            private object dummy;

            public CHCallsDesigner(Program program, Procedure proc)
            {
                this.program = program;
                this.proc = proc;
                this.dummy = new object();
            }

            public override void Initialize(object obj)
            {
                base.Initialize(obj);
                base.TreeNode.Text = $"Calls to '{proc.Name}'";
            }

            public override void OnExpanded()
            {
                if (dummy != null)
                {
                    dummy = null;
                    var callStms = program.CallGraph.CallerStatements(proc);
                    var designers = callStms
                        .Select(s => new CHCallStatementDesigner(program, s))
                        .ToArray();
                    Host.AddComponents(this, designers);
                    TreeNode.Expand();
                }
            }
        }

        private class CHCalleesDesigner  : TreeNodeDesigner
        {
            private readonly Program program;
            private readonly Procedure proc;
            private object dummy;

            public CHCalleesDesigner(Program program, Procedure proc)
            {
                this.program = program;
                this.proc = proc;
                this.dummy = new object();
            }

            public override void Initialize(object obj)
            {
                base.Initialize(obj);
                base.TreeNode.Text = $"Calls from '{proc.Name}'";
            }

            public override void OnExpanded()
            {
                if (dummy != null)
                {
                    dummy = null;
                    var callees = program.CallGraph.Callees(proc);
                    var designers = callees
                        .Select(p => new CHProcedureDesigner(program, p))
                        .ToArray();
                    Host.AddComponents(this, designers);
                    TreeNode.Expand();
                }
            }

        }

        private class CHCallStatementDesigner : TreeNodeDesigner
        {
            private readonly Program program;
            private readonly Statement stm;
            private object dummy;

            public CHCallStatementDesigner(Program program, Statement stm)
            {
                this.program = program;
                this.stm = stm;
                this.dummy = new object();
            }

            public override void Initialize(object obj)
            {
                base.Initialize(obj);
                var proc = stm.Block.Procedure;
                var offset = stm.LinearAddress - proc.EntryAddress.ToLinear();
                this.TreeNode.Text = string.Format("{0}+{1} {2}", proc, offset, stm.Instruction);
            }

            public override void OnExpanded()
            {
                if (dummy != null)
                {
                    dummy = null;
                    var procDes = Host.GetDesigner(stm.Block.Procedure);
                    if (procDes != null)
                        return;
                    procDes = new CHCallsDesigner(program, stm.Block.Procedure);
                    this.Host.AddComponent(this, procDes);
                }
            }

            public override void DoDefaultAction()
            {
                Services.RequireService<ICodeViewerService>().DisplayStatement(program, stm);
            }
        }
    }
}
