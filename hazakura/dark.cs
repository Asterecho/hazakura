/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 2023/10/11
 * Time: 1:30
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices; 

namespace hazakura
{
	/// <summary>
	/// Description of dark.
	/// </summary>
	public partial class dark : Form
	{
		[DllImport("user32.dll", EntryPoint = "GetWindowLong")]  
        public static extern long GetWindowlong(IntPtr hwnd, int nIndex);
		[DllImport("user32.dll", EntryPoint = "SetWindowLong")]  
		        public static extern long SetWindowlong(IntPtr hwnd, int nIndex, long dwNewlong);
		[DllImport("user32", EntryPoint = "SetLayeredWindowAttributes")]  
		        private static extern int SetLayeredWindowAttributes(IntPtr Handle, int crKey, byte bAlpha, int dwFlags);
		const int GWL_EXSTYLE = -20;
		const int WS_EX_TRANSPARENT = 0x20;
		const int WS_EX_LAYERED = 0x80000;
		const int LWA_ALPHA = 2;
		
		public dark()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			this.BackColor = Color.Black;
			//this.Opacity=0.8;
			this.TopMost = true;
			this.FormBorderStyle = FormBorderStyle.None;
			this.WindowState = FormWindowState.Maximized;
			SetWindowlong(Handle, GWL_EXSTYLE, GetWindowlong(Handle, GWL_EXSTYLE) | WS_EX_TRANSPARENT | WS_EX_LAYERED);
			SetLayeredWindowAttributes(Handle, 0, 64, LWA_ALPHA );
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
	}
}
