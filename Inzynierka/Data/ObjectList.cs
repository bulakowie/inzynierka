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
        Dictionary <int, Tuple<float,float>> objectCoordsMap = new Dictionary<int, Tuple<float, float>>();
        private static  ObjectList instance;
        public static ObjectList getInstance ()
        {
            if (instance == null)
            {
                instance = new ObjectList();
            }
            return instance;
        }


        public float[] renderVertices()
        {
            float[] vertices = new float[(listaObiektow.Count()) * 24];
            float screenHeight = screenHeightGlobal;
            float
             screenWidth = screenWidthGlobal;
            float proportion = screenWidthGlobal / screenHeightGlobal;
            float x_coords;
            float y_coords;
            float box_size = 0.2f;
            float textureId;
            for (int i = 0; i < listaObiektow.Count(); i++)
            {
                int j = i;
                textureId = listaObiektow[j].imageLink;
                x_coords = objectCoordsMap[j].Item1;
                y_coords = objectCoordsMap[j].Item2;
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
            uint [] indices = new uint [size/24 * 6];
            for (int i=0; i< size/24; i++)
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
            //listaObiektow[objectId].X += 0.1f;
        }

        public void mouseDragObject (int objectId, float mouseX, float mouseY)
        {
            float proportion = screenWidthGlobal / screenHeightGlobal;
            objectCoordsMap[objectId] = new Tuple<float,float>(Math.Clamp(mouseX, 0, screenWidthGlobal - 100 ),Math.Clamp( screenHeightGlobal - mouseY,0,screenHeightGlobal - 100 ));

        }
        public int gotObjectPressed (float mouseX, float mouseY)
        {
            mouseY = screenHeightGlobal - mouseY;
            foreach(var item in objectCoordsMap)
            {
                //NAPRAWIC: przy zmianie rozmiaru nie wychwytuje elementow
                if (item.Value.Item1 < mouseX && item.Value.Item1 > mouseX - 100 && item.Value.Item2  < mouseY && item.Value.Item2 > mouseY - 100)
                {
                    return item.Key;
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
                case 3:
                Node c = new Node(x,y);
                listaObiektow.Add(c);
                break;
                default: return;
            }
            objectCoordsMap[listaObiektow.Count()-1] = new Tuple<float, float>(listaObiektow[listaObiektow.Count()-1].X, listaObiektow[listaObiektow.Count()-1].Y);
        }
        public Tuple <float,float> returnObjectCoords(int a)
        {
            return objectCoordsMap[a];
        }
        public bool isLeft(float x, float y, int objectId)
        {
            if (objectId < 0) return false;
            var item = objectCoordsMap[objectId];
             y = screenHeightGlobal - y;
             if (x< item.Item1 +50)
                {
                   // Console.WriteLine("Jest po lewej stronie." + x + "A to koordynaty przedmiotu:" + item.Item1);
                    return true;
                }
              // Console.WriteLine("Jest po prawej stronie." + x + "A to koordynaty przedmiotu:" + item.Item1);
             return false;
        }
        public int getLenght()
        {
            for (int i=0; i<listaObiektow.Count(); i++)
            {
               // Console.WriteLine(listaObiektow[i].imageLink + " " + listaObiektow[i].objectID );
            }
            return listaObiektow.Count();
        }

        public bool  isObjectNode (int id)
        {
            if (listaObiektow[id].imageLink== 23) return true;
            else return false;
        }
    }
}
