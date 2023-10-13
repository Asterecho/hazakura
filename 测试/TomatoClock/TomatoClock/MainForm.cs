/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 2023/10/12
 * Time: 0:12
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;

namespace TomatoClock
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
		TimeSpan ts = new TimeSpan(0, 1, 0);
		void Timer1Tick(object sender, EventArgs e)
		{
			ts = ts.Subtract(new TimeSpan(0, 0, 1));
				string t=ts.ToString("T");
				label1.Text=t;
			if (ts.TotalSeconds < 1.0) {
					timer1.Enabled = false;
					 
					this.Refresh();
					Thread.Sleep(20*1000);
					 
					timer1.Enabled = true;
					ts = new TimeSpan(0, 1, 0);
				}
		}
		void Button1Click(object sender, EventArgs e)
		{ 
			Bitmap map = new Bitmap(pictureBox1.Image);
			pictureBox1.Image=img_color_gradation(map,100,0,0);
 
		}
		
		public static unsafe Bitmap img_color_gradation(Bitmap src, int r, int g, int b)  
{  
    int width = src.Width;  
    int height = src.Height;  
    Bitmap back = new Bitmap(width, height);  
    Rectangle rect = new Rectangle(0, 0, width, height);  
    //这种速度最快  
    BitmapData bmpData = src.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);//24位rgb显示一个像素，即一个像素点3个字节，每个字节是BGR分量。Format32bppRgb是用4个字节表示一个像素  
    byte* ptr = (byte*)(bmpData.Scan0);  
    for (int j = 0; j < height; j++)  
    {  
        for (int i = 0; i < width; i++)  
        {  
            //ptr[2]为r值，ptr[1]为g值，ptr[0]为b值  
            int red = ptr[2] + r; if (red > 255) red = 255; if (red < 0) red = 0;  
            int green = ptr[1] + g; if (green > 255) green = 255; if (green < 0) green = 0;  
            int blue = ptr[0] + b; if (blue > 255) blue = 255; if (blue < 0) blue = 0;  
            back.SetPixel(i, j, Color.FromArgb(red, green, blue));  
            ptr += 3; //Format24bppRgb格式每个像素占3字节  
        }  
        ptr += bmpData.Stride - bmpData.Width * 3;//每行读取到最后“有用”数据时，跳过未使用空间XX  
    }  
    src.UnlockBits(bmpData);  
    return back;  
}  
	}
}
