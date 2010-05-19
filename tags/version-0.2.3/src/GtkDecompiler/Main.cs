// project created on 11/24/2007 at 5:32 PM
using System;
using Gtk;

namespace GtkDecompiler
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			MainWindow win = new MainWindow ();
			win.Show ();
			Application.Run ();
		}
	}
}