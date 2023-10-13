/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 2023/10/12
 * Time: 22:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Web;
using System.Net;
using System.IO;

namespace lingva_translate
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
			string txt="こんにちは";
			string t=HttpUtility.UrlEncode(txt,System.Text.Encoding.UTF8);
			string json=GetWebClient("https://translate.plausibility.cloud/api/v1/auto/zh/"+t);
			MessageBox.Show(json);
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		private string GetWebClient(string url)
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
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
