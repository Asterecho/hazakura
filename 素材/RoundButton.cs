/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 2023/10/10
 * Time: 13:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace hazakura
{
	public enum ControlState { Hover , Normal, Pressed }
    public class RoundButton : Button
    {
        
        private int radius;//半径 
        private Color _baseColor = Color.FromArgb(51, 161, 224);//基颜色
        private Color _hoverColor= Color.FromArgb(51, 0, 224);//基颜色
        private Color _normalColor = Color.FromArgb(0, 161, 224);//基颜色
        private Color _pressedColor = Color.FromArgb(51, 161, 0);//基颜色
        //圆形按钮的半径属性
        [CategoryAttribute("布局"), BrowsableAttribute(true), ReadOnlyAttribute(false)]
        public int Radius
        {
            set
            {
                radius = value;
                this.Invalidate();
            }
            get
            {
                return radius;
            }
        }
        [DefaultValue(typeof(Color), "51, 161, 224")]
        public Color NormalColor
        {
            get
            {
                return this._normalColor;
            }
            set
            {
                this._normalColor = value;
                this.Invalidate();
            }
        }
      //  [DefaultValue(typeof(Color), "220, 80, 80")]
        public Color HoverColor {
            get
            {
                return this._hoverColor;
            }
            set
            {
                this._hoverColor = value;
                this.Invalidate();
            }
        }
 
      //  [DefaultValue(typeof(Color), "251, 161, 0")]
        public Color PressedColor {
            get
            {
                return this._pressedColor;
            }
            set
            {
                this._pressedColor = value;
                this.Invalidate();
            }
        }
        public ControlState ControlState { get; set; }
        protected override void OnMouseEnter(EventArgs e)//鼠标进入时
        {
            base.OnMouseEnter(e);
            ControlState = ControlState.Hover;//正常
        }
        protected override void OnMouseLeave(EventArgs e)//鼠标离开
        {
            base.OnMouseLeave(e);
            ControlState = ControlState.Normal;//正常
        }
        protected override void OnMouseDown(MouseEventArgs e)//鼠标按下
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left && e.Clicks == 1)//鼠标左键且点击次数为1
            {
                ControlState = ControlState.Pressed;//按下的状态
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)//鼠标弹起
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left && e.Clicks == 1)
            {
                if (ClientRectangle.Contains(e.Location))//控件区域包含鼠标的位置
                {
                    ControlState = ControlState.Hover;
                }
                else
                {
                    ControlState = ControlState.Normal;
                }
            }
        }
        public RoundButton()
        {
            Radius = 15;
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.ControlState = ControlState.Normal;
            this.SetStyle(
             ControlStyles.UserPaint |  //控件自行绘制，而不使用操作系统的绘制
             ControlStyles.AllPaintingInWmPaint | //忽略擦出的消息，减少闪烁。
             ControlStyles.OptimizedDoubleBuffer |//在缓冲区上绘制，不直接绘制到屏幕上，减少闪烁。
             ControlStyles.ResizeRedraw | //控件大小发生变化时，重绘。                  
             ControlStyles.SupportsTransparentBackColor, true);//支持透明背景颜色
        }
 
        private Color GetColor(Color colorBase, int a, int r, int g, int b)
        {
            int a0 = colorBase.A;
            int r0 = colorBase.R;
            int g0 = colorBase.G;
            int b0 = colorBase.B;
            if (a + a0 > 255) { a = 255; } else { a = Math.Max(a + a0, 0); }
            if (r + r0 > 255) { r = 255; } else { r = Math.Max(r + r0, 0); }
            if (g + g0 > 255) { g = 255; } else { g = Math.Max(g + g0, 0); }
            if (b + b0 > 255) { b = 255; } else { b = Math.Max(b + b0, 0); }
 
            return Color.FromArgb(a, r, g, b);
        }
 
        //重写OnPaint
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
            base.OnPaintBackground(e);
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
 
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
 
            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
            var path = GetRoundedRectPath(rect, radius);
 
            this.Region = new Region(path);
 
            Color baseColor;
            //Color borderColor;
            //Color innerBorderColor = this._baseColor;//Color.FromArgb(200, 255, 255, 255); ;
 
            switch (ControlState)
            {
                case ControlState.Hover:
                    baseColor = this.HoverColor;
                    break;
                case ControlState.Pressed:
                    baseColor = this.PressedColor;
                    break;
                case ControlState.Normal:
                    baseColor = this.NormalColor;
                    break;
                default:
                    baseColor = this.NormalColor;
                    break;
            }
 
            using (SolidBrush b = new SolidBrush(baseColor))
            {
                e.Graphics.FillPath(b, path);
                Font fo = new Font("宋体", 10.5F);
                Brush brush = new SolidBrush(this.ForeColor);
                StringFormat gs = new StringFormat();
                gs.Alignment = StringAlignment.Center; //居中
                gs.LineAlignment = StringAlignment.Center;//垂直居中
                e.Graphics.DrawString(this.Text, fo, brush, rect, gs);
                //  e.Graphics.DrawPath(p, path);
            }
        }
        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            int diameter = radius;
            Rectangle arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));
            GraphicsPath path = new GraphicsPath();
            path.AddArc(arcRect, 180, 90);
            arcRect.X = rect.Right - diameter;
            path.AddArc(arcRect, 270, 90);
            arcRect.Y = rect.Bottom - diameter;
            path.AddArc(arcRect, 0, 90);
            arcRect.X = rect.Left;
            path.AddArc(arcRect, 90, 90);
            path.CloseFigure();
            return path;
        }
 
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
        }
 
        
       
    }
}
