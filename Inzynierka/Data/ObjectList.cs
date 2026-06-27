using System.Security.Cryptography.X509Certificates;
using System.Text.Encodings.Web;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using static Program;

namespace ElectricData
{
    public class ObjectList
    {
        
        List<ElectricObject> listaObiektow = new List<ElectricObject>();
        public ObjectList instance;
        public ObjectList getInstance ()
        {
            if (instance == null)
            {
                instance = new ObjectList();
                return instance;
            }
            else return instance;
        }


        public float[] renderVertices()
        {
            float[] vertices = new float[(listaObiektow.Count() + 1) * 24];
            float screenHeight = screenHeightGlobal;
            float
             screenWidth = screenWidthGlobal;
            float proportion = screenWidthGlobal / screenHeightGlobal;
            float x_coords;
            float y_coords;
            float box_size = 0.2f;
            float textureId;

            float top = 1.0f;
            float bottom = 0.8f;
            float left = -1.0f;
            float right = 1.0f;


            vertices[0] = right;
            vertices[1] = top;
            vertices[2] = 0;
            vertices[3] = 1;
            vertices[4] = 1;
            vertices[5] = 0;

            vertices[6] = right;
            vertices[7] = bottom;
            vertices[8] = 0;
            vertices[9] = 1;
            vertices[10] = 0;
            vertices[11] = 0;

            vertices[12] = left;
            vertices[13] = bottom;
            vertices[14] = 0;
            vertices[15] = 0;
            vertices[16] = 0;
            vertices[17] = 0;

            vertices[18] = left;
            vertices[19] = top;
            vertices[20] = 0;
            vertices[21] = 0;
            vertices[22] = 1;
            vertices[23] = 0;


            for (int i = 1; i < listaObiektow.Count() + 1; i++)
            {
                int j = i - 1;
                textureId = listaObiektow[j].imageLink;
                x_coords = listaObiektow[j].X;
                y_coords = listaObiektow[j].Y;
                x_coords -= (screenWidth / 2);
                y_coords -= (screenHeight / 2);

                x_coords /= (screenWidth / 2);
                y_coords /= (screenHeight / 2);

                vertices[24 * i] = x_coords + box_size / proportion;
                vertices[24 * i + 1] = y_coords + box_size;
                vertices[24 * i + 2] = 0;
                vertices[24 * i + 3] = 1;
                vertices[24 * i + 4] = 1;
                vertices[24 * i + 5] = textureId;

                vertices[24 * i + 6] = x_coords + box_size / proportion;
                vertices[24 * i + 7] = y_coords;
                vertices[24 * i + 8] = 0;
                vertices[24 * i + 9] = 1;
                vertices[24 * i + 10] = 0;
                vertices[24 * i + 11] = textureId;

                vertices[24 * i + 12] = x_coords;
                vertices[24 * i + 13] = y_coords;
                vertices[24 * i + 14] = 0;
                vertices[24 * i + 15] = 0;
                vertices[24 * i + 16] = 0;
                vertices[24 * i + 17] = textureId;

                vertices[24 * i + 18] = x_coords;
                vertices[24 * i + 19] = y_coords + box_size;
                vertices[24 * i + 20] = 0;
                vertices[24 * i + 21] = 0;
                vertices[24 * i + 22] = 1;
                vertices[24 * i + 23] = textureId;
            }
            return vertices;

        }

        public uint[] renderIndices (int size)
        {
            uint [] indices = new uint [size * 6];
            for (int i=0; i< listaObiektow.Count()+1; i++)
            {
                indices[i*6] = (uint) (int)i*4;
                indices[i*6+1] = (uint) (int)i*4+1;
                indices[i*6+2] = (uint) (int)i*4+3;
                indices[i*6+3] = (uint) (int)i*4+1;
                indices[i*6+4] = (uint) (int)i*4+2;
                indices[i*6+5] = (uint) (int)i*4+3;
            }
            return indices;
        }
        public void deleteObject ()
        {
            
        }

        public void changeValueInObject(int objectId)
        {
            listaObiektow[objectId].X += 0.1f;
        }

        public void mouseDragObject (int objectId, float mouseX, float mouseY)
        {
            float proportion = screenWidthGlobal / screenHeightGlobal;
            listaObiektow[objectId].X = Math.Clamp(mouseX, 0, screenWidthGlobal - 100 );
            listaObiektow[objectId].Y = Math.Clamp( screenHeightGlobal - mouseY,0,screenHeightGlobal - 100 );

        }
        public int gotObjectPressed (float mouseX, float mouseY)
        {
            mouseY = screenHeightGlobal - mouseY;
            for (int i=0; i< listaObiektow.Count(); i++)
            {
                if (listaObiektow[i].X < mouseX && listaObiektow[i].X > mouseX - 100 && listaObiektow[i].Y  < mouseY && listaObiektow[i].Y > mouseY - 100)
                {
                    return i;
                }
            }
            return -1;
        }

        public void addObject(int objectId, float x, float y)
        {
            switch (objectId)
            {
                case 0:
                Resistor a = new Resistor(x,y);
                listaObiektow.Add(a);
                a = null;
                break;
                case 1:
                Cell b = new Cell(x,y);
                listaObiektow.Add(b);
                b = null;
                break;
            }
        }
        
    }
}
