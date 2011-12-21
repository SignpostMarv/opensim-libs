using System;
using System.Collections;
using System.Windows.Forms;

namespace TestWarp
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.Run(new Warp());
		}
	}
}