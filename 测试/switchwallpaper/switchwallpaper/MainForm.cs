/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 2023/10/10
 * Time: 21:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace switchwallpaper
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
			WallPaperChange("LouLL_Aroll");
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		public void WallPaperChange(string name){
			WebClient web = new WebClient();
			string html = web.DownloadString("https://toolkit.gitee.io/cat-switch/list/"+name+".yml");
			 string[] line=html.Split('\n');
			Random rd = new Random();
            int i = rd.Next(0,line.Length);
            string  picpath=line[i];
           
				if (picpath.Contains("http")) {
					ChangeWallPaper(DownloadImageAndSaveFile(picpath));
				}
				else{
					ChangeWallPaper(picpath);
				}
		}
		 
		[DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
		public static extern int SystemParametersInfo(
		    int uAction,
		    int uParam,
		    string lpvParam,
		    int fuWinIni
		);
		/// <summary>
		///设置这个图片为壁纸
		/// </summary>		
		public void ChangeWallPaper(string filePath){
			SystemParametersInfo(20, 0, filePath, 2);
		}
		
		/// <summary>
		/// 下载图片并存储到临时文件夹下
		/// </summary>
		/// <param name="url">图片URL</param>
		/// <returns>保存下载图片文件的路径</returns>
		private static string DownloadImageAndSaveFile(string url)
		{
		    using (var client = new WebClient())
		    {
		        //创建临时文件目录下的存储必应图片的绝对路径
		        var filePath = Path.Combine(Path.GetTempPath(), "temp.jpg");
		        //将图片下载到这个路径下
		        client.DownloadFile(url, filePath);
		        //返回当前图片路径
		        return filePath;
		    }
		}
	}
}
