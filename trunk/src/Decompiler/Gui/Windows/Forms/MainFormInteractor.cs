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

using Decompiler.Core;
using Decompiler.Core.Serialization;
using Decompiler.Gui;
using Decompiler.Loading;
using Decompiler.Gui.Windows.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Forms
{
	/// <summary>
	/// Provices a component Container implementation, and specifically handles interactions 
    /// with the MainForm. This decouples platform-specific code from the user interaction 
    /// code. This will make it easier to portto other GUI platforms.
	/// </summary>
	public class MainFormInteractor : 
        Container,
		ICommandTarget,
		DecompilerHost
	{
		private MainForm form;		
		private IDecompilerService decompilerSvc;
        private IDecompilerUIService uiSvc;
		private PhasePageInteractor currentPage;
		private InitialPageInteractor pageInitial;
		private LoadedPageInteractor pageLoaded;
		private AnalyzedPageInteractor pageAnalyzed;
		private FinalPageInteractor pageFinal;
		private MruList mru;
        private string projectFileName;
        private ServiceContainer sc;
        private Dictionary<PhasePageInteractor, PhasePageInteractor> nextPage;

		private static string dirSettings;
		
		private const int MaxMruItems = 10;

		public MainFormInteractor()
		{
			mru = new MruList(MaxMruItems);
			mru.Load(MruListFile);
            sc = new ServiceContainer();
            nextPage = new Dictionary<PhasePageInteractor, PhasePageInteractor>();
		}

		private void AttachInteractors(DecompilerMenus dm)
		{
            pageInitial = new InitialPageInteractor(form.StartPage);
            pageLoaded = new LoadedPageInteractor(form.LoadedPage);
            pageAnalyzed = new AnalyzedPageInteractor(form.AnalyzedPage);
            pageFinal = new FinalPageInteractor(form.FinalPage, this);

            Add(pageInitial);
            Add(pageLoaded);
            Add(pageAnalyzed);
            Add(pageFinal);

            nextPage[pageInitial] = pageLoaded;
            nextPage[pageLoaded] = pageAnalyzed;
            nextPage[pageAnalyzed] = pageFinal;
		}


		public virtual DecompilerDriver CreateDecompiler(LoaderBase ldr, Program prog)
		{
            return new DecompilerDriver(ldr, prog, this);
		}

        public MainForm CreateForm()
        {
            this.form = new MainForm();

			DecompilerMenus dm = new DecompilerMenus(this);
			form.Menu = dm.MainMenu;
            dm.MainToolbar.Text = "";
            dm.MainToolbar.ImageList = form.ImageList;
            form.AddToolbar(dm.MainToolbar);

            CreateServices(dm);
            AttachInteractors(dm);
			form.Closed += new System.EventHandler(this.MainForm_Closed);
			form.ToolBar.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(toolBar_ItemClicked);
			//form.InitialPage.IsDirtyChanged += new EventHandler(InitialPage_IsDirtyChanged);//$REENABLE
			SwitchInteractor(pageInitial);
			//MainForm.InitialPage.IsDirty = false;         //$REENABLE

            return form;
        }

        protected virtual LoaderBase CreateLoader(string filename, Program prog)
        {
            return new Loader(filename, prog);
        }

		public virtual Program CreateProgram()
		{
			return new Program();
		}

        private void CreateServices(DecompilerMenus dm)
        {
            FindResultsInteractor f = new FindResultsInteractor();
            f.Attach(form.FindResultsList);
            sc.AddService(typeof(IFindResultsService), f);

            DiagnosticsInteractor d = new DiagnosticsInteractor();
            d.Attach(form.DiagnosticsList);
            sc.AddService(typeof(IDiagnosticsService), d);

            decompilerSvc = new DecompilerService();
            sc.AddService(typeof(IDecompilerService), decompilerSvc);

            uiSvc = new DecompilerUiService(this.form, dm, form.OpenFileDialog, form.SaveFileDialog);
            sc.AddService(typeof(IDecompilerUIService), uiSvc);

            ProgramImageBrowserService pibSvc = new ProgramImageBrowserService(form.BrowserList);
            sc.AddService(typeof(IProgramImageBrowserService), pibSvc);
        }

		public virtual TextWriter CreateTextWriter(string filename)
		{
            if (string.IsNullOrEmpty(filename))
                return StreamWriter.Null;
			return new StreamWriter(new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write));
		}

		public PhasePageInteractor CurrentPage
		{
			get { return currentPage; }
			set { currentPage = value; }
		}

        [Obsolete("User decompilerService instead", true)]
		public DecompilerDriver Decompiler
		{
			get { return decompilerSvc.Decompiler; }
		}

        protected override object GetService(Type service)
        {
            object svc = sc.GetService(service);
            if (svc != null)
                return svc;
            return base.GetService(service);
        }

		public IMainForm MainForm
		{
			get { return form; }
		}

		public void OpenBinary(string file)
		{
			try 
			{
				Program prog = CreateProgram();
                LoaderBase ldr = CreateLoader(file, prog);
				decompilerSvc.Decompiler = CreateDecompiler(ldr, prog);
				decompilerSvc.Decompiler.LoadProgram();
			} 
			catch (Exception ex)
			{
				uiSvc.ShowError("Couldn't open file '{0}'. {1}", file, ex.Message + ex.StackTrace);
			}
            SwitchInteractor(pageLoaded);
		}

		public void OpenBinaryWithPrompt()
		{
			Cursor.Current = Cursors.WaitCursor;
			try
			{
				if (form.ShowDialog(form.OpenFileDialog) == DialogResult.OK)
				{
					OpenBinary(form.OpenFileDialog.FileName);
					mru.Use(form.OpenFileDialog.FileName);
				}
			} 
			finally 
			{
				Cursor.Current = Cursors.Arrow;
				form.SetStatus("");
			}
		}

        [Obsolete("Use decompilerservice", true)]
		public Program Program
		{
			get { return decompilerSvc.Decompiler.Program; }
		}

		public InitialPageInteractor InitialPageInteractor
		{
			get { return pageInitial; }
		}

		public LoadedPageInteractor LoadedPageInteractor
		{
			get { return pageLoaded; }
		}

        public AnalyzedPageInteractor AnalyzedPageInteractor
        {
            get { return pageAnalyzed; }
        }

		public FinalPageInteractor FinalPageInteractor
		{
			get { return pageFinal; }
		}

		private static string MruListFile
		{
			get { return SettingsDirectory + "\\mru.txt"; }
		}

		public void NextPhase()
		{
            PhasePageInteractor next;
            if (nextPage.TryGetValue(CurrentPage, out next))
			{
                SwitchInteractor(next);
			}
		}

		public void FinishDecompilation()
		{
		}

        public string ProjectFileName
        {
            get { return projectFileName; }
            set { projectFileName = value; }
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(this.ProjectFileName))
            {
                string newName = PromptForFilename(Path.ChangeExtension(decompilerSvc.Decompiler.Project.Input.Filename, DecompilerProject.FileExtension));
                if (newName == null)
                    return;
                ProjectFileName = newName;
                mru.Use(newName);

            }

            using (TextWriter sw = CreateTextWriter(ProjectFileName))
            {
                //$REFACTOR: rule of demeter, push this into a Save() method.
                decompilerSvc.Decompiler.Project.Save(sw);
            }
        }

        protected virtual string PromptForFilename(string suggestedName)
        {
            form.SaveFileDialog.FileName = suggestedName;
            if (DialogResult.OK != form.ShowDialog(form.SaveFileDialog))
                return null;
            else 
                return form.SaveFileDialog.FileName;
        }

		private static string SettingsDirectory
		{
			get 
			{ 
				if (dirSettings == null)
				{
					string dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\jkl\\grovel"; 
					if (!Directory.Exists(dir))
						Directory.CreateDirectory(dir);
					dirSettings = dir;
				}
				return dirSettings;
			}
		}

		public void SwitchInteractor(PhasePageInteractor interactor)
		{
			if (CurrentPage != null)
				CurrentPage.LeavePage();
            form.SetCurrentPage(interactor.Page);
			CurrentPage = interactor;
			interactor.EnterPage();
		}

		public void UpdateWindowTitle()
		{
			StringBuilder sb = new StringBuilder();
			if (!string.IsNullOrEmpty(decompilerSvc.ProjectName))
			{
				sb.Append(decompilerSvc.ProjectName);
                //$REFACTOR: dirtiness of project is not limiited to first page.
                //if (MainForm.InitialPage.IsDirty)
                //    sb.Append('*');
				sb.Append(" - ");
			}
			sb.Append("Decompiler");
			MainForm.TitleText = sb.ToString();
		}

		#region ICommandTarget members 
		public bool QueryStatus(ref Guid cmdSet, int cmdId, CommandStatus cmdStatus, CommandText cmdText)
		{
			if (currentPage != null && currentPage.QueryStatus(ref cmdSet, cmdId, cmdStatus, cmdText))
				return true;
			if (cmdSet == CmdSets.GuidDecompiler)
			{
				int iMru = cmdId - CmdIds.FileMru;
				if (0 <= iMru && iMru < mru.Items.Count)
				{
					cmdStatus.Status = MenuStatus.Visible|MenuStatus.Enabled;
					cmdText.Text = (string) mru.Items[iMru];
					return true;
				}

				switch (cmdId)
				{
				case CmdIds.FileOpen:
				case CmdIds.FileExit:
					cmdStatus.Status = MenuStatus.Enabled|MenuStatus.Visible;
					return true;
				case CmdIds.FileMru:
					cmdStatus.Status = MenuStatus.Visible;
					return true;
                case CmdIds.ActionNextPhase:
                    cmdStatus.Status = currentPage.CanAdvance
                        ? MenuStatus.Enabled | MenuStatus.Visible
                        : MenuStatus.Visible;
                    return true;
				}
			}
			return false;
		}

		public bool Execute(ref Guid cmdSet, int cmdId)
		{
			if (currentPage != null && currentPage.Execute(ref cmdSet, cmdId))
				return true;
			if (cmdSet == CmdSets.GuidDecompiler)
			{
				int iMru = cmdId - CmdIds.FileMru;
				if (0 <= iMru && iMru < mru.Items.Count)
				{
					string file = (string) mru.Items[iMru];
					OpenBinary(file);
					mru.Use(file);
					return true;
				}

				switch (cmdId)
				{
				case CmdIds.FileOpen: OpenBinaryWithPrompt(); return true;
                case CmdIds.FileSave: Save(); return true;
                case CmdIds.FileExit: form.Close(); return true;

				case CmdIds.ActionNextPhase: NextPhase(); return true;
				case CmdIds.ActionFinishDecompilation: FinishDecompilation(); return true;
				}
			}
			return false;
		}

		#endregion

		#region DecompilerHost Members //////////////////////////////////

		public TextWriter CreateDisassemblyWriter()
		{
            return CreateTextWriter(decompilerSvc.Decompiler.Project.Output.DisassemblyFilename);
		}

		public TextWriter CreateTypesWriter(string filename)
		{
			return CreateTextWriter(filename);
		}

		public void ShowProgress(string caption, int numerator, int denominator)
		{
			form.SetStatus(caption);
			form.ProgressBar.Value = numerator;
			form.ProgressBar.Minimum = 0;
			form.ProgressBar.Maximum = denominator;
            form.ProgressBar.Update();
		}


		public void CodeStructuringComplete()
		{
		}

		public void DecompilationFinished()
		{
			form.SetStatus("Finished");
		}

		public void InterproceduralAnalysisComplete()
		{
		}

		public void MachineCodeRewritten()
		{
			form.SetStatus("Machine code rewritten.");
		}

		public void ProceduresTransformed()
		{
			form.SetStatus("Procedures transformed.");
		}

		public void ProgramLoaded()
		{
			form.SetStatus("Program loaded.");
		}

		public void ProgramScanned()
		{
			form.SetStatus("Program scanned.");
		}

		public void TypeReconstructionComplete()
		{
			form.SetStatus("Data types reconstructed.");
		}

		public void WriteDiagnostic(Diagnostic d, Address addr, string format, params object[] args)
		{
            IDiagnosticsService svc = (IDiagnosticsService) GetService(typeof(IDiagnosticsService));
            if (svc != null)
            {
                svc.AddDiagnostic(d, addr, format, args);
            }
		}

		public TextWriter CreateDecompiledCodeWriter(string fileName)
		{
			return new StreamWriter(fileName, false, new UTF8Encoding(false));
		}

		public TextWriter GetIntermediateCodeWriter()
		{
			return CreateTextWriter(decompilerSvc.Decompiler.Project.Output.IntermediateFilename);
		}

		#endregion ////////////////////////////////////////////////////


		// Event handlers //////////////////////////////

		private void miFileExit_Click(object sender, System.EventArgs e)
		{
			form.Close();
		}

		private void MainForm_Closed(object sender, System.EventArgs e)
		{
			mru.Save(MruListFile);
		}


		private void statusBar_PanelClick(object sender, System.Windows.Forms.StatusBarPanelClickEventArgs e)
		{
		
		}

		private void txtLog_TextChanged(object sender, System.EventArgs e)
		{
		
		}


		private void toolBar_ItemClicked(object sender, System.Windows.Forms.ToolStripItemClickedEventArgs e)
		{
			MenuCommand cmd = e.ClickedItem.Tag as MenuCommand;
			if (cmd == null) throw new NotImplementedException("Button not hooked up");
			Guid g = cmd.CommandID.Guid;
			Execute(ref g, cmd.CommandID.ID);
		}

		public void OnBrowserTreeItemSelected(object sender, TreeViewEventArgs e)
		{
			if (e.Action == TreeViewAction.ByKeyboard ||
				e.Action == TreeViewAction.ByMouse)
			{
                throw new NotImplementedException();
			}
		}

		private void InitialPage_IsDirtyChanged(object sender, EventArgs e)
		{
			UpdateWindowTitle();
		}

    }
}
