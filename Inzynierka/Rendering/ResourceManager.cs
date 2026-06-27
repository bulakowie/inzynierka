


using System.Net;
using OpenTK.Graphics.ES20;
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
    public class ResourceManager
    {
        private static ResourceManager _instance = null;

        private IDictionary<string, Texture2D> _textureList = new Dictionary<string, Texture2D>();

        public static ResourceManager Instance
        {
            get{
            if(_instance == null) _instance = new ResourceManager();
            
            return _instance;
            
        }
        }
    

    public Texture2D LoadTexture (string textureName)
        {
            _textureList.TryGetValue(textureName, out var value);
            if (value is not null)
            {
                return value;
            }
            value = TextureFactory.Load(textureName);
            _textureList.Add(textureName, value);
            return value;
        }
    }
}