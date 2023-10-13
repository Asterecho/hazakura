/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 2023/10/10
 * Time: 12:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices; 
using System.Diagnostics;
using LitJson;
using System.Text;
using System.IO;
using System.Net;
using System.Web;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using CSharpWin_JD.CaptureImage;
using INIHelper;
using ZXing.QrCode;
using ZXing;
using ZXing.Common;
using ZXing.Rendering;
using Newtonsoft;
using Baidu.Aip;

namespace hazakura
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		//毛玻璃 https://www.cnblogs.com/qiaoke/p/8395106.html
		[DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);
 
        [StructLayout(LayoutKind.Sequential)]
        internal struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }
 
        internal enum WindowCompositionAttribute
        {
            WCA_ACCENT_POLICY = 19
        }
 
        internal enum AccentState
        {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLE_GRADIENT = 1,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_ENABLE_ACRYLICBLURBEHIND = 4,
            ACCENT_INVALID_STATE = 5
            	 
        }
 
        [StructLayout(LayoutKind.Sequential)]
        internal struct AccentPolicy
        {
            public AccentState AccentState;
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;
        }
 
        internal void EnableBlur()
        {
            var accent = new AccentPolicy();
            var accentStructSize = Marshal.SizeOf(accent);
          	//accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND; //毛玻璃
            accent.AccentState = AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND; //亚克力
 
            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);
 
            var data = new WindowCompositionAttributeData();
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;
 
            SetWindowCompositionAttribute(this.Handle, ref data);
 
            Marshal.FreeHGlobal(accentPtr);
        }
        
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			
			//C# 获取系统DPI缩放比例以及分辨率大小 https://www.cnblogs.com/mq0036/p/17474333.html
			this.Width=Screen.PrimaryScreen.Bounds.Width;
			this.Height=Convert.ToInt16(Screen.PrimaryScreen.Bounds.Height * 0.03f);
			this.Location = (Point)new Size(0, 0);  
		//	this.TransparencyKey = this.BackColor;
			  
			
			pictureBox1.Width=this.Height;
			pictureBox1.Height=this.Height;
			pictureBox1.Location= (Point)new Size(0, 0);  
			
			string weather="";
			string weaimg="";
			string tem="";
			string humidity="";
			
			IniFiles ini=new IniFiles(Directory.GetCurrentDirectory()+"\\setting.ini");
			string type=ini.IniReadValue("weather","type");
			switch (type) {
				case "1":
				string cityid=ini.IniReadValue("weather","cityid");
				string appid=ini.IniReadValue("weather","appid");
				string appsecret=ini.IniReadValue("weather","appsecret");
	             if (appid=="") {
	             	MessageBox.Show("请到setting.ini设置appid");
	             	this.Close();
				} 
	    
				string info=GetWebClient("https://v0.yiketianqi.com/api?unescape=1&version=v91&appid="+appid+"&appsecret="+appsecret+"&ext=&cityid=CN"+cityid);
				JsonData json=JsonMapper.ToObject(info);  //https://blog.csdn.net/DoyoFish/article/details/81976181
				JsonData data=json["data"];
				weather=data[0]["wea"].ToString();
				weaimg=data[0]["wea_img"].ToString();
				tem=data[0]["tem"].ToString()+"°C";
				humidity=data[0]["humidity"].ToString();
				pictureBox2.Image=Image.FromFile(Directory.GetCurrentDirectory()+"\\weaimg\\"+weaimg+".png");
				break;
				
				case "2":
				string cityid2=ini.IniReadValue("weather","cityid");
				string json2=GetWebClient("http://t.weather.itboy.net/api/weather/city/"+cityid2);
				JsonData txt=JsonMapper.ToObject(json2);
				tem=txt["data"]["wendu"].ToString()+"°C";
				humidity=txt["data"]["shidu"].ToString();
				weather=txt["data"]["forecast"][0]["type"].ToString();
				pictureBox2.Image=Image.FromFile(Directory.GetCurrentDirectory()+"\\res\\"+weather+".ico");
				break;
				
				case "3":
				string cityid3=ini.IniReadValue("weather","cityid");
				string tm=DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
			//	string url="http://d1.weather.com.cn/dingzhi/101131001.html?_="+tm; //简单天气信息
			//	string url2="http://toy1.weather.com.cn/search?cityname=%E4%BC%8A%E5%AE%81"; //查看城市对应id
				string url3="http://d1.weather.com.cn/sk_2d/"+cityid3+".html?_="+tm; //详细天气信息
				 
				string json3=httpGet(url3,"d1.weather.com.cn").Replace("var dataSK=","");
				JsonData txt2=JsonMapper.ToObject(json3);
				tem=txt2["temp"].ToString()+"°C";
				humidity=txt2["sd"].ToString();
				weather=txt2["weather"].ToString();
				weaimg=txt2["weathercode"].ToString();
				pictureBox2.Image=Image.FromFile(Directory.GetCurrentDirectory()+"\\res\\weather_icons_blue\\"+weaimg+".ico");
				break;
			}
			
			
		  label3.Text=System.DateTime.Now.ToString("F"); 
		  label3.Width= TextRenderer.MeasureText(label3.Text, label3.Font).Width; 
		  label3.Location=(Point)new Size(Screen.PrimaryScreen.Bounds.Width-label3.Width, this.Height/4);
		  label2.Text="电量"+ GetSystemPower().ToString() +"%";
          label2.Width= TextRenderer.MeasureText(label2.Text, label2.Font).Width; 
          label2.Location=(Point)new Size(label3.Location.X-this.Height-label2.Width-this.Height/4,this.Height/4);
          label1.Text=weather+" "+tem+" | 湿度"+humidity+" |";
          label1.Width= TextRenderer.MeasureText(label1.Text, label1.Font).Width; 
          label1.Location=(Point)new Size(label2.Location.X-label1.Width,this.Height/4);
          
		//  pictureBox2.Image=Image.FromFile(Directory.GetCurrentDirectory()+"\\weaimg\\"+weaimg+".png");
		  pictureBox2.Height=this.Height;
		  pictureBox2.Width=this.Height;
		  pictureBox2.Location=(Point)new Size(label1.Location.X-Height,0);

			label4.Text=GetWebClient("https://v1.hitokoto.cn/?encode=text");
			label4.Width= TextRenderer.MeasureText(label4.Text, label4.Font).Width; 
			label4.Location=(Point)new Size((Screen.PrimaryScreen.Bounds.Width-label4.Width)/2, this.Height/4);

		  pictureBox3.Height=this.Height;
		  pictureBox3.Width=this.Height;
		 pictureBox3.Location=(Point)new Size(label3.Location.X-pictureBox3.Width-this.Height/4,0);
			this.pictureBox3.MouseWheel += pictureBox3_MouseWheel;
			
			label5.Text="随机音乐播放器";
          label5.Width= TextRenderer.MeasureText(label5.Text, label5.Font).Width; 
          label5.Location=(Point)new Size(Height+this.Height/4,this.Height/4);
          
          pictureBox4.Height=this.Height;
		  pictureBox4.Width=this.Height;
		 pictureBox4.Location=(Point)new Size(this.Width/5+Height,0);
		
		  pictureBox5.Height=this.Height;
		  pictureBox5.Width=this.Height;
		 pictureBox5.Location=(Point)new Size(this.Width/5-Height-Height/4+Height,0);
		 
		 pictureBox6.Height=this.Height;
		  pictureBox6.Width=this.Height;
		  pictureBox6.Location=(Point)new Size(this.Width/5-(Height+Height/4)*2+Height,0);
		  
		   pictureBox7.Height=this.Height;
		  pictureBox7.Width=this.Height;
		  pictureBox7.Location=(Point)new Size(this.Width/5-(Height+Height/4)*3+Height,0);
		  
		  pictureBox8.Height=this.Height;
		  pictureBox8.Width=this.Height;
		  pictureBox8.Location=(Point)new Size(this.Width/5-(Height+Height/4)*4+Height,0);
		 
		  pictureBox9.Height=this.Height;
		  pictureBox9.Width=this.Height;
		 pictureBox9.Location=(Point)new Size(this.Width/5+Height*2+Height/4,0);
		 
		//	EnableBlur();
			RegisterBar();
			//控件自适应 https://blog.csdn.net/cdc8596/article/details/111386085
			
			//窗口停靠 https://blog.csdn.net/WPwalter/article/details/97550243
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			
			 
		}
		private void pictureBox3_MouseWheel(object sender, MouseEventArgs e)
		{
			//当e.Delta > 0时鼠标滚轮是向上滚动，e.Delta < 0时鼠标滚轮向下滚动
			if (e.Delta > 0)//滚轮向上
			{
				VolumeUp(); //放大
				//MessageBox.Show("鼠标向上滑动");
			}
			else
			{
				VolumeDown();//缩小
				//MessageBox.Show("鼠标向下滑动");
			} 
		}
		//-------------------------
		 //按钮圆角 https://bbs.csdn.net/topics/392923052
		 //https://blog.csdn.net/liaogaobo2008/article/details/103649207
		 //https://www.cnblogs.com/pachleng/p/8329579.html
		 
		 #region APPBAR

        [StructLayout(LayoutKind.Sequential)]
        struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct APPBARDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public int uCallbackMessage;
            public int uEdge;
            public RECT rc;
            public IntPtr lParam;
        }

        enum ABMsg : int
        {
            ABM_NEW=0,
            ABM_REMOVE=1,
            ABM_QUERYPOS=2,
            ABM_SETPOS=3,
            ABM_GETSTATE=4,
            ABM_GETTASKBARPOS=5,
            ABM_ACTIVATE=6,
            ABM_GETAUTOHIDEBAR=7,
            ABM_SETAUTOHIDEBAR=8,
            ABM_WINDOWPOSCHANGED=9,
            ABM_SETSTATE=10
        }

        enum ABNotify : int
        {
            ABN_STATECHANGE=0,
            ABN_POSCHANGED,
            ABN_FULLSCREENAPP,
            ABN_WINDOWARRANGE
        }

        enum ABEdge : int
        {
            ABE_LEFT=0,
            ABE_TOP,
            ABE_RIGHT,
            ABE_BOTTOM
        }

        private bool fBarRegistered = false;

        [DllImport("SHELL32", CallingConvention=CallingConvention.StdCall)]
        static extern uint SHAppBarMessage(int dwMessage, ref APPBARDATA pData);
        [DllImport("USER32")]
        static extern int GetSystemMetrics(int Index);
        [DllImport("User32.dll", ExactSpelling=true, 
            CharSet=System.Runtime.InteropServices.CharSet.Auto)]
        private static extern bool MoveWindow
            (IntPtr hWnd, int x, int y, int cx, int cy, bool repaint);
        [DllImport("User32.dll", CharSet=CharSet.Auto)]
        private static extern int RegisterWindowMessage(string msg);
        private int uCallBack;

        private void RegisterBar()
        {
            APPBARDATA abd = new APPBARDATA();
            abd.cbSize = Marshal.SizeOf(abd);
            abd.hWnd = this.Handle;
            if (!fBarRegistered)
            {
                uCallBack = RegisterWindowMessage("AppBarMessage");
                abd.uCallbackMessage = uCallBack;

                uint ret = SHAppBarMessage((int)ABMsg.ABM_NEW, ref abd);
                fBarRegistered = true;

                ABSetPos();
            }
            else
            {
                SHAppBarMessage((int)ABMsg.ABM_REMOVE, ref abd);
                fBarRegistered = false;
            }
        }

        private void ABSetPos()
        {
            APPBARDATA abd = new APPBARDATA();
            abd.cbSize = Marshal.SizeOf(abd);
            abd.hWnd = this.Handle;
            abd.uEdge = (int)ABEdge.ABE_TOP;

            if (abd.uEdge == (int)ABEdge.ABE_LEFT || abd.uEdge == (int)ABEdge.ABE_RIGHT) 
            {
                abd.rc.top = 0;
                abd.rc.bottom = SystemInformation.PrimaryMonitorSize.Height;
                if (abd.uEdge == (int)ABEdge.ABE_LEFT) 
                {
                    abd.rc.left = 0;
                    abd.rc.right = Size.Width;
                }
                else 
                {
                    abd.rc.right = SystemInformation.PrimaryMonitorSize.Width;
                    abd.rc.left = abd.rc.right - Size.Width;
                }

            }
            else 
            {
                abd.rc.left = 0;
                abd.rc.right = SystemInformation.PrimaryMonitorSize.Width;
                if (abd.uEdge == (int)ABEdge.ABE_TOP) 
                {
                    abd.rc.top = 0;
                    abd.rc.bottom = Size.Height;
                }
                else 
                {
                    abd.rc.bottom = SystemInformation.PrimaryMonitorSize.Height;
                    abd.rc.top = abd.rc.bottom - Size.Height;
                }
            }

            // Query the system for an approved size and position. 
            SHAppBarMessage((int)ABMsg.ABM_QUERYPOS, ref abd); 

            // Adjust the rectangle, depending on the edge to which the 
            // appbar is anchored. 
            switch (abd.uEdge) 
            { 
                case (int)ABEdge.ABE_LEFT: 
                    abd.rc.right = abd.rc.left + Size.Width;
                    break; 
                case (int)ABEdge.ABE_RIGHT: 
                    abd.rc.left= abd.rc.right - Size.Width;
                    break; 
                case (int)ABEdge.ABE_TOP: 
                    abd.rc.bottom = abd.rc.top + Size.Height;
                    break; 
                case (int)ABEdge.ABE_BOTTOM: 
                    abd.rc.top = abd.rc.bottom - Size.Height;
                    break; 
            }

            // Pass the final bounding rectangle to the system. 
            SHAppBarMessage((int)ABMsg.ABM_SETPOS, ref abd); 

            // Move and size the appbar so that it conforms to the 
            // bounding rectangle passed to the system. 
            MoveWindow(abd.hWnd, abd.rc.left, abd.rc.top, 
                abd.rc.right - abd.rc.left, abd.rc.bottom - abd.rc.top, true); 
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == uCallBack)
            {
                switch(m.WParam.ToInt32())
                {
                    case (int)ABNotify.ABN_POSCHANGED:
                        ABSetPos();
                        break;
                }
            }
			try {
				base.WndProc(ref m);
			} catch (Exception) {
				
				 
			}
            
        }

        protected override System.Windows.Forms.CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style &= (~0x00C00000); // WS_CAPTION
                cp.Style &= (~0x00800000); // WS_BORDER
                cp.ExStyle = 0x00000080 | 0x00000008; // WS_EX_TOOLWINDOW | WS_EX_TOPMOST
                return cp;
            }
        }
        #endregion
		void MainFormFormClosing(object sender, FormClosingEventArgs e)
		{
			RegisterBar(); //不管用
		}
//		private AutoSizeFormClass asc=new AutoSizeFormClass();
//		void MainFormResize(object sender, EventArgs e)
//		{
//			asc.controlAutoSize(this);
//		}
        
		[DllImport("kernel32.dll",EntryPoint="GetSystemPowerStatus")]   //win32 api
        private static extern void GetSystemPowerStatus(ref SYSTEM_POWER_STATUS lpSystemPowerStatus);   

         public struct SYSTEM_POWER_STATUS    //结构体
         {   
             public Byte ACLineStatus;                //0 = offline, 1 = Online, 255 = UnKnown Status.   
             public Byte BatteryFlag;   
            public Byte BatteryLifePercent;   
            public Byte Reserved1;   
            public int BatteryLifeTime;   
             public int BatteryFullLifeTime;   
         }  

        /// <summary>
        ///  获取系统电量百分比
        /// </summary>
        /// <returns></returns>
        public static float GetSystemPower()
        {
            SYSTEM_POWER_STATUS SysPower = new SYSTEM_POWER_STATUS();
            GetSystemPowerStatus(ref SysPower);

            return SysPower.BatteryLifePercent;
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
		 // 歌词 一言 小说 RSS
		void Label4Click(object sender, EventArgs e)
		{
			 IniFiles ini=new IniFiles(Directory.GetCurrentDirectory()+"\\setting.ini");
			string type=ini.IniReadValue("hitokoto","type");
			switch (type) {
				case "1":
				label4.Text=GetWebClient("https://v1.hitokoto.cn/?encode=text");
				label4.Width= TextRenderer.MeasureText(label4.Text, label4.Font).Width; 
				label4.Location=(Point)new Size((Screen.PrimaryScreen.Bounds.Width-label4.Width)/2, this.Height/4);
				break;
				
				case "2":
				string txt=ini.IniReadValue("hitokoto","txt");
				string[] line=txt.Split('|');
				Random rd = new Random();
	            int i = rd.Next(0,line.Length);
	            label4.Text=line[i];
	            label4.Width= TextRenderer.MeasureText(label4.Text, label4.Font).Width; 
				label4.Location=(Point)new Size((Screen.PrimaryScreen.Bounds.Width-label4.Width)/2, this.Height/4);
				break;
			}
			
			
		}
		void PictureBox2Click(object sender, EventArgs e)
		{
			//切换壁纸
			 IniFiles ini=new IniFiles(Directory.GetCurrentDirectory()+"\\setting.ini");
			string name=ini.IniReadValue("wallpaper","name");
			try {
				WallPaperChange(name);
			} catch (Exception) {
				 
			}
			
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
		void PictureBox1Click(object sender, EventArgs e)
		{
			IniFiles ini=new IniFiles(Directory.GetCurrentDirectory()+"\\setting.ini");
			string flag=ini.IniReadValue("sakura","flag");
			if (flag=="1") {
				Process.Start ("SlideToShutDown.exe" ); 
			}
		}
		void Label3Click(object sender, EventArgs e)
		{
			this.Close();
		}
		void Timer1Tick(object sender, EventArgs e)
		{
			label3.Text=System.DateTime.Now.ToString("F"); 
		}
		void PictureBox3Click(object sender, EventArgs e)
		{
			
			Mute();
		}
        
		[DllImport("user32.dll")]
static extern void keybd_event(byte bVk, byte bScan, UInt32 dwFlags, UInt32 dwExtraInfo);
 
[DllImport("user32.dll")]
static extern Byte MapVirtualKey(UInt32 uCode, UInt32 uMapType);
 
private const byte VK_VOLUME_MUTE = 0xAD;
private const byte VK_VOLUME_DOWN = 0xAE;
private const byte VK_VOLUME_UP = 0xAF;
private const UInt32 KEYEVENTF_EXTENDEDKEY = 0x0001;
private const UInt32 KEYEVENTF_KEYUP = 0x0002;
 
/// <summary>
/// 改变系统音量大小，增加
/// </summary>
public void VolumeUp()
{
    keybd_event(VK_VOLUME_UP, MapVirtualKey(VK_VOLUME_UP, 0), KEYEVENTF_EXTENDEDKEY, 0);
    keybd_event(VK_VOLUME_UP, MapVirtualKey(VK_VOLUME_UP, 0), KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
}
 
/// <summary>
/// 改变系统音量大小，减小
/// </summary>
public void VolumeDown()
{
    keybd_event(VK_VOLUME_DOWN, MapVirtualKey(VK_VOLUME_DOWN, 0), KEYEVENTF_EXTENDEDKEY, 0);
    keybd_event(VK_VOLUME_DOWN, MapVirtualKey(VK_VOLUME_DOWN, 0), KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
}
 
/// <summary>
/// 改变系统音量大小，静音
/// </summary>
public void Mute()
{
    keybd_event(VK_VOLUME_MUTE, MapVirtualKey(VK_VOLUME_MUTE, 0), KEYEVENTF_EXTENDEDKEY, 0);
    keybd_event(VK_VOLUME_MUTE, MapVirtualKey(VK_VOLUME_MUTE, 0), KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
}
		 
		void Label5MouseClick(object sender, MouseEventArgs e)
		{
			
			 if (e.Button == MouseButtons.Left) {//左键
				try {
					IniFiles ini=new IniFiles(Directory.GetCurrentDirectory()+"\\setting.ini");
					string path =ini.IniReadValue("music","path");
					
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
		            
		            
		            musicplay mp=new musicplay();
					mp.PlayMusic(musicpath);
					
					label5.Text=Path.GetFileNameWithoutExtension(musicpath);
					label5.Width= TextRenderer.MeasureText(label5.Text, label5.Font).Width; 
				} catch (Exception) {
					MessageBox.Show("请设置歌曲文件夹路径，并且保证含有mp3格式歌曲");
				}	 
			} 
			//右键
    		else if(e.Button == MouseButtons.Right){
				musicplay mp=new musicplay();
				mp.StopMusic();
			} 
		}
		void PictureBox4Click(object sender, EventArgs e)
		{
			CaptureImageTool capture = new CaptureImageTool();
            if (capture.ShowDialog() == DialogResult.OK)
            {
                Image image = capture.Image;
                Clipboard.SetImage(image);
            }
		}
		 /// <summary>
          /// 生成二维码图片
          /// </summary>
          /// <param name="strMessage">要生成二维码的字符串</param>
          /// <param name="width">二维码图片宽度</param>
          /// <param name="height">二维码图片高度</param>
         /// <returns></returns>
         private Bitmap GetQRCodeByZXingNet(String strMessage,Int32 width,Int32 height)
          {
             Bitmap result = null;
             try
             {
                 BarcodeWriter barCodeWriter = new BarcodeWriter();
                 barCodeWriter.Format = BarcodeFormat.QR_CODE;
                 barCodeWriter.Options.Hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
                 barCodeWriter.Options.Hints.Add(EncodeHintType.ERROR_CORRECTION, ZXing.QrCode.Internal.ErrorCorrectionLevel.H);
                 barCodeWriter.Options.Height = height;
                 barCodeWriter.Options.Width = width;
                 barCodeWriter.Options.Margin = 0;
                 ZXing.Common.BitMatrix bm = barCodeWriter.Encode(strMessage);
                 result = barCodeWriter.Write(bm);
             }
             catch (Exception ex)
             {
                 //异常输出
             }
             return result;
         }
		void PictureBox5MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left) {//左键
				GetQRCodeByZXingNet(Clipboard.GetText(), 200, 200).Save(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)+"\\二维码.png");
			} 
			//右键
    		else if(e.Button == MouseButtons.Right){
				Clipboard.SetImage(GetQRCodeByZXingNet(Clipboard.GetText(), 200, 200));
			} 
		}
		 public static dark dk = null ; ////设置为全局变量，以便其余窗体 https://zhuanlan.zhihu.com/p/352081599
		void PictureBox6MouseClick(object sender, MouseEventArgs e)
		{	
			if (e.Button == MouseButtons.Left) {//左键
				dk=new dark();
				dk.Show();
			}
			//右键
    		else if(e.Button == MouseButtons.Right){
				dk.Close();
			} 
		}
		//调用记事本传入文本
		 
		void PictureBox7Click(object sender, EventArgs e)
		{
			IniFiles ini=new IniFiles(Directory.GetCurrentDirectory()+"\\setting.ini");
			string API_KEY=ini.IniReadValue("ocr","apikey");
			string SECRET_KEY=ini.IniReadValue("ocr","secretkey");
			 
			if (API_KEY=="") {
				MessageBox.Show("请到setting.ini设置API_KEY");
				this.Close();
				}
			var client = new Baidu.Aip.Ocr.Ocr(API_KEY, SECRET_KEY);
            client.Timeout = 60000;  // 修改超时时间
            
	          //  var result = client.GeneralEnhanced(imageToByte(pictureBox1.Image));        //本地图图片
	          var image = imageToByte(Clipboard.GetImage());
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
			string info=result.ToString();
				JsonData json=JsonMapper.ToObject(info);  //https://blog.csdn.net/DoyoFish/article/details/81976181
				JsonData token=json["words_result"];
				string txt="";
				for (int i = 0; i < token.Count; i++) {
					txt+=token[i]["words"].ToString()+"\n";	
				}
             MessageBox.Show(txt);
			 Clipboard.SetText(txt);
		}
		private byte[] imageToByte(System.Drawing.Image _image)
		{
		    MemoryStream ms = new MemoryStream();
		    _image.Save(ms,System.Drawing.Imaging.ImageFormat.Jpeg);
		    return  ms.ToArray();
		}
		void PictureBox8Click(object sender, EventArgs e)
		{
			IniFiles ini=new IniFiles(Directory.GetCurrentDirectory()+"\\setting.ini");
			string type=ini.IniReadValue("fanyi","type");
			switch (type) {
				case "1":
				string t=HttpUtility.UrlEncode(Clipboard.GetText(),System.Text.Encoding.UTF8);
				string json3=GetWebClient("https://translate.plausibility.cloud/api/v1/auto/zh/"+t);
				JsonData txt2=JsonMapper.ToObject(json3);  //https://blog.csdn.net/DoyoFish/article/details/81976181
				MessageBox.Show(txt2["translation"].ToString());
				Clipboard.SetText(txt2["translation"].ToString());
				break;
				case "2":
				string info=GetToken();
				JsonData json=JsonMapper.ToObject(info);  //https://blog.csdn.net/DoyoFish/article/details/81976181
				JsonData token=json["access_token"];
				JsonData json2=JsonMapper.ToObject(textTrans(Clipboard.GetText(),token.ToString()));  //https://blog.csdn.net/DoyoFish/article/details/81976181
				JsonData data=json2["result"];
				string txt=data["trans_result"][0]["dst"].ToString();
				MessageBox.Show(txt);
				Clipboard.SetText(txt);	
				break;
			}
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
		 	IniFiles ini=new IniFiles(Directory.GetCurrentDirectory()+"\\setting.ini");
			string API_KEY=ini.IniReadValue("fanyi","apikey");
			string SECRET_KEY=ini.IniReadValue("fanyi","secretkey");
			if (API_KEY=="") {
				MessageBox.Show("请到setting.ini设置API_KEY");
				Environment.Exit(0);
				} 
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
		
		void PictureBox9MouseClick(object sender, MouseEventArgs e)
		{
			 
			if (e.Button == MouseButtons.Left) {//左键
				
				flag=true;
				IniFiles ini=new IniFiles(Directory.GetCurrentDirectory()+"\\setting.ini");
				min=int.Parse(ini.IniReadValue("tomato","work"));
				sec=int.Parse(ini.IniReadValue("tomato","rest"));
				ts=new TimeSpan(0, min, 0);
			}
			//右键
    		else if(e.Button == MouseButtons.Right){
				 
				flag=false;
				ToolTip toolTip1 =  new  ToolTip();
				toolTip1.SetToolTip( pictureBox9, "番茄时钟" );
			} 
		}
		bool flag=false;
		int min=0;
		int sec=0;
		
	 TimeSpan ts = new TimeSpan(0, 0, 0);
		 
	async	void Timer2Tick(object sender, EventArgs e)
		{ 
			if (flag) {
				ts = ts.Subtract(new TimeSpan(0, 0, 1));
				string t=ts.ToString("T");
				ToolTip toolTip1 =  new  ToolTip();
				toolTip1.SetToolTip( pictureBox9,  t );
				Image img=pictureBox9.Image;
				pictureBox9.Image=pictureBox10.Image;
				pictureBox10.Image=img;
			if (ts.TotalSeconds < 1.0) {
					timer2.Enabled = false;
					musicplay mp=new musicplay();
					mp.PlayMusic("ding.wav");
				
					this.Refresh();
				//	Thread.Sleep(20*1000);
					await Task.Delay(sec*1000);	 
					timer2.Enabled = true;
					ts = new TimeSpan(0, min, 0);
				}
			}
			
		}
		
		 
		 
		 
	}
}
