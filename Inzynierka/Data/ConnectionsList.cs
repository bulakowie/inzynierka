using System.Data;
using OpenTK.Mathematics;
using static Program;

namespace ElectricData
{
    public class ConnectionList
    {
        public ConnectionList()
        {
            isWireActive = 0;
            startID = -1;
            listaPrzewodow = new List<Wire>();
        }
        int isWireActive;
        int startID;
        List<Wire> listaPrzewodow;
        public ConnectionList instance;
        public ConnectionList getInstance()
        {
            if (instance == null)
            {
                instance = new ConnectionList();
                return instance;
            }
            else return instance;
        }

        //renderowanie kabli, musimy sciagnac dane na temat polozenia elementow a nbastepnie rozciagnac ten kabel tak by wygladalo to dobrze //bog mi swiadkiem co mnie podkusilo by pisac w C#
        public float[] renderVertices(ObjectList obj, int mode )
        {
            float[] vertices = new float[(listaPrzewodow.Count()) * 24];
            float screenHeight = screenHeightGlobal;
            float screenWidth = screenWidthGlobal;
            float box_size = 0.1f;
            float proportion = screenWidthGlobal / screenHeightGlobal;
            Tuple <float,float> coords1;
            Tuple <float,float> coords2;
            float textureId = 3;

            for (int i = 0; i < listaPrzewodow.Count(); i++)
            {
                coords1= obj.returnObjectCoords(listaPrzewodow[i].secondObject);
                coords2= obj.returnObjectCoords(listaPrzewodow[i].firstObject);

                coords1 = new Tuple <float,float>(coords1.Item1 -(screenWidth / 2), coords1.Item2 - (screenHeight /2));
                coords2  = new Tuple <float,float>(coords2.Item1 -(screenWidth / 2), coords2.Item2 - (screenHeight /2));
               
                coords1 = new Tuple <float,float>(coords1.Item1 /(screenWidth / 2), coords1.Item2 / (screenHeight /2));
                coords2  = new Tuple <float,float>(coords2.Item1 /(screenWidth / 2), coords2.Item2 / (screenHeight /2) + box_size/proportion);


                if (listaPrzewodow[i].isFirstObjectLeft) coords1 = new Tuple <float,float>(coords1.Item1 - 0.5f * box_size, coords1.Item2 + box_size/(2*proportion));
                else coords1 = new Tuple <float,float>(coords1.Item1 +  1.5f*box_size/proportion , coords1.Item2 + box_size/(2*proportion) );

                if (listaPrzewodow[i].isSecondObjectLeft) coords2  = new Tuple <float,float>(coords2.Item1 - 0.5f * box_size, coords2.Item2 + box_size/(2*proportion));
                else coords2  = new Tuple <float,float>(coords2.Item1  + 1.5f*box_size/proportion, coords2.Item2  + box_size/(2*proportion));

                vertices[24 * i] = coords2.Item1;
                vertices[24 * i + 1] = coords2.Item2;        
                vertices[24 * i + 2] = 0;
                vertices[24 * i + 3] = 1;
                vertices[24 * i + 4] = 1;
                vertices[24 * i + 5] = textureId;

                vertices[24 * i + 6] = coords2.Item1;
                vertices[24 * i + 7] = coords1.Item2;
                vertices[24 * i + 8] = 0;                       
                vertices[24 * i + 9] = 1;
                vertices[24 * i + 10] = 0;
                vertices[24 * i + 11] = textureId;

                vertices[24 * i + 12] = coords1.Item1;
                vertices[24 * i + 13] = coords1.Item2;
                vertices[24 * i + 14] = 0;
                vertices[24 * i + 15] = 0;
                vertices[24 * i + 16] = 0;
                vertices[24 * i + 17] = textureId;

                vertices[24 * i + 18] = coords1.Item1;
                vertices[24 * i + 19] = coords2.Item2;
                vertices[24 * i + 20] = 0;
                vertices[24 * i + 21] = 0;
                vertices[24 * i + 22] = 1;
                vertices[24 * i + 23] = textureId;
            }
            if (mode == 1)
            {
                
            }
            return vertices;

        }

        public uint[] renderIndices()
        {
            uint[] indices = new uint[(listaPrzewodow.Count()) * 6];
            for (int i = 0; i < listaPrzewodow.Count() + 1; i++)
            {
                indices[i * 6] = (uint)(int)i * 4;
                indices[i * 6 + 1] = (uint)(int)i * 4 + 1;
                indices[i * 6 + 2] = (uint)(int)i * 4 + 3;
                indices[i * 6 + 3] = (uint)(int)i * 4 + 1;
                indices[i * 6 + 4] = (uint)(int)i * 4 + 2;
                indices[i * 6 + 5] = (uint)(int)i * 4 + 3;
            }
            return indices;
        }

        public void mouseDragWire(int objectId, float mouseX, float mouseY, bool _isFirstLeft, bool _isSecondLeft)
        {
            if (isWireActive == 0)
            {
                startID = objectId;
                isWireActive = 1;
                Console.WriteLine("Start" + objectId);

            }

            if (objectId != startID)
            {
                isWireActive = 0;

                if (startID!=-1 && objectId!=-1)
                {
                    for (int i=0; i< listaPrzewodow.Count(); i++)
                    {
                        if ((listaPrzewodow[i].firstObject == objectId && listaPrzewodow[i].secondObject == startID) || (listaPrzewodow[i].firstObject == startID && listaPrzewodow[i].secondObject == objectId))
                        {
                            Console.WriteLine("Polaczenie juz istnieje, blad");
                            startID = -1;
                            return;
                        }
                    }
                    listaPrzewodow.Add(new Wire(objectId, startID,_isFirstLeft,_isSecondLeft));
                    Console.Write("Pierwszy obiekt strona:" + _isFirstLeft );
                    Console.Write("Drugi obiekt strona:" + _isSecondLeft );
               
                    Console.WriteLine("Dodano nowy przewod, jest ich teraz:" + listaPrzewodow.Count()) ;
                }
                Console.WriteLine("STop" + objectId);
                startID = -1;

            }
        }
    }

}
