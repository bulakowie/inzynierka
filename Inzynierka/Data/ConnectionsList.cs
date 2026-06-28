using System.Data;
using OpenTK.Graphics.OpenGL;
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
        public float[] renderVertices(ObjectList obj, int mode)
        {
            float[] vertices = new float[(listaPrzewodow.Count()) * 24];
            float screenHeight = screenHeightGlobal;
            float screenWidth = screenWidthGlobal;
            float box_size = 0.1f;
            float proportion = screenWidthGlobal / screenHeightGlobal;
            Tuple<float, float> coords1;
            Tuple<float, float> coords2;
            float textureId = 3;

            for (int i = 0; i < listaPrzewodow.Count(); i++)
            {
                coords1 = obj.returnObjectCoords(listaPrzewodow[i].secondObject);
                coords2 = obj.returnObjectCoords(listaPrzewodow[i].firstObject);

                coords1 = new Tuple<float, float>(coords1.Item1 - (screenWidth / 2), coords1.Item2 - (screenHeight / 2));
                coords2 = new Tuple<float, float>(coords2.Item1 - (screenWidth / 2), coords2.Item2 - (screenHeight / 2));

                coords1 = new Tuple<float, float>(coords1.Item1 / (screenWidth / 2), coords1.Item2 / (screenHeight / 2));
                coords2 = new Tuple<float, float>(coords2.Item1 / (screenWidth / 2), coords2.Item2 / (screenHeight / 2) + box_size / proportion);


                if (!listaPrzewodow[i].isSecondObjectLeft) coords1 = new Tuple <float,float>(coords1.Item1 + box_size, coords1.Item2 + box_size);

                if (!listaPrzewodow[i].isFirstObjectLeft) coords2  = new Tuple <float,float>(coords2.Item1 + box_size, coords2.Item2 + box_size);

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

                ObjectList objectList = ObjectList.getInstance();

                if (startID != -1 && objectId != -1)
                {
                    
                    if (objectList.isObjectNode(startID) ||
                    objectList.isObjectNode(objectId))
                    {
                        Console.WriteLine("Dodanie do node, koniec funkcji +" + objectId + " + " + startID);
                        listaPrzewodow.Add(new Wire(objectId, startID, _isFirstLeft, _isSecondLeft));
                        return;
                    }
                    bool guard = true;

                    int problemCount1 = 0, problemCount2 = 0, problemCount3 = 0, problemCount4 = 0;
                    bool isConenctionExisting = false;
                    bool shouldLoopFuckindDie = false;
                    for (int i = 0; i <= listaPrzewodow.Count() ; i++)
                    {
                        if (i ==listaPrzewodow.Count() && !isConenctionExisting)
                        {
                            Console.WriteLine("Dodanie nowego połączenia od zera" + objectId + "oraz" + startID);
                            listaPrzewodow.Add(new Wire(objectId, startID, _isSecondLeft, _isFirstLeft ));
                            shouldLoopFuckindDie = true;
                        } 
                        if ((listaPrzewodow[i].firstObject == objectId && listaPrzewodow[i].secondObject == startID) || (listaPrzewodow[i].firstObject == startID && listaPrzewodow[i].secondObject == objectId))
                        {
                           isConenctionExisting = true;
                        }
                        
                    

                        if (listaPrzewodow[i].firstObject == objectId && listaPrzewodow[i].isFirstObjectLeft) problemCount1++;
                        else if (listaPrzewodow[i].firstObject == objectId && !listaPrzewodow[i].isFirstObjectLeft) problemCount3++;

                        if (listaPrzewodow[i].firstObject == startID && listaPrzewodow[i].isFirstObjectLeft) problemCount2++;
                        else if (listaPrzewodow[i].firstObject == startID && !listaPrzewodow[i].isFirstObjectLeft) problemCount4++;

                        if (listaPrzewodow[i].secondObject == objectId && listaPrzewodow[i].isSecondObjectLeft) problemCount1++;
                        else  if (listaPrzewodow[i].secondObject == objectId && !listaPrzewodow[i].isSecondObjectLeft) problemCount3++;

                        if (listaPrzewodow[i].secondObject == startID && listaPrzewodow[i].isSecondObjectLeft) problemCount2++;
                         if (listaPrzewodow[i].secondObject == startID && !listaPrzewodow[i].isSecondObjectLeft) problemCount4++;
                                            if (shouldLoopFuckindDie) break;
}

                    Console.WriteLine(problemCount1 + " " + problemCount2 + " " + problemCount3 + " " + problemCount4);
                    if (problemCount1 > 1 || problemCount2 > 1 || problemCount3 > 1 || problemCount4 > 1)
                    {

                        Console.WriteLine("KURTWAAAA");
                        objectList.addObject(3, mouseX, mouseY);
                        for (int i = 0; i < listaPrzewodow.Count(); i++)
                        {

                            if (listaPrzewodow[i].firstObject == objectId)
                            {
                                int g = objectId; int d; bool issecondLeft; bool isFirstLeft = listaPrzewodow[i].isFirstObjectLeft;;
                                    d = listaPrzewodow[i].secondObject;
                                    issecondLeft = listaPrzewodow[i].isSecondObjectLeft;

                                    Console.WriteLine("Usuniecie polaczenia:" + d + "a elementem " + objectId);

                                listaPrzewodow.Remove(listaPrzewodow[i]);
                                if (guard)
                                {
                                    guard = false;
                                }
                                Console.WriteLine("Dodanie pomiedzy NODE" + (objectList.getLenght() - 1) + "a elementem " + d);
                                listaPrzewodow.Add(new Wire(objectList.getLenght() - 1, d, !issecondLeft, issecondLeft));
                                i--;

                            }

                            else if (listaPrzewodow[i].secondObject == objectId)
                            {
                                int g = objectId; int d; bool issecondLeft; bool isFirstLeft = listaPrzewodow[i].isSecondObjectLeft;

                                    d = listaPrzewodow[i].firstObject;
                                    issecondLeft = listaPrzewodow[i].isFirstObjectLeft;

                                    Console.WriteLine("Usuniecie polaczenia:" + d + "a elementem " + objectId);

                                listaPrzewodow.Remove(listaPrzewodow[i]);
                                if (guard)
                                {
                                    guard = false;
                                }
                                Console.WriteLine("Dodanie pomiedzy NODE" + (objectList.getLenght() - 1) + "a elementem " + d);
                                listaPrzewodow.Add(new Wire(objectList.getLenght() - 1, d, !issecondLeft, issecondLeft));
                                 i--;
                            }
                            else if (listaPrzewodow[i].firstObject == startID)
                            {
                                int g = objectId; int d; bool issecondLeft; bool isFirstLeft = listaPrzewodow[i].isFirstObjectLeft;;
                                    d = listaPrzewodow[i].secondObject;
                                    issecondLeft = listaPrzewodow[i].isSecondObjectLeft;

                                    Console.WriteLine("Usuniecie polaczenia:" + d + "a elementem " + startID);

                                listaPrzewodow.Remove(listaPrzewodow[i]);
                                if (guard)
                                {
                                    guard = false;
                                }
                                Console.WriteLine("Dodanie pomiedzy NODE" + (objectList.getLenght() - 1) + "a elementem " + d);
                                listaPrzewodow.Add(new Wire(objectList.getLenght() - 1, d, !issecondLeft, issecondLeft));
                                 i--;

                            }
                            else if (listaPrzewodow[i].secondObject == startID)
                            {
                                int g = objectId; int d; bool issecondLeft; bool isFirstLeft = listaPrzewodow[i].isSecondObjectLeft;

                                    d = listaPrzewodow[i].firstObject;
                                    issecondLeft = listaPrzewodow[i].isFirstObjectLeft;

                                    Console.WriteLine("Usuniecie polaczenia:" + d + "a elementem " + startID);

                                listaPrzewodow.Remove(listaPrzewodow[i]);
                                if (guard)
                                {
                                    guard = false;
                                }
                                Console.WriteLine("Dodanie pomiedzy NODE" + (objectList.getLenght() - 1) + "a elementem " + d);
                                listaPrzewodow.Add(new Wire(objectList.getLenght() - 1, d, !issecondLeft, issecondLeft));
                                 i--;
                            }
                        }
                       Console.WriteLine("Dodanie poza forem:" + (objectList.getLenght() - 1) + "a elementem " + objectId);
                        listaPrzewodow.Add(new Wire(objectId, objectList.getLenght() - 1,_isSecondLeft, _isFirstLeft ));
                        Console.WriteLine("Dodanie poza forem:" + (objectList.getLenght() - 1) + "a elementem " + startID);
                        listaPrzewodow.Add(new Wire(objectList.getLenght() - 1, startID, _isFirstLeft, _isSecondLeft));
                        //usun wszystkie przewody ktore juz istnieja i wrzuc je jeszcze do tego node.
                    }
                    else 
                    {
                        //Console.WriteLine("Dodanie poza wszystkim:" + startID + "a elementem " + objectId);
                        //listaPrzewodow.Add(new Wire(objectId, startID, _isSecondLeft, _isFirstLeft ));
                    }
                    // Console.Write("Pierwszy obiekt strona:" + _isFirstLeft );
                    //  Console.Write("Drugi obiekt strona:" + _isSecondLeft );

                    //  Console.WriteLine("Dodano nowy przewod, jest ich teraz:" + listaPrzewodow.Count()) ;
                }
                Console.WriteLine("STop" + listaPrzewodow.Count());
                startID = -1;

            }
        }
    }

}
