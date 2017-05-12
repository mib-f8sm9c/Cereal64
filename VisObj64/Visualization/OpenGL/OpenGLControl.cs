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

        public MouseFunction MouseMode { get; set; }

        public bool OpenGLLoaded { get; private set; }

        public List<VO64GraphicsCollection> GraphicsCollections;

        public ReadOnlyCollection<VO64GraphicsElement> SelectedElements
        {
            get { return _selectedElements.AsReadOnly(); }
        }
        private List<VO64GraphicsElement> _selectedElements;

        private float _minSelectBoxX, _minSelectBoxY, _minSelectBoxZ, _maxSelectBoxX, _maxSelectBoxY, _maxSelectBoxZ;

        private bool CanDisplaySelectBox()
        {
            return !(_minSelectBoxX == _maxSelectBoxX || _minSelectBoxY == _maxSelectBoxY || _minSelectBoxZ == _maxSelectBoxZ);
        }

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

            MouseMode = MouseFunction.Camera;
            
            _glDisplay.MouseWheel += new MouseEventHandler(_glDisplay_MouseWheel);
        }

        private void _glDisplay_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!OpenGLLoaded)
                return;

            MakeCurrent();

            Camera.OnMouseScroll(e);
        }

        public void ClearGraphics()
        {
            for (int i = GraphicsCollections.Count - 1; i >= 0; i--)
            {
                VO64GraphicsCollection collection = GraphicsCollections[i];
                GraphicsCollections.Remove(collection);
                collection.Dispose();
            }
        }

        public void RefreshGraphics()
        {
            //Repaint the control
            _glDisplay.Invalidate();
            _glDisplay.Update();
        }

        private void Camera_CameraUpdated(object sender, EventArgs e)
        {
            RefreshGraphics();
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

            GL.Enable(EnableCap.ColorArray);
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

        private void SelectAtMouse(MouseEventArgs e, bool addSelection)
        {
            int[] viewport = new int[4];
            Matrix4 modelMatrix, projMatrix;

            GL.GetFloat(GetPName.ModelviewMatrix, out modelMatrix);
            GL.GetFloat(GetPName.ProjectionMatrix, out projMatrix);
            GL.GetInteger(GetPName.Viewport, viewport);

            Vector3 start = UnProject(new Vector3(e.X, e.Y, 0.0f), projMatrix, modelMatrix, new Size(viewport[2], viewport[3]));
            Vector3 end = UnProject(new Vector3(e.X, e.Y, 1.0f), projMatrix, modelMatrix, new Size(viewport[2], viewport[3]));
            Vector3 dist = end - start;

            //Now we just need to be able to go through each triangle and figure out if it intersects!
            List<VO64GraphicsCollection> graphicsObjects = new List<VO64GraphicsCollection>();
            graphicsObjects.AddRange(this.GraphicsCollections);

            float t, minEl = float.MaxValue;
            VO64GraphicsElement selectedEl = null;

            while (graphicsObjects.Count > 0)
            {
                VO64GraphicsCollection coll = graphicsObjects.Last();
                graphicsObjects.RemoveAt(graphicsObjects.Count - 1);

                graphicsObjects.AddRange(coll.Collections);

                foreach (VO64GraphicsElement el in coll.Elements)
                {
                    if (el.Enabled)
                    {
                        foreach (IVO64Triangle tri in el.Triangles)
                        {
                            if(rayIntersectsTriangle(start, dist, 
                                new Vector3(el.Vertices[tri.T1].X, el.Vertices[tri.T1].Y, el.Vertices[tri.T1].Z),
                                new Vector3(el.Vertices[tri.T2].X, el.Vertices[tri.T2].Y, el.Vertices[tri.T2].Z),
                                new Vector3(el.Vertices[tri.T3].X, el.Vertices[tri.T3].Y, el.Vertices[tri.T3].Z),
                                out t))
                            {
                                if(t < minEl)
                                {
                                    minEl = t;
                                    selectedEl = el;
                                }
                                break;
                            }
                        }
                    }
                }
            }

            if (selectedEl != null)
            {
                if(!addSelection)
                    this.ClearSelectedElements();
                this._selectedElements.Add(selectedEl);

                //Handle the select box
                ClearSelectBox();

                float minX = float.MaxValue, maxX = float.MinValue, minZ = float.MaxValue, maxZ = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;

                foreach (VO64GraphicsElement el in _selectedElements)
                {
                    foreach (IVO64Vertex vtx in el.Vertices)
                    {
                        if (minX > vtx.X)
                            minX = vtx.X;
                        if (maxX < vtx.X)
                            maxX = vtx.X;
                        if (minY > vtx.Y)
                            minY = vtx.Y;
                        if (maxY < vtx.Y)
                            maxY = vtx.Y;
                        if (minZ > vtx.Z)
                            minZ = vtx.Z;
                        if (maxZ < vtx.Z)
                            maxZ = vtx.Z;
                    }
                }

                SetSelectBox(minX, minY, minZ, maxX, maxY, maxZ);
                //ReRender();
                RefreshGraphics();
            }
        }

        public static Vector3 UnProject(Vector3 mouse, Matrix4 projection, Matrix4 view, Size viewport)
        {
            Vector4 vec;

            vec.X = 2.0f * mouse.X / (float)viewport.Width - 1;
            vec.Y = -(2.0f * mouse.Y / (float)viewport.Height - 1);
            vec.Z = mouse.Z;
            vec.W = 1.0f;

            Matrix4 viewInv = Matrix4.Invert(view);
            Matrix4 projInv = Matrix4.Invert(projection);

            Vector4.Transform(ref vec, ref projInv, out vec);
            Vector4.Transform(ref vec, ref viewInv, out vec);

            if (vec.W > 0.000001f || vec.W < -0.000001f)
            {
                vec.X /= vec.W;
                vec.Y /= vec.W;
                vec.Z /= vec.W;
            }

            return vec.Xyz;
        }

        //http://www.lighthouse3d.com/tutorials/maths/ray-triangle-intersection/
        bool rayIntersectsTriangle(Vector3 p, Vector3 d,
			        Vector3 v0, Vector3 v1, Vector3 v2, out float t)
        {
	        Vector3 e1,e2,h,s,q = Vector3.Zero;
	        float a,f,u,v;
            e1 = v1 - v0;
            e2 = v2 - v0;
            t = 0;

            h = Vector3.Cross(d, e2);
            a = Vector3.Dot(e1, h);

	        if (a > -0.00001 && a < 0.00001)
		        return(false);

	        f = 1/a;
            s = p - v0;
            u = f * (Vector3.Dot(s, h));

	        if (u < 0.0 || u > 1.0)
		        return(false);

            q = Vector3.Cross(s, e1);
            v = f * Vector3.Dot(d, q);

	        if (v < 0.0 || u + v > 1.0)
		        return(false);

	        // at this stage we can compute t to find out where
	        // the intersection point is on the line
            t = f * Vector3.Dot(e2, q);

            if (t > 0.00001) // ray intersection
            {
                //t is distance from p along d!!
                return (true);
            }

            else // this means that there is a line intersection
            // but not a ray intersection
            {
                t = 0;
                return (false);
            }

        }

        public void ClearSelectedElements()
        {
            while(_selectedElements.Count > 0)
            {
                _selectedElements[0].Selected = false;
                _selectedElements.RemoveAt(0);
            }
        }

        public void SelectElement(VO64GraphicsElement element)
        {
            //Check if element exists?
            if (!_selectedElements.Contains(element))
            {
                _selectedElements.Add(element);
                element.Selected = true;
            }
        }

        public void UnselectElement(VO64GraphicsElement element)
        {
            //Check if element exists?
            if (_selectedElements.Contains(element))
            {
                _selectedElements.Remove(element);
                element.Selected = false;
            }
        }

        public void SetSelectBox(float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
        {
            _minSelectBoxX = minX;
            _minSelectBoxY = minY;
            _minSelectBoxZ = minZ;
            _maxSelectBoxX = maxX;
            _maxSelectBoxY = maxY;
            _maxSelectBoxZ = maxZ;
        }

        public void ClearSelectBox()
        {
            _minSelectBoxX = 0;
            _minSelectBoxY = 0;
            _minSelectBoxZ = 0;
            _maxSelectBoxX = 0;
            _maxSelectBoxY = 0;
            _maxSelectBoxZ = 0;
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
            switch (MouseMode)
            {
                case MouseFunction.Camera:
                    Camera.OnMouseDown(e);
                    break;
                case MouseFunction.Select:
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        OpenTK.Input.KeyboardState keyboard = OpenTK.Input.Keyboard.GetState();
                        SelectAtMouse(e, keyboard.IsKeyDown(OpenTK.Input.Key.ShiftLeft) || keyboard.IsKeyDown(OpenTK.Input.Key.ShiftRight));
                    }
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
            GL.Enable(EnableCap.ColorArray);

	        GL.Disable(EnableCap.CullFace);
	        //GL.CullFace(CullFaceMode.FrontAndBack);

            GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.AmbientAndDiffuse);
            GL.Enable(EnableCap.ColorMaterial);

	        //GL.Enable(EnableCap.Blend);
	        //GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.AlphaFunc(AlphaFunction.Gequal, 0.5f);
            GL.Enable(EnableCap.AlphaTest);

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

            //Selection box
            if (CanDisplaySelectBox())
            {
                DrawSelectBox();
            }

            // Restore the state
            GL.PopMatrix();
        }

        private void DrawSelectBox()
        {
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.Begin(BeginMode.Quads);

            GL.Color3((byte)255, (byte)255, (byte)0);
            GL.Vertex3(_minSelectBoxX, _minSelectBoxY, _minSelectBoxZ);
            GL.Vertex3(_maxSelectBoxX, _minSelectBoxY, _minSelectBoxZ);
            GL.Vertex3(_maxSelectBoxX, _maxSelectBoxY, _minSelectBoxZ);
            GL.Vertex3(_minSelectBoxX, _maxSelectBoxY, _minSelectBoxZ);

            GL.Vertex3(_minSelectBoxX, _minSelectBoxY, _maxSelectBoxZ);
            GL.Vertex3(_maxSelectBoxX, _minSelectBoxY, _maxSelectBoxZ);
            GL.Vertex3(_maxSelectBoxX, _maxSelectBoxY, _maxSelectBoxZ);
            GL.Vertex3(_minSelectBoxX, _maxSelectBoxY, _maxSelectBoxZ);

            GL.Vertex3(_minSelectBoxX, _minSelectBoxY, _minSelectBoxZ);
            GL.Vertex3(_minSelectBoxX, _maxSelectBoxY, _minSelectBoxZ);
            GL.Vertex3(_minSelectBoxX, _maxSelectBoxY, _maxSelectBoxZ);
            GL.Vertex3(_minSelectBoxX, _minSelectBoxY, _maxSelectBoxZ);

            GL.Vertex3(_maxSelectBoxX, _minSelectBoxY, _minSelectBoxZ);
            GL.Vertex3(_maxSelectBoxX, _maxSelectBoxY, _minSelectBoxZ);
            GL.Vertex3(_maxSelectBoxX, _maxSelectBoxY, _maxSelectBoxZ);
            GL.Vertex3(_maxSelectBoxX, _minSelectBoxY, _maxSelectBoxZ);

            GL.Vertex3(_minSelectBoxX, _maxSelectBoxY, _minSelectBoxZ);
            GL.Vertex3(_minSelectBoxX, _maxSelectBoxY, _maxSelectBoxZ);
            GL.Vertex3(_maxSelectBoxX, _maxSelectBoxY, _maxSelectBoxZ);
            GL.Vertex3(_maxSelectBoxX, _maxSelectBoxY, _minSelectBoxZ);

            GL.Vertex3(_minSelectBoxX, _maxSelectBoxY, _minSelectBoxZ);
            GL.Vertex3(_minSelectBoxX, _maxSelectBoxY, _maxSelectBoxZ);
            GL.Vertex3(_maxSelectBoxX, _maxSelectBoxY, _maxSelectBoxZ);
            GL.Vertex3(_maxSelectBoxX, _maxSelectBoxY, _minSelectBoxZ);

            GL.End();
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }

        #endregion
    }
}
