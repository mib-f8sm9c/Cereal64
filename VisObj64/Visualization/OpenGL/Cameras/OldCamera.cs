using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;

namespace Cereal64.VisObj64.Visualization.OpenGL.Cameras
{
    public class OldCamera : ICamera
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double TX { get; set; }
        public double TY { get; set; }
        public double TZ { get; set; }

        public double LX { get; set; }
        public double LY { get; set; }
        public double LZ { get; set; }

        public double AngleX { get; set; }
        public double AngleY { get; set; }

        public int MouseX { get; set; }
        public int MouseY { get; set; }

        public event EventHandler CameraUpdated;

        public OldCamera()
        {
            Reset();
        }

        public void Reset()
        {
            AngleX = 0;
            AngleY = 0;
            X = 0f;
            Y = 0f;
            Z = -5f;
            LX = 0;
            LY = 0;
            LZ = 1.0f;
        }


        public void OnKeyDown(KeyEventArgs e)
        {
            //Move camera here
            if (e.KeyData == Keys.W)
                Movement(false, 6.0f);

            if (e.KeyData == Keys.S)
                Movement(false, -6.0f);

            if (e.KeyData == Keys.A)
                Movement(true, -6.0f);

            if (e.KeyData == Keys.D)
                Movement(true, 6.0f);

            if (e.KeyData == Keys.T)
                Movement(false, 24.0f);

            if (e.KeyData == Keys.G)
                Movement(false, -24.0f);

            if (e.KeyData == Keys.F)
                Movement(true, -24.0f);

            if (e.KeyData == Keys.H)
                Movement(true, 24.0f);
        }

        public void OnMouseDown(MouseEventArgs e)
        {

        }

        public void OnMouseUp(MouseEventArgs e)
        {

        }

        public void OnMouseMove(MouseEventArgs e)
        {

        }

        public void OnMouseScroll(MouseEventArgs e)
        {

        }

        public void Focus()
        {
            GL.LoadIdentity();
            gl_LookAt(X, Y, Z, X + LX, Y + LY, Z + LZ);
        }

        public void Movement(bool strafe, float speed)
        {
            if (!strafe)
            {
                X += LX * 0.025f * speed;
                Y += LY * 0.025f * speed;
                Z += LZ * 0.025f * speed;
            }
            else
            {
                X += (float)Math.Cos(AngleX) * (0.025f * speed);
                Z += (float)Math.Sin(AngleX) * (0.025f * speed);
            }

            updateCamera();
        }

        private void updateCamera()
        {
            if (this.CameraUpdated != null)
                this.CameraUpdated(this, null);
        }

        public void MouseMove(int x, int y)
        {
            AngleX += (0.01f * (x - MouseX));
            AngleY -= (0.01f * (y - MouseY));

            Orientation(AngleX, AngleY);

            MouseX = x;
            MouseY = y;

            updateCamera();
        }

        public void Orientation(double angle, double angle2)
        {
            LX = (double)Math.Sin(angle);
            LY = angle2;
            LZ = (double)-Math.Cos(angle);
        }

        void gl_LookAt(double p_EyeX, double p_EyeY, double p_EyeZ, double p_CenterX, double p_CenterY, double p_CenterZ)
        {
            double l_X = p_EyeX - p_CenterX;
            double l_Y = p_EyeY - p_CenterY;
            double l_Z = p_EyeZ - p_CenterZ;

            if (l_X == l_Y && l_Y == l_Z && l_Z == 0.0f) return;

            if (l_X == l_Z && l_Z == 0.0f)
            {
                if (l_Y < 0.0f)
                    GL.Rotate(-90.0f, 1, 0, 0);
                else
                    GL.Rotate(90.0f, 1, 0, 0);
                GL.Translate(-l_X, -l_Y, -l_Z);
                return;
            }

            double l_rX = 0.0f;
            double l_rY = 0.0f;

            double l_hA = (l_X == 0.0f) ? l_Z : hypot(l_X, l_Z);
            double l_hB;
            if (l_Z == 0.0f)
                l_hB = hypot(l_X, l_Y);
            else
                l_hB = (l_Y == 0.0f) ? l_hA : hypot(l_Y, l_hA);

            l_rX = Math.Asin(l_Y / l_hB) * (180 / Math.PI);
            l_rY = Math.Asin(l_X / l_hA) * (180 / Math.PI);

            GL.Rotate(l_rX, 1, 0, 0);
            if (l_Z < 0.0f)
                l_rY += 180.0f;
            else
                l_rY = 360.0f - l_rY;

            GL.Rotate(l_rY, 0, 1, 0);
            GL.Translate(-p_EyeX, -p_EyeY, -p_EyeZ);
        }

        private static double hypot(double a, double b)
        {
            return Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
        }

    }
}
