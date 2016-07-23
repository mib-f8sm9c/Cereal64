using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using Cereal64.VisObj64.Visualization.OpenGL.Cameras;
using Cereal64.VisObj64.Data.OpenGL;
using System.Collections.ObjectModel;

namespace VisObj64.Visualization.OpenGL
{
    public partial class OpenGLControl : UserControl
    {
        public enum MouseFunction
        {
            Camera,
            Select
        }

        private MouseFunction _mouseFunction;

        public bool OpenGLLoaded { get; private set; }

        public List<VO64GraphicsCollection> GraphicsCollections;

        public ReadOnlyCollection<VO64GraphicsElement> SelectedElements
        {
            get { return _selectedElements.AsReadOnly(); }
        }
        private List<VO64GraphicsElement> _selectedElements;

        public ICamera Camera
        {
            get { return _camera; }
            set { if (_camera != null) _camera.CameraUpdated -= Camera_CameraUpdated; _camera = value; _camera.CameraUpdated += Camera_CameraUpdated; }
        }
        private ICamera _camera;

        [CategoryAttribute("Appearance"),
        DescriptionAttribute("Color that the OpenGL control uses to represent empty space")]
        public Color ClearColor
        {
            get;
            set;
        }

        public OpenGLControl()
        {
            //Set defaults
            ClearColor = Color.CornflowerBlue;

            Camera = new NewCamera();
            Camera.CameraUpdated += new EventHandler(Camera_CameraUpdated);

            InitializeComponent();

            GraphicsCollections = new List<VO64GraphicsCollection>();
            _selectedElements = new List<VO64GraphicsElement>();

            _mouseFunction = MouseFunction.Camera;
        }

        private void Camera_CameraUpdated(object sender, EventArgs e)
        {
            //Repaint the contrl
            _glDisplay.Invalidate();
            _glDisplay.Update();
        }

        private void glDisplay_Load(object sender, EventArgs e)
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime || DesignMode) return;
            
            MakeCurrent();

            //If there are unitialized-related errors, it's probably setting this too early
            SetupViewport();
        }

        private void glDisplay_Paint(object sender, PaintEventArgs e)
        {
            if (!OpenGLLoaded)
                return;

            MakeCurrent();

            GL.ClearColor(ClearColor);
            gl_DrawScene();

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Lighting);

            _glDisplay.SwapBuffers();
        }

        private void SetupViewport()
        {
            //MakeCurrent();

            gl_InitRenderer();
            gl_ResizeScene(_glDisplay.Width, _glDisplay.Height);
        }

        public void ReDrawGLContent()
        {
            if (_glDisplay.Context != null)
            {
                MakeCurrent();

                _glDisplay.Invalidate();
                _glDisplay.Update();
            }

            //Redraw
        }

        private void MakeCurrent()
        {
            _glDisplay.MakeCurrent();
        }

        private uint[] selectBuffer = new uint[16];

        private void SelectAtMouse(bool addSelection)
        {
            //http://www.glprogramming.com/red/chapter13.html
            GL.SelectBuffer(16, selectBuffer);
            GL.RenderMode(RenderingMode.Select);
            int hits;
            gl_DrawScene();
            hits = GL.RenderMode(RenderingMode.Render);

            //look through the select buffer for the closest object to the camera?
        }

        private void SelectElement(VO64GraphicsElement element)
        {
            //Check if element exists?
            if (!_selectedElements.Contains(element))
            {
                _selectedElements.Add(element);
                element.Selected = true;
            }
        }

        private void UnselectElement(VO64GraphicsElement element)
        {
            //Check if element exists?
            if (_selectedElements.Contains(element))
            {
                _selectedElements.Remove(element);
                element.Selected = false;
            }
        }

        private void OpenGLControl_Resize(object sender, EventArgs e)
        {
            if (OpenGLLoaded)
            {
                MakeCurrent();

                gl_ResizeScene(_glDisplay.Width, _glDisplay.Height);
            }
        }

        private void glDisplay_KeyDown(object sender, KeyEventArgs e)
        {
            if (!OpenGLLoaded)
                return;

            MakeCurrent();


            bool hasFocus = this.Focused;
            if (!hasFocus)
            {
                foreach (Control ctl in this.Controls)
                {
                    if (ctl.Focused)
                    {
                        hasFocus = true;
                        break;
                    }
                }
            }
            if (hasFocus)
            {
                //Any camera code or interaction code goes here
                Camera.OnKeyDown(e);
            }


        }

        private void glDisplay_MouseDown(object sender, MouseEventArgs e)
        {
            if (!OpenGLLoaded)
                return;

            MakeCurrent();

            //Any camera code or interaction code goes here
            switch(_mouseFunction)
            {
                case MouseFunction.Camera:
                    Camera.OnMouseDown(e);
                    break;
                case MouseFunction.Select:
                    OpenTK.Input.KeyboardState keyboard = OpenTK.Input.Keyboard.GetState();
                    SelectAtMouse(keyboard.IsKeyDown(OpenTK.Input.Key.ShiftLeft) || keyboard.IsKeyDown(OpenTK.Input.Key.ShiftRight));
                    break;
            }
        }

        private void glDisplay_MouseUp(object sender, MouseEventArgs e)
        {
            if (!OpenGLLoaded)
                return;

            MakeCurrent();

            //Any camera code or interaction code goes here
            Camera.OnMouseUp(e);

        }

        private void glDisplay_MouseMove(object sender, MouseEventArgs e)
        {
            if (!OpenGLLoaded)
                return;

            //Any camera code or interaction code goes here
            Camera.OnMouseMove(e);

        }

        #region draw.c functions

        //These functions draw heavily from the OpenGL code used in XDaniel's Starfox64Toolkit, serious
        // thanks go out to him for this section.

        void gl_Perspective(double fovy, double aspect, double zNear, double zFar)
        {
	        double xmin, xmax, ymin, ymax;

	        ymax = zNear * Math.Tan(fovy * Math.PI / 360.0);
	        ymin = -ymax;
	        xmin = ymin * aspect;
	        xmax = ymax * aspect;

	        GL.Frustum(xmin, xmax, ymin, ymax, zNear, zFar);
        }

        void gl_InitRenderer()
        {
            GL.MatrixMode(MatrixMode.Projection);

            int w = _glDisplay.Width;
            int h = _glDisplay.Height;
            GL.LoadIdentity();
            GL.Ortho(0, w, 0, h, -1, 1); // Bottom-left corner pixel has coordinate (0, 0)
            GL.Viewport(0, 0, w, h); // Use all of the glControl painting area
	        gl_Perspective(60.0f, (float)Width / (float)Height, 0.1f, 100.0f);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

	        GL.ShadeModel(ShadingModel.Smooth);
	        GL.Enable(EnableCap.PointSmooth);
	        GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);

	        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

	        //GL.ClearColor(0.2f, 0.5f, 0.7f, 1.0f);

            //Having issues with one computer at this line, it's generating an AccessViolationException. Appears to work okay without it
	        //GL.ClearDepth(5.0f);

	        GL.DepthFunc(DepthFunction.Lequal);
	        GL.Enable(EnableCap.DepthTest);

	        GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

	        GL.Light(LightName.Light0, LightParameter.Ambient, new OpenTK.Graphics.Color4(1.0f, 1.0f, 1.0f, 1.0f));
            GL.Light(LightName.Light0, LightParameter.Diffuse, new OpenTK.Graphics.Color4(1.0f, 1.0f, 1.0f, 1.0f));
            GL.Light(LightName.Light0, LightParameter.Specular, new OpenTK.Graphics.Color4(1.0f, 1.0f, 1.0f, 1.0f));
            GL.Light(LightName.Light0, LightParameter.Position, new OpenTK.Graphics.Color4(1.0f, 1.0f, 1.0f, 1.0f));
	        GL.Enable(EnableCap.Light0);

	        GL.Enable(EnableCap.Lighting);
	        GL.Enable(EnableCap.Normalize);

	        GL.Disable(EnableCap.CullFace);
	        //GL.CullFace(CullFaceMode.FrontAndBack);

	        GL.Enable(EnableCap.Blend);
	        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            //Note: This helps with the alpha problems, but it's not a good fix
            //GL.AlphaFunc(AlphaFunction.Gequal, 0.75f);
            //GL.Enable(EnableCap.AlphaTest);

            OpenGLLoaded = true;
        }

        void gl_ResizeScene(int Width, int Height)
        {
            if (!OpenGLLoaded)
                return;

	        GL.Viewport(0, 0, Width, Height);

	        GL.MatrixMode(MatrixMode.Projection);
	        GL.LoadIdentity();
	        gl_Perspective(60.0f, (float)Width / (float)Height, 0.1f, 100.0f);

	        GL.MatrixMode(MatrixMode.Modelview);
	        GL.LoadIdentity();
        }

        void gl_DrawScene()
        {
            if (!OpenGLLoaded)
                return;

	        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Camera.Focus();

            GL.PushMatrix();

            //Draw the scene here
            foreach (VO64GraphicsCollection collection in GraphicsCollections)
            {
                collection.Draw();
            }

            // Restore the state
            GL.PopMatrix();
        }

        #endregion
    }
}
