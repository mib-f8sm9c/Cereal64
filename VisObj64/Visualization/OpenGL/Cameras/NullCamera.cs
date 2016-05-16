using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cereal64.VisObj64.Visualization.OpenGL.Cameras
{
    public class NullCamera : ICamera
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double TX { get; set; }
        public double TY { get; set; }
        public double TZ { get; set; }

        public void OnKeyDown(KeyEventArgs e)
        {

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

        public event EventHandler CameraUpdated;

        public void Focus()
        {
            //Do nothing
            CameraUpdated(this, new EventArgs());
        }
    }
}
