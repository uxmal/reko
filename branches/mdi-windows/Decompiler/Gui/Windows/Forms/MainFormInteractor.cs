/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Configuration;
using Decompiler.Core;
using Decompiler.Core.Services;
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
		DecompilerHost,
        IStatusBarService
	{
		private IMainForm form;		
		private IDecompilerService decompilerSvc;
        private IDecompilerShellUiService uiSvc;
        private IDiagnosticsService diagnosticsSvc;
		private IPhasePageInteractor currentPage;
		private InitialPageInteractor pageInitial;
		private ILoadedPageInteractor pageLoaded;
		private IAnalyzedPageInteractor pageAnalyzed;
		private IFinalPageInteractor pageFinal;
		private MruList mru;
        private string projectFileName;
        private ServiceContainer sc;
        private Dictionary<IPhasePageInteractor, IPhasePageInteractor> nextPage;
        private IDecompilerConfigurationService config;

		private static string dirSettings;
		
		private const int MaxMruItems = 10;

		public MainFormInteractor()
		{
			mru = new MruList(MaxMruItems);
			mru.Load(MruListFile);
            sc = new ServiceContainer();
            nextPage = new Dictionary<IPhasePageInteractor, IPhasePageInteractor>();
		}

		private void CreatePhaseInteractors()
		{
            pageInitial = CreateInitialPageInteractor();
            pageLoaded = CreateLoadedPageInteractor();
            pageAnalyzed = new AnalyzedPageInteractorImpl();
            pageFinal = new FinalPageInteractor();

            Add(pageInitial);
            Add(pageLoaded);
            Add(pageAnalyzed);
            Add(pageFinal);

            nextPage[pageInitial] = pageLoaded;
            nextPage[pageLoaded] = pageAnalyzed;
            nextPage[pageAnalyzed] = pageFinal;
		}


        protected virtual InitialPageInteractor CreateInitialPageInteractor()
        {
            return new InitialPageInteractorImpl(); 
        }

        protected virtual ILoadedPageInteractor CreateLoadedPageInteractor()
        {
            return new LoadedPageInteractor();
        }

		public virtual IDecompiler CreateDecompiler(LoaderBase ldr)
		{
            return new DecompilerDriver(ldr, this, sc);
		}

        public IMainForm LoadForm()
        {
            this.form = CreateForm();

			DecompilerMenus dm = new DecompilerMenus(this);
			form.Menu = dm.MainMenu;
            dm.MainToolbar.Text = "";
            dm.MainToolbar.ImageList = form.ImageList;
            form.AddToolbar(dm.MainToolbar);

            CreateServices(sc, dm);
            CreatePhaseInteractors();

            form.Closed += new System.EventHandler(this.MainForm_Closed);
			form.ToolBar.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(toolBar_ItemClicked);
			//form.InitialPage.IsDirtyChanged += new EventHandler(InitialPage_IsDirtyChanged);//$REENABLE
			SwitchInteractor(pageInitial);
			//MainForm.InitialPage.IsDirty = false;         //$REENABLE

            return form;
        }

        protected virtual IMainForm CreateForm()
        {
            return new MainForm2();
        }

        protected virtual void CreateServices(ServiceContainer sc, DecompilerMenus dm)
        {
            config = new DecompilerConfiguration();
            sc.AddService(typeof(IDecompilerConfigurationService), config);

            sc.AddService(typeof(IStatusBarService), (IStatusBarService) this);

            var f = new FindResultsInteractor();
            f.Attach(form.FindResultsList);
            sc.AddService(typeof(IFindResultsService), f);

            var d = new DiagnosticsInteractor();
            d.Attach(form.DiagnosticsList);
            diagnosticsSvc = d;
            sc.AddService(typeof(IDiagnosticsService), d);

            decompilerSvc = new DecompilerService();
            sc.AddService(typeof(IDecompilerService), decompilerSvc);

            uiSvc = CreateShellUiService(dm);
            sc.AddService(typeof(IDecompilerShellUiService), uiSvc);
            sc.AddService(typeof(IDecompilerUIService), uiSvc);

            var codeViewSvc = new CodeViewerServiceImpl(sc);
            sc.AddService(typeof(ICodeViewerService), codeViewSvc);

            var pibSvc = new ProgramImageBrowserService(form.BrowserList);
            sc.AddService(typeof(IProgramImageBrowserService), pibSvc);

            var del = CreateDecompilerListener();
            sc.AddService(typeof(IWorkerDialogService), (IWorkerDialogService) del);
            sc.AddService(typeof(DecompilerEventListener), del);

            ArchiveBrowserService abSvc = new ArchiveBrowserService(sc);
            sc.AddService(typeof(IArchiveBrowserService), abSvc);

            sc.AddService(typeof(IMemoryViewService), new MemoryViewServiceImpl(sc));
            sc.AddService(typeof(IDisassemblyViewService), new DisassemblyViewServiceImpl(sc));
        }

        protected virtual DecompilerEventListener CreateDecompilerListener()
        {
            return new WindowsDecompilerEventListener(sc);
        }

        protected virtual IDecompilerShellUiService CreateShellUiService(DecompilerMenus dm)
        {
            return new DecompilerShellUiService((Form)this.form, dm, form.OpenFileDialog, form.SaveFileDialog, this.sc);
        }

		public virtual TextWriter CreateTextWriter(string filename)
		{
            if (string.IsNullOrEmpty(filename))
                return StreamWriter.Null;
			return new StreamWriter(new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write), new UTF8Encoding(false));
		}

		public IPhasePageInteractor CurrentPage
		{
			get { return currentPage; }
			set { currentPage = value; }
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
                diagnosticsSvc.ClearDiagnostics();
                SwitchInteractor(InitialPageInteractor);
                pageInitial.OpenBinary(file, this);
            } 
			catch (Exception ex)
			{
                uiSvc.ShowError(ex, "Couldn't open file '{0}'.", file);
			}
		}

        private T GetService<T>()
        {
            return (T)GetService(typeof(T));
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

		public InitialPageInteractor InitialPageInteractor
		{
			get { return pageInitial; }
		}

		public ILoadedPageInteractor LoadedPageInteractor
		{
			get { return pageLoaded; }
		}

        public IAnalyzedPageInteractor AnalyzedPageInteractor
        {
            get { return pageAnalyzed; }
        }

		public IFinalPageInteractor FinalPageInteractor
		{
			get { return pageFinal; }
		}

		private static string MruListFile
		{
			get { return SettingsDirectory + "\\mru.txt"; }
		}

		public void NextPhase()
		{
            try
            {
                IPhasePageInteractor next;
                if (nextPage.TryGetValue(CurrentPage, out next))
                {
                    SwitchInteractor(next);
                }
            }
            catch (Exception ex)
            {
                uiSvc.ShowError(ex, "Unable to proceed.");
            }
		}

		public void FinishDecompilation()
		{
		}

        public void LayoutMdi(MdiLayout layout)
        {
            ((Form)form).LayoutMdi(layout);
        }

        public void ShowAboutBox()
        {
            using (AboutDialog dlg = new AboutDialog())
            {
                uiSvc.ShowModalDialog(dlg);
            }
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

		public void SwitchInteractor(IPhasePageInteractor interactor)
		{
            if (interactor == CurrentPage)
                return;

            if (CurrentPage != null)
            {
                if (!CurrentPage.LeavePage())
                    return;
            }
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
                case CmdIds.HelpAbout:
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

                case CmdIds.WindowsCascade: LayoutMdi(MdiLayout.Cascade); return true;
                case CmdIds.WindowsTileVertical: LayoutMdi(MdiLayout.TileVertical); return true;
                case CmdIds.WindowsTileHorizontal: LayoutMdi(MdiLayout.TileHorizontal); return true;
         
                case CmdIds.HelpAbout: ShowAboutBox(); return true;
				}
			}
			return false;
		}

		#endregion

        #region IStatusBarService Members ////////////////////////////////////

        public void SetText(string text)
        {
            form.StatusStrip.Items[0].Text = text;
        }

        #endregion


		#region DecompilerHost Members //////////////////////////////////

        public IDecompilerConfigurationService Configuration
        {
            get { return config; }
        }

		public TextWriter CreateDecompiledCodeWriter(string fileName)
		{
			return new StreamWriter(fileName, false, new UTF8Encoding(false));
		}

        public void WriteDisassembly(Action<TextWriter> writer)
        {
            using (TextWriter output = CreateTextWriter(decompilerSvc.Decompiler.Project.Output.DisassemblyFilename))
            {
                writer(output);
            }
        }

        public void WriteIntermediateCode(Action<TextWriter> writer)
        {
            using (TextWriter output = CreateTextWriter(decompilerSvc.Decompiler.Project.Output.IntermediateFilename))
            {
                writer(output);
            }
        }

        public void WriteTypes(Action<TextWriter> writer)
        {
            using (TextWriter output = CreateTextWriter(decompilerSvc.Decompiler.Project.Output.TypesFilename))
            {
                writer(output);
            }
        }

        public void WriteDecompiledCode(Action<TextWriter> writer)
        {
            using (TextWriter output = CreateTextWriter(decompilerSvc.Decompiler.Project.Output.OutputFilename))
            {
                writer(output);
            }
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
			if (cmd == null) throw new NotImplementedException("Button not hooked up.");
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


        public virtual void Run()
        {
            Application.Run((Form) LoadForm());
        }
    }

}
