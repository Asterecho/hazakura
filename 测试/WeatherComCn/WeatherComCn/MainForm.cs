/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 2023/10/13
 * Time: 14:22
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Text;
using LitJson;

namespace WeatherComCn
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
			string weather="";
			string weaimg="";
			string tem="";
			string humidity="";
			
			string tm=DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
			string url="http://d1.weather.com.cn/dingzhi/101131001.html?_="+tm; //简单天气信息
			string url2="http://toy1.weather.com.cn/search?cityname=%E4%BC%8A%E5%AE%81"; //查看城市对应id
			string url3="http://d1.weather.com.cn/sk_2d/101010100.html?_="+tm; //详细天气信息
			 
			string json3=httpGet(url3,"d1.weather.com.cn").Replace("var dataSK=","");
			JsonData txt=JsonMapper.ToObject(json3);
			tem=txt["temp"].ToString();
			humidity=txt["sd"].ToString();
			weather=txt["weather"].ToString();
			weaimg=txt["weathercode"].ToString();
			
			label1.Text=tem+" "+humidity+" "+weather;
			pictureBox1.Image=Image.FromFile(Directory.GetCurrentDirectory()+"\\res\\weather_icons_blue\\"+weaimg+".ico");
		//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		public static string httpGet(string Url,string host)
        {
            string retString = string.Empty;
            //System.GC.Collect();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Proxy = null;
            request.KeepAlive = false;
            request.Method = "GET";
            request.ContentType = "text/html";
            request.Host=host;
            request.Referer="http://www.weather.com.cn/";
            request.AutomaticDecompression = DecompressionMethods.GZip;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();

                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }catch(Exception ex){
                //抛出异常返回具体错误消息
                retString = ex.Message;
            }
            return retString;
        } 
		
	}
}
