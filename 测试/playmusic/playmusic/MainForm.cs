/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 2023/10/10
 * Time: 23:56
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace playmusic
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			
			
			String path = @"E:\歌曲";

//第一种方法
string[] files = Directory.GetFiles(path, "*.mp3");
string t="";
foreach (string file in files)
{
	t+=file+"\n";
}
 string[] line=t.Split('\n');
			Random rd = new Random();
            int i = rd.Next(0,line.Length);
            string  musicpath=line[i];
            
            playmuisc.musicplay mp=new playmuisc.musicplay();
			mp.PlayMusic(musicpath);
			
			this.Text=Path.GetFileNameWithoutExtension(musicpath);
            
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
	}
}
