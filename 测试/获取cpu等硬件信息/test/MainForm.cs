/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 2023/10/13
 * Time: 13:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenHardwareMonitor.Hardware;
using System.Diagnostics;
using System.Management;

namespace test
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
		 public class UpdateVisitor : IVisitor
       {
           public void VisitComputer(IComputer computer)
           {
               computer.Traverse(this);
           }
           public void VisitHardware(IHardware hardware)
           {
               hardware.Update();
               foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
           }
           public void VisitSensor(ISensor sensor) { }
           public void VisitParameter(IParameter parameter) { }
       }
       public void GetSystemInfo()
       {
           UpdateVisitor updateVisitor = new UpdateVisitor();
           Computer computer = new Computer();
           computer.Open();
           computer.CPUEnabled = true;
           computer.Accept(updateVisitor);
           for (int i = 0; i < computer.Hardware.Length; i++)
           {
               if (computer.Hardware[i].HardwareType == HardwareType.CPU)
               {
                   for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                   {
                       if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Level)
                            //   Console.WriteLine(computer.Hardware[i].Sensors[j].Name + ":" + computer.Hardware[i].Sensors[j].Value.ToString() + "\r");
                       	this.label1.Text=computer.Hardware[i].Sensors[j].Name + ":" + computer.Hardware[i].Sensors[j].Value.ToString() + "\r";
                   }
               }
           }
           computer.Close();
       }
       
       
//    Voltage, // V
//    Current, // A
//    Power, // W
//    Clock, // MHz
//    Temperature, // °C
//    Load, // %
//    Frequency, // Hz
//    Fan, // RPM
//    Flow, // L/h
//    Control, // %
//    Level, // %
//    Factor, // 1
//    Data, // GB = 2^30 Bytes
//    SmallData, // MB = 2^20 Bytes
//    Throughput, // B/s
//    TimeSpan, // Seconds
//    
		void Timer1Tick(object sender, EventArgs e)
		{
		 	GetSystemInfo();
		 
       
		}
	}
}
