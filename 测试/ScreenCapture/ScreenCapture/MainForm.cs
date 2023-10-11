/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 2023/10/11
 * Time: 0:17
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CSharpWin_JD.CaptureImage;

namespace ScreenCapture
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
			CaptureImageTool capture = new CaptureImageTool();
            //capture.SelectCursor = new Cursor(Properties.Resources.Arrow_M.Handle); 
           
            if (capture.ShowDialog() == DialogResult.OK)
            {
                Image image = capture.Image;
                pictureBox1.Image = CombinImage(pictureBox1.Image,image);
                                
            }
		}
		 public static Image CombinImage(Image sourceImg, Image  destImg)
        {
            Image imgBack = sourceImg ;     //相框图片  
            Image img =destImg ;      //照片图片



            //从指定的System.Drawing.Image创建新的System.Drawing.Graphics        
            Graphics g = Graphics.FromImage(imgBack);

            g.DrawImage(imgBack, 0, 0, destImg.Width, destImg.Height );      // g.DrawImage(imgBack, 0, 0, 相框宽, 相框高); 
           // g.FillRectangle(System.Drawing.Brushes.Black, 16, 16, (int)112 + 2, ((int)73 + 2));//相片四周刷一层黑色边框



            //g.DrawImage(img, 照片与相框的左边距, 照片与相框的上边距, 照片宽, 照片高);
            g.DrawImage(img, 0, 0, destImg.Width,destImg.Height);  
            GC.Collect();
            return imgBack;
        }
	}
}
