/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 2023/10/11
 * Time: 15:13
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.IO;
using System.Net;
using RestSharp;
using LitJson;

namespace Translate
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
		//	
			string info=GetToken();
			JsonData json=JsonMapper.ToObject(info);  //https://blog.csdn.net/DoyoFish/article/details/81976181
			JsonData token=json["access_token"];
	 
	
			JsonData json2=JsonMapper.ToObject(textTrans("さくら",token.ToString()));  //https://blog.csdn.net/DoyoFish/article/details/81976181
			JsonData data=json2["result"];
			string txt=data["trans_result"][0]["dst"].ToString();
			MessageBox.Show(txt);
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		 
		public static string textTrans(string txt,string token)
        {
            
            string host = "https://aip.baidubce.com/rpc/2.0/mt/texttrans/v1?access_token=" + token;
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
            request.Method = "post";
            request.KeepAlive = true;
            String str = "{\"from\":\"auto\",\"to\":\"zh\",\"q\":\""+txt+"\",\"termIds\":\"\"}";
            byte[] buffer = encoding.GetBytes(str);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string result = reader.ReadToEnd();
          //  Console.WriteLine(result);
            return result;
        }
		
		public static string GetToken(){
			string API_KEY = "xLo3gRoE6siGlwpPrYO5XHPf";
            string SECRET_KEY = "C6DxZT2d8t67x65lb7RP8e1F2ebpIX4u";
            
			var client = new RestClient("https://aip.baidubce.com/oauth/2.0/token?grant_type=client_credentials&client_id="+API_KEY+"&client_secret="+SECRET_KEY);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            var body = @"";
            request.AddParameter("application/json", body,  ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.Content;
		}
	}
}
