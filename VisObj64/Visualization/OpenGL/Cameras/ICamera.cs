using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cereal64.VisObj64.Visualization.OpenGL.Cameras
{
    public interface ICamera
    {
        void OnKeyDown(KeyEventArgs e);
        void OnMouseDown(MouseEventArgs e);
        void OnMouseUp(MouseEventArgs e);
        void OnMouseMove(MouseEventArgs e);

        event EventHandler CameraUpdated;

        void Focus();
    }
}
