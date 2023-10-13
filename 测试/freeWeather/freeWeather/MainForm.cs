/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 2023/10/13
 * Time: 13:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using LitJson;
using System.Net;
using System.IO;

namespace freeWeather
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public MainForm()
		{
			//http://t.weather.sojson.com/api/weather/city/101131001
			//http://t.weather.itboy.net/api/weather/city/101131001
			//
			//https://www.cnblogs.com/dyhao/p/11942563.html
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//MessageBox.Show(txt["data"]["shidu"].ToString());
			string weather="";
			string weaimg="";
			string tem="";
			string humidity="";
			
			string json=GetWebClient("http://t.weather.itboy.net/api/weather/city/101131001");
			JsonData txt=JsonMapper.ToObject(json);
			tem=txt["data"]["wendu"].ToString();
			humidity=txt["data"]["shidu"].ToString();
			weather=txt["data"]["forecast"][0]["type"].ToString();
		 	
			label1.Text=tem+" "+humidity+" "+weather;
			pictureBox1.Image=Image.FromFile("res\\"+weather+".ico");
			
		
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
