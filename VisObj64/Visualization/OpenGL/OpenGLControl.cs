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

namespace VisObj64.Visualization.OpenGL
{
    public partial class OpenGLControl : UserControl
    {
        public bool OpenGLLoaded { get; private set; }

        public List<VO64GraphicsCollection> GraphicsCollections;

        public ICamera Camera
        {
            get { return _camera; }
            set { if(_camera != null) _camera.CameraUpdated -= CameraUpdated; _camera = value; _camera.CameraUpdated += CameraUpdated; }
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
        }

        private void Camera_CameraUpdated(object sender, EventArgs e)
        {
            _glDisplay.Invalidate();//gl_DrawScene();
        }

        private void glDisplay_Load(object sender, EventArgs e)
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;
            
            MakeCurrent();

            //If there are unitialized-related errors, it's probably setting this too early
            SetupViewport();

            TestMethod();
        }

        private void TestMethod()
        {
            //Create something to view
            VO64GraphicsCollection collection = new VO64GraphicsCollection();
            VO64GraphicsElement element = VO64GraphicsElement.CreateNewElement();

            element.AddVertex(new VO64SimpleVertex(-0.5f, 0.5f, 0f, 0.0f, 0.0f, -0.5f, 0.5f, 0f));
            element.AddVertex(new VO64SimpleVertex(-0.5f, -0.5f, 0f, 0.0f, 1.0f, -0.5f, -0.5f, 1f));
            element.AddVertex(new VO64SimpleVertex(0.5f, -0.5f, 0f, 1.0f, 1.0f, 0.5f, -0.5f, 1f));
            element.AddVertex(new VO64SimpleVertex(0.5f, 0.5f, 0f, 1.0f, 0.0f, 0.5f, 0.5f, 0f));
            element.AddTriangle(new VO64SimpleTriangle(0, 1, 2));
            element.AddTriangle(new VO64SimpleTriangle(2, 3, 0));

            collection.Add(element);
            this.GraphicsCollections.Add(collection);

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
            MakeCurrent();

            gl_InitRenderer();
            gl_ResizeScene(_glDisplay.Width, _glDisplay.Height);
        }

        public void ReDrawGLContent()
        {
            if (_glDisplay.Context != null)
            {
                MakeCurrent();

                _glDisplay.Invalidate();
            }

            //Redraw
        }

        private void MakeCurrent()
        {
            _glDisplay.MakeCurrent();
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
            Camera.OnMouseDown(e);

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

        private void CameraUpdated(object sender, EventArgs e)
        {
            //Update the screen
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
	        GL.CullFace(CullFaceMode.Back);

	        GL.Enable(EnableCap.Blend);
	        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

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
