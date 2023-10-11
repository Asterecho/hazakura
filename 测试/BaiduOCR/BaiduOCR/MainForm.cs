/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 2023/10/11
 * Time: 14:22
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json;
using Baidu.Aip;

namespace BaiduOCR
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
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		void Button1Click(object sender, EventArgs e)
		{
			var APP_ID = "40876704";
			var API_KEY = "RL2k63KNKu44yMDpFlmtFZgk";
            var SECRET_KEY = "3G1BshN3lODB717SQzMWIV0i9OOx27QQ";

            var client = new Baidu.Aip.Ocr.Ocr(API_KEY, SECRET_KEY);
            client.Timeout = 60000;  // 修改超时时间
            
          //  var result = client.GeneralEnhanced(imageToByte(pictureBox1.Image));        //本地图图片
var image = imageToByte(pictureBox1.Image);
	// 调用通用文字识别, 图片参数为本地图片，可能会抛出网络等异常，请使用try/catch捕获
	var result = client.GeneralBasic(image);
	Console.WriteLine(result);
	// 如果有可选参数
	var options = new Dictionary<string, object>{
	    {"language_type", "CHN_ENG"},
	    {"detect_direction", "true"},
	    {"detect_language", "true"},
	    {"probability", "true"}
	};
	// 带参数调用通用文字识别, 图片参数为本地图片
	result = client.GeneralBasic(image, options);           
          MessageBox.Show(result.ToString());
		}
		private byte[] imageToByte(System.Drawing.Image _image)
{
    MemoryStream ms = new MemoryStream();
    _image.Save(ms,System.Drawing.Imaging.ImageFormat.Jpeg);
    return  ms.ToArray();
} 
	}
}
