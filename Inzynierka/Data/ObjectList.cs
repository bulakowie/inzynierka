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
        public List<ElectricObject> returnLista()
        {
            return listaObiektow;
        }

        public void KillTheObject (int a)
        {

    }
        public  void renderVertices(RenderDataCollector dataCollector)
        {
            float screenHeight = screenHeightGlobal;
            float
             screenWidth = screenWidthGlobal;
            float x_coords;
            float y_coords;
           
            float textureId;
            for (int i = 0; i < listaObiektow.Count(); i++)
            {
                int j = i;
                float problem = listaObiektow[j].imageLink%100;

                textureId = (listaObiektow[j].imageLink - problem) / 100;
                float textureX, textureY;
                textureY = (problem - problem%10)/100;
                
                textureX = (problem%10)/10;
                x_coords = objectCoordsMap[j].Item1;
                y_coords = objectCoordsMap[j].Item2;
                x_coords -= (screenWidth / 2);
                y_coords -= (screenHeight / 2);
                float box_size = 0.1f;
                float proportion = screenWidthGlobal / screenHeightGlobal;

                x_coords /= (screenWidth / 2);
                y_coords /= (screenHeight / 2);
                
                if (isObjectNode(i))
                {
                    box_size = 0.1f;
                }

                dataCollector.addRenderData(textureId, textureX, textureY,textureX + 0.1f, textureY + 0.1f,
                x_coords, y_coords,x_coords + box_size / proportion, y_coords + box_size );
                
            }

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

        public void changeValueInObject(int objectId, int whichValue, double value)
        {
            listaObiektow[objectId].objectValues_list[whichValue] = new Tuple<objectValues,double>(listaObiektow[objectId].objectValues_list[whichValue].Item1,value);
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
                if (item.Value.Item1 < mouseX && item.Value.Item1 > mouseX - 50 && item.Value.Item2  < mouseY && item.Value.Item2 > mouseY - 50)
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
                case 4:
                ACSource d= new ACSource(x,y);
                listaObiektow.Add(d);
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
             if (x< item.Item1 +25)
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
        public string GetObjectInfo (int id)
        {
            string returnString = "";
            ElectricObject a = listaObiektow[id];
            returnString += a.objectName;

            return returnString;
        }

        public bool  isObjectNode (int id)
        {
            if (listaObiektow[id].imageLink== 23) return true;
            else return false;
        }
    }
}
