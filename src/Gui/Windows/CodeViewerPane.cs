#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using Reko.Core.CLanguage;
using Reko.Core.Output;
using Reko.Gui.Windows.Controls;
using Reko.Gui.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Reko.Core.Types;

namespace Reko.Gui.Windows
{
    /// <summary>
    /// Pane that displays decompiled code, allows the user to navigate code,
    /// and annotate it. It displays both executable code (procedures)
    /// and data types.
    /// </summary>
    public class CodeViewerPane : IWindowPane, ICommandTarget
    {
        private CodeView codeView;
        private IServiceProvider services;
        private Program program;
        private Procedure proc;
        private DataType dataType;
        private NavigationInteractor<Procedure> navInteractor;
        private bool ignoreEvents;

        public TextView TextView { get { return codeView.TextView; } }
        public TextBox Declaration { get { return codeView.ProcedureDeclaration; } }
        public IWindowFrame FrameWindow { get; set; }

        internal void DisplayType(Program program, DataType dt)
        {
            throw new NotImplementedException();
        }

        public bool IsDirty { get; set; }

        #region IWindowPane Members

        public Control CreateControl()
        {
            var uiPrefsSvc = services.RequireService<IUiPreferencesService>();

            this.codeView = new CodeView();
            this.codeView.Dock = DockStyle.Fill;
            this.codeView.CurrentAddressChanged += codeView_CurrentAddressChanged;
            this.codeView.ProcedureDeclaration.TextChanged += ProcedureDeclaration_TextChanged;

            this.TextView.Font = new Font("Lucida Console", 10F);
            this.TextView.BackColor = SystemColors.Window;
            this.TextView.Services = services;

            this.TextView.ContextMenu = services.RequireService<IDecompilerShellUiService>().GetContextMenu(MenuIds.CtxCodeView);

            this.navInteractor = new NavigationInteractor<Procedure>();
            this.navInteractor.Attach(codeView);
            this.TextView.Navigate += textView_Navigate;
            return this.codeView;
        }

        private void EnableControls()
        {
            Core.Serialization.Procedure_v1 sProc;
            if (!TryParseSignature(codeView.ProcedureDeclaration.Text, out sProc))
            {
                // If parser failed, perhaps it's simply a valid name? 
                if (!IsValidCIdentifier(codeView.ProcedureDeclaration.Text))
                {
                    // Not valid name either, die.
                    codeView.ProcedureDeclaration.ForeColor = Color.Red;
                    return;
                }
            }
            codeView.ProcedureDeclaration.ForeColor = SystemColors.ControlText;
        }

        public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            if (cmdId.Guid == CmdSets.GuidReko)
            {
                switch (cmdId.ID)
                {
                case CmdIds.EditCopy:   //$TODO: once the TextViewer supports selections, these two 
                    status.Status = codeView.TextView.Selection.IsEmpty
                        ? MenuStatus.Visible
                        : MenuStatus.Visible | MenuStatus.Enabled;
                    return true;
                }
            }
            return false;
        }

        public bool Execute(CommandID cmdId)
        {
            if (cmdId.Guid == CmdSets.GuidReko)
            {
                switch (cmdId.ID)
                {
                case CmdIds.EditCopy:
                    Copy();
                    return true;
                }
            }
            return false;
        }

        public void Copy()
        {
            if (this.proc == null)
                return;
            if (codeView.TextView.Focused)
            {
                var ms = new MemoryStream();
                this.codeView.TextView.Selection.Save(ms, System.Windows.Forms.DataFormats.UnicodeText);
                Debug.Print(Encoding.Unicode.GetString(ms.ToArray()));
                Clipboard.SetData(DataFormats.UnicodeText, ms);
            }
            else if (codeView.ProcedureDeclaration.Focused)
            {
                Clipboard.SetText(codeView.ProcedureDeclaration.SelectedText);
            }
        }

        private bool IsValidCIdentifier(string id)
        {
            return Regex.IsMatch(id, "^[_a-zA-Z][_a-zA-Z0-9]*$");
        }

        private bool TryParseSignature(string txtSignature, out Core.Serialization.Procedure_v1 sProc)
        {
            // save the user a keystroke.
            txtSignature = txtSignature + ";";
            var lexer = new Core.CLanguage.CLexer(new StringReader(txtSignature));
            var cstate = new Core.CLanguage.ParserState();
            var cParser = new CParser(cstate, lexer);
            try
            {
                var decl = cParser.Parse_Decl();
                sProc = null;
                if (decl == null)
                    return false;
                var syms = new SymbolTable();
                syms.AddDeclaration(decl);
                if (syms.Procedures.Count != 1)
                    return false;
                sProc = (Core.Serialization.Procedure_v1) syms.Procedures[0];
                return true;
            }
            catch
            {
                sProc = null;
                return false;
            }
        }

        public void SetSite(IServiceProvider sp)
        {
            this.services = sp;
        }

        public void Close()
        {
            if (codeView!=null)
                ((Control)codeView).Dispose();
            codeView = null;
        }

        #endregion

        /// <summary>
        /// Display the procedure <paramref name="proc"/> in the code window.
        /// </summary>
        /// <param name="proc"></param>
        public void DisplayProcedure(Program program,Procedure proc)
        {
            if (codeView == null || program == null || proc == null)
                return;

            if (this.proc != null)
                this.proc.NameChanged -= Procedure_NameChanged;
            this.ignoreEvents = true;
            this.program = program;
            this.proc = proc;
            SetTextView(proc);
            SetDeclaration(proc);
            this.proc.NameChanged += Procedure_NameChanged;

            // Navigate 
            this.codeView.CurrentAddress = proc;
            IsDirty = false;
            EnableControls();
            ignoreEvents = false;
        }

        public void DisplayDataType(Program program, DataType dt)
        {
            if (codeView == null || program == null || dt == null)
                return;
            this.program = program;
            this.dataType = dt;
            this.SetTextView(dt);
            SetDeclaration(dt);
        }

        /// <summary>
        /// If the user has provided us with a declaration, use that. Otherwise
        /// just show a function name.
        /// </summary>
        /// <param name="proc"></param>
        private void SetDeclaration(Procedure proc)
        {
            int i = program.Procedures.IndexOfValue(proc);
            if (i >= 0)
            {
                Reko.Core.Serialization.Procedure_v1 uProc;
                if (program.UserProcedures.TryGetValue(program.Procedures.Keys[i], out uProc))
                {
                    this.codeView.ProcedureDeclaration.Text = uProc.CSignature;
                    return;
                }
            }
            this.codeView.ProcedureDeclaration.Text = proc.Name;
        }

        private void SetDeclaration(DataType dt)
        {
            codeView.ProcedureDeclaration.Text = "";
        }


        private void SetTextView(Procedure proc)
        {
            var tsf = new TextSpanFormatter();
            var fmt = new AbsynCodeFormatter(tsf);
            fmt.InnerFormatter.UseTabs = false;
            fmt.Write(proc);
            this.TextView.Model = tsf.GetModel();
        }

        private void SetTextView(DataType dt)
        {
            var tsf = new TextSpanFormatter() { Indentation = 0 };
            var fmt = new TypeFormatter(tsf, false);
            tsf.UseTabs = false;
            fmt.Write(dt, "");
            this.TextView.Model = tsf.GetModel();
        }

        void codeView_CurrentAddressChanged(object sender, EventArgs e)
        {
            if (ignoreEvents)
                return;
            var value = codeView.CurrentAddress;
            DisplayProcedure(program, value);
        }

        void textView_Navigate(object sender, EditorNavigationArgs e)
        {
            var procDst = e.Destination as Procedure;
            if (procDst == null)
                return;
            navInteractor.RememberAddress(this.proc);
            DisplayProcedure(this.program, procDst);    // ...and move to the new position.

            //$REVIEW: should this fire an event on a ISelectionService interface
            // and let interested parties track that?
            var pbSvc = services.GetService<IProjectBrowserService>();
            if (pbSvc != null)
            {
                pbSvc.SelectedObject = procDst;
            }
        }

        private void ModifyProcedure()
        {
            //$REVIEW: slow, but we don't have to add an `Address` property to procedure.
            // It has big repercussions in teh code base.
            var iAddr = this.program.Procedures.IndexOfValue(proc);
            if (iAddr < 0)
                return;
            var addr = this.program.Procedures.Keys[iAddr];
            var declText = codeView.ProcedureDeclaration.Text.Trim();
            Core.Serialization.Procedure_v1 sProc;
            if (TryParseSignature(codeView.ProcedureDeclaration.Text, out sProc))
            {
                var up = program.EnsureUserProcedure(addr, sProc.Name);
                up.CSignature = codeView.ProcedureDeclaration.Text;
                proc.Name = sProc.Name;
            }
            else
            {
                if (IsValidCIdentifier(declText) &&
                    proc.Name != declText)
                {
                    var up = program.EnsureUserProcedure(addr, proc.Name);
                    proc.Name = declText;
                }
            }
        }

        private void ProcedureDeclaration_TextChanged(object sender, EventArgs e)
        {
            if (ignoreEvents)
                return;
            ModifyProcedure();
            IsDirty = true;
            EnableControls();
        }

        private void Procedure_NameChanged(object sender, EventArgs e)
        {
            SetTextView(proc);
            FrameWindow.Title = proc.Name;
        }
    }
}
