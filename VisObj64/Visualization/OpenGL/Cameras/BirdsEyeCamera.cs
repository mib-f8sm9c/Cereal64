using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace Cereal64.VisObj64.Visualization.OpenGL.Cameras
{
    public class BirdsEyeCamera : ICamera
    {
        /// <summary>
        /// Position of the camera in 3d space
        /// </summary>
        private Vector3 Position;

        /// <summary>
        /// Position of the target of the camera in 3d space
        /// </summary>
        private Vector3 Target;

        /// <summary>
        /// Vector pointing in direction of the top of the camera - Not necessarily normalized
        /// </summary>
        private Vector3 Up;

        /// <summary>
        /// Lock the Up vector, prevents the camera from rolling
        /// </summary>
        private bool LockUp;

        private const float FOV = 60.0f; //Degrees, make change-able later

        /// <summary>
        /// Last recorded position of mouse
        /// </summary>
        private Vector2 MousePosition;

        /// <summary>
        /// Retains the mouse clicked state
        /// </summary>
        private bool MouseClicked;

        /// <summary>
        /// Called when the camera position changes
        /// </summary>
        public event EventHandler CameraUpdated;

        /// <summary>
        /// Normalized vector pointing in forward direction (Z)
        /// </summary>
        private Vector3 ForwardDirection
        {
            get
            {
                Vector3 forward = Target - Position;
                forward.Normalize();
                return forward;
            }
        }

        /// <summary>
        /// Normalized vector pointing in up direction (Y)
        /// </summary>
        private Vector3 UpDirection
        {
            get
            {
                Vector3 up = new Vector3(Up);
                up.Normalize();
                return up;
            }
        }

        /// <summary>
        /// Normalized vector pointing in left direction (X)
        /// </summary>
        private Vector3 LeftDirection
        {
            get
            {
                Vector3 leftVector = Vector3.Cross(UpDirection, ForwardDirection);
                return leftVector;
            }
        }

        public BirdsEyeCamera()
        {
            Reset();
        }

        /// <summary>
        /// Reset the camera to a default position
        /// </summary>
        public void Reset()
        {
            Position = new Vector3(0, 100f, 0);
            Target = new Vector3(0, 0f, 0);
            Up = new Vector3(1f, 0f, 0f);

            LockUp = true;
        }

        /// <summary>
        /// Set OpenGL to focus on the camera position/target/up
        /// </summary>
        public void Focus()
        {
            GL.LoadIdentity();
            Matrix4 lookAtMatrix = Matrix4.LookAt(Position, Target, Up);
            GL.LoadMatrix(ref lookAtMatrix);
        }

        /// <summary>
        /// Translate the camera by the given vector
        /// </summary>
        /// <param name="movement">Distance/direction to move the camera</param>
        /// <param name="delayUpdate">If true, camera will not call the update event</param>
        public void Translate(Vector3 movement, bool delayUpdate = false)
        {
            Position.X += movement.X;
            Position.Y += movement.Y;
            Position.Z += movement.Z;
            Target.X += movement.X;
            Target.Y += movement.Y;
            Target.Z += movement.Z;

            if (!delayUpdate)
                updateCamera();
        }

        /// <summary>
        /// Rotate the camera by the parameters given
        /// </summary>
        /// <param name="rotationAxis">Axis about which to rotate the camera</param>
        /// <param name="rotationVal">The angle (in radians) to rotate</param>
        /// <param name="delayUpdate">If true, camera will not call the update event</param>
        public void Rotate(Vector3 rotationAxis, float rotationVal, bool delayUpdate = false)
        {
            //Rotate the look at & up
            Vector3 lookAt = Target - Position;
            Quaternion rotation = Quaternion.FromAxisAngle(rotationAxis, rotationVal);
            Vector3 newLookAt = Vector3.Transform(lookAt, rotation);
            Vector3 newUp = Vector3.Transform(Up, rotation);

            Target = newLookAt + Position;
            if(!LockUp)
                Up = newUp;

            if(!delayUpdate)
                updateCamera();
        }

        /// <summary>
        /// Call the CameraUpdated event
        /// </summary>
        private void updateCamera()
        {
            if (this.CameraUpdated != null)
                this.CameraUpdated(this, null);
        }

        public void OnKeyDown(KeyEventArgs e)
        {
            float speed = 51f;
            if (((int)e.Modifiers & (int)Keys.Shift) == (int)Keys.Shift) //Doesn't work
                speed = 1;

            //Move camera here
            if (e.KeyData == Keys.W)
                Translate(ForwardDirection * speed);

            if (e.KeyData == Keys.S)
                Translate(ForwardDirection * -speed);

            if (e.KeyData == Keys.A)
                Translate(LeftDirection * speed);

            if (e.KeyData == Keys.D)
                Translate(LeftDirection * -speed);

            if (e.KeyData == Keys.Q)
                Translate(UpDirection * speed);

            if (e.KeyData == Keys.E)
                Translate(UpDirection * -speed);

            if (e.KeyData == Keys.Z)
                Rotate(Up, -0.1f);

            if (e.KeyData == Keys.C)
                Rotate(Up, 0.1f);

            if (e.KeyData == Keys.X)
                Rotate(LeftDirection, -0.1f);

            if (e.KeyData == Keys.V)
                Rotate(LeftDirection, 0.1f);
        }

        public void OnMouseDown(MouseEventArgs e)
        {
            MouseClicked = true;
            MousePosition = new Vector2(e.X, e.Y);
        }

        public void OnMouseUp(MouseEventArgs e)
        {
            MouseClicked = false;
        }

        public void OnMouseMove(MouseEventArgs e)
        {
            if (MouseClicked)
            {
                float distChangedX = (e.Y - MousePosition.Y) * Position.Y / 100;
                float distChangedZ = -(e.X - MousePosition.X) * Position.Y / 100;
                Translate(new Vector3(distChangedX, 0, distChangedZ));
                //float angleChangeX = FOV * (float)Math.PI / 180 * (e.X - MousePosition.X) / 200; //Needs to be width/height?
                //float angleChangeY = -FOV * (float)Math.PI / 180 * (e.Y - MousePosition.Y) / 200; //There's probably a good way to do this

                //if (Math.Abs(angleChangeX) > 0)
                //{
                //    Rotate(UpDirection, angleChangeX, true);
                //}

                //if (Math.Abs(angleChangeY) > 0)
                //{
                //    Rotate(LeftDirection, angleChangeY, true);
                //}

                //if (Math.Abs(angleChangeX) > 0 || Math.Abs(angleChangeY) > 0)
                //    updateCamera();

                MousePosition = new Vector2(e.X, e.Y);
            }
        }

        public void OnMouseScroll(MouseEventArgs e)
        {
            double yMod = 1;
            if (e.Delta > 0)
            {
                yMod = Math.Pow(0.9, e.Delta / 120);
                Translate(new Vector3(0, Position.Y * (float)(yMod - 1), 0));
            }
            else
            {
                yMod = Math.Pow(1.1, e.Delta / 120);
                Translate(new Vector3(0, Position.Y * (float)(1 - yMod), 0));
            }
        }

    }
}
