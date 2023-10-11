/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 2023/10/10
 * Time: 20:23
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using LitJson;

namespace tianqi
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
			string info=GetWebClient("https://v0.yiketianqi.com/api?unescape=1&version=v91&appid=74185547&appsecret=vzMA08Tw&ext=&cityid=CN101131001");
			JsonData json=JsonMapper.ToObject(info);  //https://blog.csdn.net/DoyoFish/article/details/81976181
			 
				JsonData data=json["data"];
				string weather=data[0]["wea"].ToString();
				string weaimg=data[0]["wea_img"].ToString();
				string tem=data[0]["tem"].ToString()+"°C";
				string humidity=data[0]["humidity"].ToString();
				
				pictureBox1.Image=Image.FromFile(Directory.GetCurrentDirectory()+"\\weaimg\\"+weaimg+".png");
				label1.Text=weather+" "+tem+"|"+humidity;
			 
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		private string GetWebClient(string url)
		{
		    string strHTML = "";
		    WebClient myWebClient = new WebClient();
		    Stream myStream = myWebClient.OpenRead(url);
		    StreamReader sr = new StreamReader(myStream, System.Text.Encoding.GetEncoding("utf-8"));
		    strHTML = sr.ReadToEnd();
		    myStream.Close();
		    return strHTML;
		}
	}
}
