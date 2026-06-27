using System.Xml.Serialization;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using System.Transactions;
namespace Render
{
    
    public class MainWindow
    {
        public MainWindow(string windowTitle, int windowWidth, int windowHeight)
        {
            this.windowTitle = windowTitle;
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
        }

       protected string windowTitle {get;set;}
        protected int windowWidth {get;set;}
        protected  int windowHeight {get;set;}

        private GameWindowSettings defaultUstawienia = GameWindowSettings.Default;
        private NativeWindowSettings natynweUstawienia = NativeWindowSettings.Default;


        private readonly float[] triangleSides =
        {
            -0.5f, -0.5f, 0.0f,
            0.5f, -0.5f, 0.0f,
            0.0f, 0.5f, 0.0f
        };

        private int _shaderHandle;
        private int _vertexBufferObject;

        private int _vertexArrayObject;

        public void uruchom ()
        {
            natynweUstawienia.Size = new Vector2i(windowWidth, windowHeight);
            natynweUstawienia.Title = windowTitle;
            GameTime gametime = new GameTime();
            using GameWindow gameWindow = new GameWindow(defaultUstawienia, natynweUstawienia );
            gameWindow.Load += load;
            gameWindow.UpdateFrame += (FrameEventArgs eventArgs) =>
            {
                double time = eventArgs.Time;
                gametime.ElapsedGameTime = TimeSpan.FromMilliseconds(time);
                gametime.TotalGameTime += TimeSpan.FromMilliseconds(time);
                update(gametime);
            };
            gameWindow.RenderFrame += (FrameEventArgs eventArgs) =>
            {
                render(gametime);
                gameWindow.SwapBuffers();
            };
            gameWindow.Run();
        }
        public void init()
        {
            
        }
        public void load ()
        {
            string vertexShader = @"
            #version 330 core
            layout (location = 0) in vec3 aPosition;
            void main() 
            {
             gl_Position = vec4(aPosition.xyz, 1.0);
            }";

            string fragmentShader = @"
            #version 330 core
            out vec4 color;
            void main() 
            {
             color = vec4(1.0, 0.0, 0.0, 1.0);
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
            GL.GetShader(fragmentShaderId, ShaderParameter.CompileStatus,out var fragmentShaderCompilation);

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


            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, triangleSides.Length * sizeof(float), triangleSides, BufferUsageHint.StaticDraw);


            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.VertexAttribPointer(0,3, VertexAttribPointerType.Float, false, sizeof(float) * 3,0);
            GL.EnableVertexAttribArray(0);
        }
        public void update (GameTime gametime)
        {
            Console.WriteLine(gametime.TotalGameTime.TotalMilliseconds);
        }
        public void render(GameTime gametime)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(Color4.Chocolate);
            GL.UseProgram(_shaderHandle);
            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
        }
    }
}