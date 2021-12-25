using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HexMineSweeper
{
    public class HexButton : PictureBox
    {
        public int rightButtonState = 0;
        public int iButton, jButton;

        public HexButton() {}

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            OnSizeChanged(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            using (GraphicsPath gp = new GraphicsPath())
            {
                RectangleF rect = ClientRectangle;
                PointF position = ClientRectangle.Location;
                rect.Inflate(-1, -1);
                PointF[] points = new PointF[6];

                float R = rect.Width / 2;
                float r = rect.Height;

                points[0] = new PointF(position.X, position.Y + r / 2);
                points[1] = new PointF(position.X + R / 2, position.Y);
                points[2] = new PointF(position.X + R + R / 2, position.Y);
                points[3] = new PointF(position.X + R * 2, position.Y + r / 2);
                points[4] = new PointF(position.X + R + R / 2, position.Y + r);
                points[5] = new PointF(position.X + R / 2, position.Y + r);

                gp.AddPolygon(points);
                Region = new Region(gp);
            }
        }
    }
}
