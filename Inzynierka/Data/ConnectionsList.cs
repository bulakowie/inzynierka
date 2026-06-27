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

        public float[] renderVertices() //renderowanie kabli, musimy sciagnac dane na temat polozenia elementow a nbastepnie rozciagnac ten kabel tak by wygladalo to dobrze
        //bog mi swiadkiem co mnie podkusilo by pisac w C#
        {
            float[] vertices = new float[(listaPrzewodow.Count()) * 24];
            float screenHeight = screenHeightGlobal;
            float
             screenWidth = screenWidthGlobal;
            float proportion = screenWidthGlobal / screenHeightGlobal;
            float x_coords;
            float y_coords;
            float box_size = 0.2f;
            float textureId;


            for (int i = 1; i < listaPrzewodow.Count() + 1; i++)
            {
                int j = i - 1;
                textureId = 1;
                x_coords = 1;
                y_coords = 1;
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

        public void mouseDragWire(int objectId, float mouseX, float mouseY)
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

                if (startID!=-1)
                {
                    int firstObjectProblemCount =0, secondObjectProblemCount = 0;
                    for (int i=0; i< listaPrzewodow.Count(); i++)
                    {
                        if ((listaPrzewodow[i].firstObject == objectId && listaPrzewodow[i].secondObject == startID) || (listaPrzewodow[i].firstObject == startID && listaPrzewodow[i].secondObject == objectId))
                        {
                            Console.WriteLine("Polaczenie juz istnieje, blad");
                            startID = -1;
                            return;
                        }
                        if (listaPrzewodow[i].firstObject == objectId || listaPrzewodow[i].firstObject == startID) firstObjectProblemCount++;
                        if (listaPrzewodow[i].secondObject == objectId || listaPrzewodow[i].secondObject == startID) secondObjectProblemCount++;

                    }
                    if (firstObjectProblemCount > 1 || secondObjectProblemCount > 1)
                    {
                        Console.WriteLine("Jeden z elementow ma juz 2 polaczenia, blad");
                        startID = -1;
                        return;
                    }
                    listaPrzewodow.Add(new Wire(objectId, startID));
                }
                Console.WriteLine("STop" + objectId);
                startID = -1;

            }
        }
    }

}
