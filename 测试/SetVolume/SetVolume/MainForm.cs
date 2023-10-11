/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 2023/10/10
 * Time: 23:11
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace SetVolume
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
			
			this.MouseWheel += label1_MouseWheel;
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		private void label1_MouseWheel(object sender, MouseEventArgs e)
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
		void Button1Click(object sender, EventArgs e)
		{			VolumeUp();
		}
		void Button2Click(object sender, EventArgs e)
		{
			VolumeDown();
		}
		void Label1Click(object sender, EventArgs e)
		{
			Mute();
		}
		 
	}
}
