using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace Render
{
    public class Texture2D
    {
        public int handle {get; set; }

        public Texture2D (int handle)
        {
            this.handle = handle;
        }

        public void Use ()
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, handle);
            
        }
    }
}