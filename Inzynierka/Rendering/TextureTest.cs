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
namespace Render
{
    
    public class TextureTest
    {
        public TextureTest(string windowTitle, int windowWidth, int windowHeight, ObjectList objects, ConnectionList connections, AnimationList animations, UIList uis)
        {
            this.windowTitle = windowTitle;
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
            this.objects = objects;
            this.connections = connections;
            this.animations = animations;
            this.uis = uis;
        }

       protected string windowTitle {get;set;}
        protected int windowWidth {get;set;}
        protected  int windowHeight {get;set;}

        protected ObjectList objects;
        protected ConnectionList connections;

        protected AnimationList animations;
        protected UIList uis;


        private GameWindowSettings defaultUstawienia = GameWindowSettings.Default;
        private NativeWindowSettings natynweUstawienia = NativeWindowSettings.Default;


        private float[] _vertices;
        private float[] _vertices1 ;
        private float[] _vertices2; 
        private float[] _vertices3;

        private float[] _vertices4;
        private uint[] _indices;

        private Texture2D _texture;
        private readonly IDictionary<string,int> _uniforms = new Dictionary<string,int>();

        private int _shaderHandle;
        private int _vertexBufferObject;

        private int _elementBufferObject;
        private bool left_or_right_start;
        private bool left_or_right_stop;
        private int _vertexArrayObject;


        public void calculateIndices (int objects_number)
        {
            
        }
        public void uruchom ()
        {
            _vertices = objects.renderVertices().Concat(connections.renderVertices(objects,mode)).ToArray();
            _indices = objects.renderIndices(_vertices.Count());
            natynweUstawienia.Size = new Vector2i(windowWidth, windowHeight);
            natynweUstawienia.Title = windowTitle;
            GameTime gametime = new GameTime();
            using GameWindow gameWindow = new GameWindow(defaultUstawienia, natynweUstawienia );
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
            uniform sampler2D u_Texture[24];
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
            
            GL.GetProgram(_shaderHandle, GetProgramParameterName.ActiveUniforms, out var totalUniforms);
            for (int i=0; i< totalUniforms; i++)
            {
                string key = GL.GetActiveUniform(_shaderHandle, i ,out _, out _);
                int location = GL.GetUniformLocation(_shaderHandle, key);
                _uniforms.Add(key,location);
            }



            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float),  _vertices, BufferUsageHint.StaticDraw);


            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.VertexAttribPointer(0,3, VertexAttribPointerType.Float, false, sizeof(float) * 6,0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, 6 * sizeof(float), 5 * sizeof(float));
            GL.EnableVertexAttribArray(2);

            _elementBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            int[] samplers = new int[24] {23,22,21,20,19,18,17,16,15,14,13,12,11,10,9,8,7,6,5,4,3,2,1,0};
            var textureSampleUniformLocation = GetUniformLocation("u_Texture[0]");
            GL.UseProgram(_shaderHandle);
            GL.Uniform1(textureSampleUniformLocation, 24, samplers);


        //23
        ResourceManager.Instance.LoadTexture("C:/Users/Koza/Documents/inzynierka/Inzynierka/sprites/node.png");
        //22 - 19
        ResourceManager.Instance.LoadTexture("C:/Users/Koza/Documents/inzynierka/Inzynierka/sprites/resistor_button/resistor_button4.png");
        ResourceManager.Instance.LoadTexture("C:/Users/Koza/Documents/inzynierka/Inzynierka/sprites/resistor_button/resistor_button3.png");
        ResourceManager.Instance.LoadTexture("C:/Users/Koza/Documents/inzynierka/Inzynierka/sprites/resistor_button/resistor_button2.png");
        ResourceManager.Instance.LoadTexture("C:/Users/Koza/Documents/inzynierka/Inzynierka/sprites/resistor_button/resistor_button1.png");

        //18 - 10
        ResourceManager.Instance.LoadTexture("C:/Users/Koza/Documents/inzynierka/Inzynierka/sprites/cell_button/cell_button9.png");
        ResourceManager.Instance.LoadTexture("C:/Users/Koza/Documents/inzynierka/Inzynierka/sprites/cell_button/cell_button8.png");
        ResourceManager.Instance.LoadTexture("C:/Users/Koza/Documents/inzynierka/Inzynierka/sprites/cell_button/cell_button7.png");
        ResourceManager.Instance.LoadTexture("C:/Users/Koza/Documents/inzynierka/Inzynierka/sprites/cell_button/cell_button6.png");
        ResourceManager.Instance.LoadTexture("C:/Users/Koza/Documents/inzynierka/Inzynierka/sprites/cell_button/cell_button5.png");
        ResourceManager.Instance.LoadTexture("C:/Users/Koza/Documents/inzynierka/Inzynierka/sprites/cell_button/cell_button4.png");
        ResourceManager.Instance.LoadTexture("C:/Users/Koza/Documents/inzynierka/Inzynierka/sprites/cell_button/cell_button3.png");
        ResourceManager.Instance.LoadTexture("C:/Users/Koza/Documents/inzynierka/Inzynierka/sprites/cell_button/cell_button2.png");
        ResourceManager.Instance.LoadTexture("C:/Users/Koza/Documents/inzynierka/Inzynierka/sprites/cell_button/cell_button1.png");


        ResourceManager.Instance.LoadTexture("C:/Users/Koza/Documents/inzynierka/Inzynierka/sprites/insert_animation/insert_animation6.png");
        ResourceManager.Instance.LoadTexture("C:/Users/Koza/Documents/inzynierka/Inzynierka/sprites/insert_animation/insert_animation5.png");
        ResourceManager.Instance.LoadTexture("C:/Users/Koza/Documents/inzynierka/Inzynierka/sprites/insert_animation/insert_animation4.png");
        ResourceManager.Instance.LoadTexture("C:/Users/Koza/Documents/inzynierka/Inzynierka/sprites/insert_animation/insert_animation3.png");
        ResourceManager.Instance.LoadTexture("C:/Users/Koza/Documents/inzynierka/Inzynierka/sprites/insert_animation/insert_animation2.png");
        ResourceManager.Instance.LoadTexture("C:/Users/Koza/Documents/inzynierka/Inzynierka/sprites/insert_animation/insert_animation1.png");

        ResourceManager.Instance.LoadTexture("C:/Users/Koza/Documents/inzynierka/Inzynierka/sprites/wire.png");
        ResourceManager.Instance.LoadTexture("C:/Users/Koza/Documents/inzynierka/Inzynierka/sprites/Cell.png");
        ResourceManager.Instance.LoadTexture("C:/Users/Koza/Documents/inzynierka/Inzynierka/sprites/Resistor.png");
        ResourceManager.Instance.LoadTexture("C:/Users/Koza/Documents/inzynierka/Inzynierka/sprites/blank.png");


            GL.Enable(EnableCap.Blend);
GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }
        int ret = -1;
        int mode; //0 is dragging object, 1 is dragging wire
        
        public void update (GameTime gametime, GameWindow gameWindow)
        {
            var mouse = gameWindow.MouseState;
             _vertices1 = objects.renderVertices();
             _vertices2 = connections.renderVertices(objects,mode);
             _vertices3 = animations.renderVertices(objects);
             _vertices4 = uis.renderVertices();

             _vertices = new float[_vertices1.Length +_vertices2.Length + _vertices3.Length + _vertices4.Length];
            Array.Copy(_vertices2, 0 ,_vertices, 0,  _vertices2.Length);
            Array.Copy(_vertices1, 0 ,_vertices, _vertices2.Length,  _vertices1.Length);
            Array.Copy(_vertices3, 0 ,_vertices, _vertices1.Length+ _vertices2.Length,  _vertices3.Length);
            Array.Copy(_vertices4, 0 ,_vertices, _vertices1.Length+ _vertices2.Length + _vertices3.Length,  _vertices4.Length);


             _indices = objects.renderIndices(_vertices.Count());


            if(uis.isAbove(mouse.X, mouse.Y) >= 0)
            {
                if (mouse.IsButtonPressed(MouseButton.Left))
                {
                    objects.addObject(uis.isAbove(mouse.X, mouse.Y),500,500);
                }
            }
            //Jesli trzyma coś cały czas to:
            if (mouse.IsButtonDown(MouseButton.Left) && ret >=0)
            {
                objects.mouseDragObject(ret, mouse.X, mouse.Y);
                mode = 0;
            }
            else if (mouse.IsButtonDown(MouseButton.Right) && ret >=0)
            {
                connections.mouseDragWire(ret, mouse.X, mouse.Y, left_or_right_start, left_or_right_stop);
                mode = 1;
            }
           //pierwsze nacisniecie:
            else if (mouse.IsButtonPressed(MouseButton.Left))
            {
                ret = objects.gotObjectPressed(mouse.X, mouse.Y);
            }
            else if (mouse.IsButtonDown(MouseButton.Right) )
            {
                ret = objects.gotObjectPressed(mouse.X, mouse.Y);
                if (ret>=0) 
                {
                    left_or_right_start = objects.isLeft( mouse.X, mouse.Y,ret);
                    animations.addInsertAnimation(ret,left_or_right_start);
                }

            } 
            //jesli nie trzyma nic to:
            else 
            {
                animations.killInsertAnimation(ret);
                ret = -1;
                if (mode == 1)
                {
                    left_or_right_stop = objects.isLeft(mouse.X, mouse.Y, objects.gotObjectPressed(mouse.X, mouse.Y));
                    connections.mouseDragWire(objects.gotObjectPressed(mouse.X, mouse.Y), mouse.X, mouse.Y, left_or_right_start, left_or_right_stop);
                }
                mode = 0;
                return;
            }

             
            
        }
        public void render(GameTime gametime)
        {
             GL.ClearColor(Color4.Chocolate);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.UseProgram(_shaderHandle);
            GL.BindVertexArray(_vertexArrayObject);
           // GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);

            GL.BufferData(BufferTarget.ArrayBuffer,_vertices.Length * sizeof(float),_vertices, BufferUsageHint.DynamicDraw);          
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);

            GL.BufferData(BufferTarget.ElementArrayBuffer,_indices.Length * sizeof(uint), _indices,BufferUsageHint.DynamicDraw);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public int GetUniformLocation(string uniformName) => _uniforms[uniformName];
    }
    
}