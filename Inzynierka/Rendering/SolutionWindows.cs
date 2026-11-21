using System.Xml.Serialization;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System.Transactions;
using System.Runtime.InteropServices;
using ElectricData;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Calculations;
namespace Render
{

    public class SolutionWindow
    {
        string solution;
        private StringBuilder _inputText = new StringBuilder();
        private string g =""; 
        public SolutionWindow(string windowTitle, int windowWidth, int windowHeight, string s)
        {
            this.windowTitle = windowTitle;
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
            this.solution = s;
        }

        protected string windowTitle { get; set; }
        protected int windowWidth { get; set; }
        protected int windowHeight { get; set; }

            protected MainRenderer mainRenderer = new MainRenderer();

        protected TextManager textManager = new TextManager();
        protected UIList uis;


        private GameWindowSettings defaultUstawienia = GameWindowSettings.Default;
        private NativeWindowSettings natynweUstawienia = NativeWindowSettings.Default;


        private float[] _vertices;
        private float[] _vertices1;
        private float[] _vertices2;
        private float[] _vertices3;

        private float[] _vertices4;

        private float[] _vertices5;
        private uint[] _indices;

        private Texture2D _texture;
        private readonly IDictionary<string, int> _uniforms = new Dictionary<string, int>();

        private int _shaderHandle;
        private int _vertexBufferObject;

        private int _elementBufferObject;
        private bool left_or_right_start;
        private bool left_or_right_stop;
        private int _vertexArrayObject;


        public void uruchom()
        {
            
            RenderDataCollector renderDataCollector = new RenderDataCollector();
             _vertices = new float[3];

            _indices = new uint[3];
            natynweUstawienia.Size = new Vector2i(windowWidth, windowHeight);
            natynweUstawienia.Title = windowTitle;
            GameTime gametime = new GameTime();
            using GameWindow gameWindow = new GameWindow(defaultUstawienia, natynweUstawienia);
            gameWindow.TextInput += OnTextInput;
            gameWindow.Resize += (ResizeEventArgs e) =>
        {
            GL.Viewport(0, 0, e.Width, e.Height);

            Program.changeScreenSize(e.Width, e.Height);
        };

            gameWindow.Load += load;
            gameWindow.UpdateFrame += (FrameEventArgs eventArgs) =>
            {
                double time = eventArgs.Time;
                gametime.ElapsedGameTime = TimeSpan.FromMilliseconds(time);
                gametime.TotalGameTime += TimeSpan.FromMilliseconds(time);
                update(gametime, gameWindow);
            };
            gameWindow.RenderFrame += (FrameEventArgs eventArgs) =>
            {
                render(gametime);
                gameWindow.SwapBuffers();
            };
            return;
        }
        public void init()
        {

        }
        public void load()
        {
            string vertexShader = @"
            #version 330 core
            layout (location = 0) in vec3 aPosition;
            layout (location = 1) in vec2 aTexCoord;
            layout (location =2 ) in float vIndex;
            out vec2 texCoord;
            out float aIndex;
            void main() 
            {
             gl_Position = vec4(aPosition.xyz, 1.0);
                 texCoord = aTexCoord;
                 aIndex = vIndex;
            }";

            string fragmentShader = @"
            #version 330 core
            out vec4 color;
            in vec2 texCoord;
            in float aIndex;
            uniform sampler2D u_Texture[2];
            void main() 
            {
            int index = int(aIndex);
             color = texture(u_Texture[index], texCoord);
            }";


            int vertexShaderId = GL.CreateShader(ShaderType.VertexShaderArb);
            GL.ShaderSource(vertexShaderId, vertexShader);

            GL.CompileShader(vertexShaderId);


            GL.GetShader(vertexShaderId, ShaderParameter.CompileStatus, out var vertexShaderCompilation);

            if (vertexShaderCompilation != (int)All.True)
            {
                Console.WriteLine(GL.GetShaderInfoLog(vertexShaderId));
            }

            int fragmentShaderId = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(fragmentShaderId, fragmentShader);
            GL.CompileShader(fragmentShaderId);
            GL.GetShader(fragmentShaderId, ShaderParameter.CompileStatus, out var fragmentShaderCompilation);

            if (fragmentShaderCompilation != (int)All.True)
            {
                Console.WriteLine(GL.GetShaderInfoLog(fragmentShaderId));
            }


            _shaderHandle = GL.CreateProgram();
            GL.AttachShader(_shaderHandle, vertexShaderId);
            GL.AttachShader(_shaderHandle, fragmentShaderId);
            GL.LinkProgram(_shaderHandle);

            GL.DetachShader(_shaderHandle, vertexShaderId);
            GL.DetachShader(_shaderHandle, fragmentShaderId);

            GL.DeleteShader(vertexShaderId);
            GL.DeleteShader(fragmentShaderId);

            GL.GetProgram(_shaderHandle, GetProgramParameterName.ActiveUniforms, out var totalUniforms);
            for (int i = 0; i < totalUniforms; i++)
            {
                string key = GL.GetActiveUniform(_shaderHandle, i, out _, out _);
                int location = GL.GetUniformLocation(_shaderHandle, key);
                _uniforms.Add(key, location);
            }



            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);


            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 6, 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, 6 * sizeof(float), 5 * sizeof(float));
            GL.EnableVertexAttribArray(2);

            _elementBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            int[] samplers = new int[2] { 1, 0 };
            var textureSampleUniformLocation = GetUniformLocation("u_Texture[0]");
            GL.UseProgram(_shaderHandle);
            GL.Uniform1(textureSampleUniformLocation, 2, samplers);

            ResourceManager.Instance.LoadTexture("C:/Users/Koza/Documents/inzynierka/Inzynierka/sprites/font-sample.png");
            ResourceManager.Instance.LoadTexture("C:/Users/Koza/Documents/inzynierka/Inzynierka/sprites/spritesheet1.png");


            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);


        }
        int ret = -1;
            int ret2 = -1;

        int mode; //0 is dragging object, 1 is dragging wire

        public void update(GameTime gametime, GameWindow gameWindow)
        {
            double solution = 0;
            var mouse = gameWindow.MouseState;
            var keyboard = gameWindow.KeyboardState;
           
            RenderDataCollector renderDataCollector = new RenderDataCollector();
            
            textManager.writingObjectData(ret2);
            _inputText.Clear();
                       

             textManager.renderVertices(renderDataCollector);

            _vertices = mainRenderer.renderVertices(renderDataCollector);

            _indices = mainRenderer.renderIndices(renderDataCollector);
            


                return;
            

            
        }
        public void render(GameTime gametime)
        {
            GL.ClearColor(Color4.Chocolate);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.UseProgram(_shaderHandle);
            GL.BindVertexArray(_vertexArrayObject);
            // GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);

            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);

            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.DynamicDraw);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public int GetUniformLocation(string uniformName) => _uniforms[uniformName];

        void OnTextInput(TextInputEventArgs e)
        {
            _inputText.Append(e.AsString);
        }
    }

}