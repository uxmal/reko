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

namespace Reko.WebSite
{
using Reko.Core;
using Reko.Core.Serialization;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web;
//using System.Web.SessionState;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using System.Web.UI.HtmlControls;
using Reko.Loading;
using System.ComponentModel.Design;
using Reko.Core.Configuration;

	public class TestC // : System.Web.UI.Page
	{
#if WEB_FORMS
		private WebDecompilerHost host;

		protected System.Web.UI.WebControls.DropDownList ddlSamples;
		protected System.Web.UI.WebControls.TextBox txtAssembler;
		protected System.Web.UI.WebControls.Button btnDecompile;
		protected System.Web.UI.WebControls.Literal plcOutput;
		protected System.Web.UI.WebControls.Literal plcDecompiled;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			host = new WebDecompilerHost();
			if (!IsPostBack)
			{
				host.PopulateSampleFiles(Server, "*.c", ddlSamples);
			}
		}

        #region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.ddlSamples.SelectedIndexChanged += new System.EventHandler(this.ddlSamples_SelectedIndexChanged);
			this.Load += new EventHandler(Page_Load);
			this.btnDecompile.Click += new EventHandler(btnDecompile_Click);

		}
        #endregion

		private bool CompileCFile(string workdir, string filename)
		{
			Process p = new Process();
			ProcessStartInfo si = new ProcessStartInfo("cl.exe", "-c -Fa -Ox -nologo " + filename);
			si.ErrorDialog = false;
			si.CreateNoWindow = true;
			si.RedirectStandardOutput = true;
			si.RedirectStandardError = true;
			si.UseShellExecute = false;
			si.WorkingDirectory = workdir;
			p.StartInfo = si;

			p.Start();
			if (p.WaitForExit(5000))
			{
				StringWriter sw = new StringWriter();
				if (p.ExitCode == 0)
				{
					return true;
				}
				else
				{
					sw.WriteLine(si.Arguments);
					sw.WriteLine("Error code: {0}", p.ExitCode);
					string s = p.StandardOutput.ReadLine();		// read and ignore the source file name
					CopyTextWriter(p.StandardOutput, sw);
					sw.WriteLine();
					CopyTextWriter(p.StandardError, sw);

					plcOutput.Text = sw.ToString();
				}
			}
			return false;
		}

		private void CopyCSourceToTempFile(string cCode, string fileName)
		{
			using (StreamWriter w = new StreamWriter(fileName, false, Encoding.GetEncoding("ISO-8859-1")))
			{
				w.WriteLine(cCode);
			}
		}

		private void CopyTextWriter(TextReader rdr, TextWriter writer)
		{
			string s = rdr.ReadLine();
			while (s is not null)
			{
				writer.WriteLine(s);
				s = rdr.ReadLine();
			}
		}

		private void DecompileC()
		{
			string tmpName = Guid.NewGuid().ToString();
			string tmpDir = Server.MapPath("tmp");
			string cFile = Path.Combine(tmpDir, tmpName + ".c");
			string asmFile = Path.Combine(tmpDir, tmpName + ".asm");
			try
			{
				CopyCSourceToTempFile(txtAssembler.Text, cFile);
				if (CompileCFile(tmpDir, cFile))
				{
                    var sc = new ServiceContainer();
                    var ldr = new Loader(sc);
                    var cfg = RekoConfigurationService.Load(sc);
                    var arch = cfg.GetArchitecture("x86-protected-32");
                    var env = cfg.GetEnvironment("win32");
                    var platform = env.Load(sc, arch);
                    var asm = arch.CreateAssembler(null);
                    var program = asm.AssembleFragment(Address.Ptr32(0x10000000), txtAssembler.Text + Environment.NewLine);
                    program.Platform = platform;
					var decomp = new Decompiler(ldr, sc);
					var project = new Project
                    {
                        Programs = { program }
                    };
                    decomp.Project = project;
                    decomp.ScanPrograms();
                    decomp.AnalyzeDataFlow();
                    decomp.ReconstructTypes();
                    decomp.StructureProgram();
                    decomp.WriteDecompilerProducts();

					plcOutput.Text = host.DisassemblyWriter.ToString();
					plcDecompiled.Text = host.DecompiledCodeWriter.ToString();
				}
			} 
			finally
			{
				if (File.Exists(asmFile))
					File.Delete(asmFile);
			}
		}

		private void btnDecompile_Click(object sender, System.EventArgs e)
		{
			DecompileC();
		}

		private void ddlSamples_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			txtAssembler.Text = host.FetchSample(Server, ddlSamples.SelectedValue);
		}
#endif
    }
}
