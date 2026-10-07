using System.Data;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using static Program;

namespace ElectricData
{

    public class ConnectionList
    {
        public static bool isTheWireCorrect = false;
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
        public void renderVertices(RenderDataCollector dataCollector)
        {
            ObjectList obj = ObjectList.getInstance();
            float screenHeight = screenHeightGlobal;
            float screenWidth = screenWidthGlobal;
            float box_size = 0.1f;
            float proportion = screenWidthGlobal / screenHeightGlobal;
            Tuple<float, float> coords1;
            Tuple<float, float> coords2;
            Tuple<float, float> coords3;
            Tuple<float, float> coords4;

            float textureId = 0;

            for (int i = 0; i < listaPrzewodow.Count(); i++)
            {
                //Musze odnalezc wszystkie algorytmy do generowania kabla.
                //1. klasyczny, jeśli ten po LEWEJ (czyli mniejszykoordynat x) jest skierowany w prawo kabel i ten po PRAWEJ jest w lewo
                //  Wtedy wyciagamy kabel z obu i pewnym punkcjie łączymy je pionową linią
                //2. Odwrotny, jeśli jeden lub dwa kable wymagają zawrócenia, czyli są skeirowane w przeciwną stronę niż ich partenr
                //wtedy trzeba kabel wyprowadzic, przeciaganc w gore lub w doł, zawrócić, znowu góre lub w dół i połączyć
                //
                
                
                int a = 2;
                bool c1_isLeft, c2_isLeft;
                float textureX = 0.8f;
                float textureY = 0.9f;
                if (obj.returnObjectCoords(listaPrzewodow[i].secondObject).Item1 < obj.returnObjectCoords(listaPrzewodow[i].firstObject).Item1)
                {
                   // Console.WriteLine("SWAP");
                    coords1 = obj.returnObjectCoords(listaPrzewodow[i].secondObject);
                    coords2 = obj.returnObjectCoords(listaPrzewodow[i].firstObject);
                    c1_isLeft =listaPrzewodow[i].isSecondObjectLeft;
                    c2_isLeft =listaPrzewodow[i].isFirstObjectLeft;

                    textureY = 1f;
                }
                else
                {
                    coords1 = obj.returnObjectCoords(listaPrzewodow[i].firstObject);
                    coords2 = obj.returnObjectCoords(listaPrzewodow[i].secondObject);
                    c1_isLeft =listaPrzewodow[i].isFirstObjectLeft;
                    c2_isLeft =listaPrzewodow[i].isSecondObjectLeft;
                    
                }


                coords1 = new Tuple<float, float>(coords1.Item1 - (screenWidth / 2), coords1.Item2 - (screenHeight / 2));
                coords2 = new Tuple<float, float>(coords2.Item1 - (screenWidth / 2), coords2.Item2 - (screenHeight / 2));

                coords1 = new Tuple<float, float>(coords1.Item1 / (screenWidth / 2), coords1.Item2 / (screenHeight / 2));
                coords2 = new Tuple<float, float>(coords2.Item1 / (screenWidth / 2), coords2.Item2 / (screenHeight / 2));

               if (!c1_isLeft && c2_isLeft) a = 1;
               else if (c1_isLeft && c2_isLeft )a = 2;
               else if (!c1_isLeft && !c2_isLeft )a = 3;
                else if (c1_isLeft && !c2_isLeft) a = 4;
               else a = 0;
                switch (a)
                {
                    case 1:
                    //jesli odleglosc miedzy nimi jest wieksza niz 1 box, ORAZ drugi przedmiot patrzy w naszą stronę
                dataCollector.addRenderData(1, textureX, textureY,textureX + 0.1f, textureY + 0.1f,
                coords1.Item1 + box_size / proportion, coords1.Item2,
                coords1.Item1 + 2* box_size / proportion, coords1.Item2 +2* box_size / proportion);

                if (coords1.Item2 < coords2.Item2)
                dataCollector.addRenderData(1, textureX, textureY,textureX + 0.1f, textureY + 0.1f,
                coords1.Item1 + box_size / proportion, coords1.Item2,
                coords1.Item1 + 2* box_size / proportion, coords2.Item2 +2* box_size / proportion);
                else
                dataCollector.addRenderData(1, textureX, textureY,textureX + 0.1f, textureY + 0.1f,
                coords1.Item1 + box_size / proportion, coords1.Item2,
                coords1.Item1 + 2* box_size / proportion, coords2.Item2 );


                dataCollector.addRenderData(1, textureX, textureY,textureX + 0.1f, textureY + 0.1f,
                coords1.Item1 + 2* box_size / proportion, coords2.Item2,
                coords2.Item1, coords2.Item2 +2* box_size / proportion);
                break;

                    case 2:
                    //jesli my jestesmy odwroceni od rugeigo obiektu plecami
                dataCollector.addRenderData(1, textureX, textureY,textureX + 0.1f, textureY + 0.1f,
                coords1.Item1 - box_size / proportion, coords1.Item2,
                coords1.Item1 , coords1.Item2 +2* box_size / proportion);

                if (coords1.Item2 < coords2.Item2)
                dataCollector.addRenderData(1, textureX, textureY,textureX + 0.1f, textureY + 0.1f,
                coords1.Item1 - box_size / proportion, coords1.Item2,
                coords1.Item1, coords2.Item2 +2* box_size / proportion);
                else
                dataCollector.addRenderData(1, textureX, textureY,textureX + 0.1f, textureY + 0.1f,
                coords1.Item1 - box_size / proportion, coords1.Item2,
                coords1.Item1 , coords2.Item2 );

                dataCollector.addRenderData(1, textureX, textureY,textureX + 0.1f, textureY + 0.1f,
                coords1.Item1, coords2.Item2,
                coords2.Item1, coords2.Item2 +2* box_size / proportion);
                break;

                case 3:
                 dataCollector.addRenderData(1, textureX, textureY,textureX + 0.1f, textureY + 0.1f,
                coords1.Item1 + box_size / proportion, coords1.Item2,
                coords2.Item1 + 2* box_size / proportion, coords1.Item2 +2* box_size / proportion);

                if (coords1.Item2 < coords2.Item2)
                dataCollector.addRenderData(1, textureX, textureY,textureX + 0.1f, textureY + 0.1f,
                coords2.Item1 + box_size / proportion, coords2.Item2,
                coords2.Item1 + 2* box_size / proportion, coords1.Item2);
                else
                dataCollector.addRenderData(1, textureX, textureY,textureX + 0.1f, textureY + 0.1f,
                coords2.Item1 + box_size / proportion, coords2.Item2,
                coords2.Item1 + 2* box_size / proportion, coords1.Item2 +2* box_size / proportion);
                 dataCollector.addRenderData(1, textureX, textureY,textureX + 0.1f, textureY + 0.1f,
                coords2.Item1 + box_size / proportion, coords2.Item2,
                coords2.Item1 + 2* box_size / proportion, coords2.Item2 +2* box_size / proportion);
                break;

                case 4:
                dataCollector.addRenderData(1, textureX, textureY,textureX + 0.1f, textureY + 0.1f,
                coords1.Item1 - box_size / proportion, coords1.Item2,
                coords1.Item1 , coords1.Item2 +2* box_size / proportion);

                if (coords1.Item2 < coords2.Item2)
                {
                    if (abs(coords1.Item2 - coords2.Item2) > box_size * 2)
                    {

                        float between = (coords1.Item2 + coords2.Item2) /2;
                dataCollector.addRenderData(1, textureX, textureY,textureX +  0.1f, textureY + 0.1f,
                coords1.Item1, between,
                coords2.Item1 + 2* box_size / proportion, between+2* box_size / proportion);
                dataCollector.addRenderData(1, textureX, textureY,textureX + 0.1f, textureY + 0.1f,
                coords2.Item1 + box_size / proportion, between,
                coords2.Item1 + 2* box_size / proportion, coords2.Item2 +2* box_size / proportion);      
                dataCollector.addRenderData(1, textureX, textureY,textureX + 0.1f, textureY + 0.1f,
                coords1.Item1 - box_size / proportion, coords1.Item2,
                coords1.Item1, between + 2*box_size / proportion); 
                }
                else
                {
                dataCollector.addRenderData(1, textureX, textureY,textureX + 0.1f, textureY + 0.1f,
                coords1.Item1 - box_size / proportion, coords1.Item2,
                coords1.Item1, coords2.Item2 +6* box_size / proportion);

                dataCollector.addRenderData(1, textureX, textureY,textureX + 0.1f, textureY + 0.1f,
                coords1.Item1 - box_size / proportion,  coords2.Item2 +4* box_size / proportion,
                coords2.Item1 + 2* box_size / proportion, coords2.Item2 +6* box_size / proportion); 

                dataCollector.addRenderData(1, textureX, textureY,textureX + 0.1f, textureY + 0.1f,
                coords2.Item1 + box_size / proportion, coords2.Item2,
                coords2.Item1 + 2* box_size / proportion, coords2.Item2 +6* box_size / proportion); 
                    }
                }
                else
                {
                    if (abs(coords1.Item2 - coords2.Item2) > box_size * 2)
                    {

                        float between = (coords1.Item2 + coords2.Item2) /2;
                dataCollector.addRenderData(1, textureX, textureY,textureX +  0.1f, textureY + 0.1f,
                coords1.Item1 - box_size / proportion, between,
                coords2.Item1 + 2* box_size / proportion, between+2* box_size / proportion);
                dataCollector.addRenderData(1, textureX, textureY,textureX + 0.1f, textureY + 0.1f,
                coords2.Item1 + box_size / proportion, between,
                coords2.Item1 + 2* box_size / proportion, coords2.Item2 +2* box_size / proportion);      
                dataCollector.addRenderData(1, textureX, textureY,textureX + 0.1f, textureY + 0.1f,
                coords1.Item1 - box_size / proportion, coords1.Item2,
                coords1.Item1, between + 2*box_size / proportion); 
                }
                else
                {
                dataCollector.addRenderData(1, textureX, textureY,textureX + 0.1f, textureY + 0.1f,
                coords1.Item1 - box_size / proportion, coords1.Item2,
                coords1.Item1, coords2.Item2 +6* box_size / proportion);

                dataCollector.addRenderData(1, textureX, textureY,textureX + 0.1f, textureY + 0.1f,
                coords1.Item1 - box_size / proportion,  coords2.Item2 +4* box_size / proportion,
                coords2.Item1 + 2* box_size / proportion, coords2.Item2 +6* box_size / proportion); 

                dataCollector.addRenderData(1, textureX, textureY,textureX + 0.1f, textureY + 0.1f,
                coords2.Item1 + box_size / proportion, coords2.Item2,
                coords2.Item1 + 2* box_size / proportion, coords2.Item2 +6* box_size / proportion); 
                    }  
                }
                
                
                dataCollector.addRenderData(1, textureX, textureY,textureX + 0.1f, textureY + 0.1f,
                coords2.Item1 + box_size / proportion, coords2.Item2,
                coords2.Item1 + 2* box_size / proportion, coords2.Item2 +2* box_size / proportion);
                break;
                case 0:
                break;
                }
              //  if (!listaPrzewodow[i].isSecondObjectLeft && !obj.isObjectNode(listaPrzewodow[i].secondObject)) coords1 = new Tuple<float, float>(coords1.Item1 + 1.5f * box_size, coords1.Item2 + box_size);

                //if (!listaPrzewodow[i].isFirstObjectLeft && !obj.isObjectNode(listaPrzewodow[i].firstObject)) coords2 = new Tuple<float, float>(coords2.Item1 + 1.5f * box_size, coords2.Item2 + box_size);

               
                
              // dataCollector.addRenderData(textureId, textureX, textureY,textureX + 0.1f, textureY + 0.1f,
               // coords3.Item1, coords3.Item2, coords4.Item1, coords4.Item2 );

               // dataCollector.addRenderData(textureId, textureX, textureY,textureX + 0.1f, textureY + 0.1f,
               // coords4.Item1, coords4.Item2, coords2.Item1, coords2.Item2 );
                }


        }

        private float abs(float v)
        {
            if (v<0) return v * -1;
            else return v;
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
                    List<int> problem1List = new List<int>();
                    List<int> problem2List = new List<int>();
                    List<int> problem3List = new List<int>();
                    List<int> problem4List = new List<int>();

                    bool isConenctionExisting = false;
                    bool shouldLoopFuckindDie = false;
                    for (int i = 0; i <= listaPrzewodow.Count(); i++)
                    {
                        if (i == listaPrzewodow.Count() && !isConenctionExisting)
                        {
                            Console.WriteLine("Dodanie nowego połączenia od zera" + objectId + "oraz" + startID);
                            listaPrzewodow.Add(new Wire(objectId, startID, _isSecondLeft, _isFirstLeft));

                            shouldLoopFuckindDie = true;
                        }
                        if ((listaPrzewodow[i].firstObject == objectId && listaPrzewodow[i].isFirstObjectLeft == _isSecondLeft
                        && listaPrzewodow[i].secondObject == startID && listaPrzewodow[i].isSecondObjectLeft == _isFirstLeft)
                        || (listaPrzewodow[i].firstObject == startID && listaPrzewodow[i].isFirstObjectLeft == _isFirstLeft
                        && listaPrzewodow[i].secondObject == objectId && listaPrzewodow[i].isSecondObjectLeft == _isSecondLeft))
                        {
                            isConenctionExisting = true;
                        }




                        if (shouldLoopFuckindDie) break;
                    }

                    Console.WriteLine(problemCount1 + " " + problemCount2 + " " + problemCount3 + " " + problemCount4);
                    //Sprawdzamy na podstawie ilosci polaczen ktory element jest elementem "głównym" czyli tym który ma do siebie podlaczone dwa przewody
                    if (problemCount1 > 1 || problemCount2 > 1 || problemCount3 > 1 || problemCount4 > 1)
                    {
                        int problemObjectId = -1, theOther = -1;
                        bool isProblemObjectSideLeft = false, isOtherObjectSideLeft = false;
                        List<Wire> nowePrzewody = new List<Wire>();

                        List<int> listZlychPrzewodow = new List<int>();
                        if (problemCount1 > 1)
                        {
                            listZlychPrzewodow = problem1List;
                            isProblemObjectSideLeft = true;
                            problemObjectId = objectId;
                            isOtherObjectSideLeft = _isFirstLeft;
                            theOther = startID;
                        }
                        if (problemCount2 > 1)
                        {
                            listZlychPrzewodow = problem2List;
                            isProblemObjectSideLeft = false;
                            problemObjectId = objectId;
                            isOtherObjectSideLeft = _isFirstLeft;
                            theOther = startID;


                        }
                        if (problemCount3 > 1)
                        {
                            listZlychPrzewodow = problem3List;
                            isProblemObjectSideLeft = true;
                            problemObjectId = startID;
                            isOtherObjectSideLeft = _isSecondLeft; ;

                            theOther = objectId;


                        }
                        if (problemCount4 > 1)
                        {
                            listZlychPrzewodow = problem4List;
                            isProblemObjectSideLeft = false;
                            problemObjectId = startID;
                            isOtherObjectSideLeft = _isSecondLeft; ;
                            theOther = objectId;

                        }
                        //sprawdz czy jest polaczony już z jakimś nodem : wtedy zamiast nowego node po prostu podlaczamy
                        //problematyczny element do istniejącego node
                        int a = czyPosiadaNode(problemObjectId, isProblemObjectSideLeft, objectList);
                        if (a != -1)
                        {
                            listaPrzewodow.Remove(listaPrzewodow[listaPrzewodow.Count - 1]);
                            listaPrzewodow.Add(new Wire(theOther, a, isOtherObjectSideLeft, false));
                            return;
                        }
                        objectList.addObject(3, mouseX, mouseY);
                        int nodeID = objectList.getLenght() - 1;
                        listaPrzewodow.Add(new Wire(problemObjectId, nodeID, isProblemObjectSideLeft, false));
                        for (int i = listZlychPrzewodow.Count() - 1; i >= 0; i--)
                        {
                            if (listaPrzewodow[listZlychPrzewodow[i]].firstObject == problemObjectId && listaPrzewodow[listZlychPrzewodow[i]].isFirstObjectLeft == isProblemObjectSideLeft)
                            {
                                int g = objectId; int d; bool issecondLeft; bool isFirstLeft = listaPrzewodow[listZlychPrzewodow[i]].isFirstObjectLeft; ;
                                d = listaPrzewodow[listZlychPrzewodow[i]].secondObject;
                                issecondLeft = listaPrzewodow[listZlychPrzewodow[i]].isSecondObjectLeft;

                                Console.WriteLine("Usuniecie polaczenia:" + d + "a elementem " + objectId);
                                if (guard)
                                {
                                    guard = false;
                                }
                                Console.WriteLine("Dodanie pomiedzy NODE" + (objectList.getLenght() - 1) + "a elementem " + d);
                                nowePrzewody.Add(new Wire(nodeID, d, !issecondLeft, issecondLeft));


                            }
                            else if (listaPrzewodow[listZlychPrzewodow[i]].secondObject == problemObjectId && listaPrzewodow[listZlychPrzewodow[i]].isSecondObjectLeft == isProblemObjectSideLeft)
                            {
                                int g = objectId; int d; bool issecondLeft; bool isFirstLeft = listaPrzewodow[listZlychPrzewodow[i]].isSecondObjectLeft; ;
                                d = listaPrzewodow[listZlychPrzewodow[i]].firstObject;
                                issecondLeft = listaPrzewodow[listZlychPrzewodow[i]].isFirstObjectLeft;

                                Console.WriteLine("Usuniecie polaczenia:" + d + "a elementem " + objectId);

                                if (guard)
                                {
                                    guard = false;
                                }
                                Console.WriteLine("Dodanie pomiedzy NODE" + (objectList.getLenght() - 1) + "a elementem " + d);
                                nowePrzewody.Add(new Wire(nodeID, d, !isFirstLeft, isFirstLeft));

                            }

                        }
                        // Console.WriteLine("Dodanie poza forem:" + (objectList.getLenght() - 1) + "a elementem " + objectId);
                        // listaPrzewodow.Add(new Wire(objectId, objectList.getLenght() - 1,_isSecondLeft, _isFirstLeft ));
                        //  Console.WriteLine("Dodanie poza forem:" + (objectList.getLenght() - 1) + "a elementem " + startID);
                        // listaPrzewodow.Add(new Wire(objectList.getLenght() - 1, startID, _isSecondLeft, _isFirstLeft));
                        listaPrzewodow.AddRange(nowePrzewody);

                        listZlychPrzewodow.Sort();
                        for (int i = listZlychPrzewodow.Count() - 1; i >= 0; i--) listaPrzewodow.Remove(listaPrzewodow[listZlychPrzewodow[i]]);

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
        int czyPosiadaNode(int id, bool isLeft, ObjectList obj)
        {
            for (int i = 0; i < listaPrzewodow.Count; i++)
            {
                if (listaPrzewodow[i].firstObject == id && listaPrzewodow[i].isFirstObjectLeft == isLeft)
                {
                    if (obj.isObjectNode(listaPrzewodow[i].secondObject))
                    {
                        return listaPrzewodow[i].secondObject;
                    }
                }
                else if (listaPrzewodow[i].secondObject == id && listaPrzewodow[i].isSecondObjectLeft == isLeft)
                {
                    if (obj.isObjectNode(listaPrzewodow[i].firstObject))
                    {
                        return listaPrzewodow[i].firstObject;
                    }

                }
            }
            return -1;
        }
        public void KillTheObject (int a)
        {
            for (int i=0; i< listaPrzewodow.Count; i++)
            {
                if (listaPrzewodow[i].firstObject == a || listaPrzewodow[i].secondObject == a)
                {
                    listaPrzewodow.RemoveAt(i);
                    i--;
                }

            }
        }
        public Tuple<Stack<int>, Tuple<int, int>> getConnections(int id)
        {
            int left = 0, right = 0;
            Stack<int> ret = new Stack<int>();
            for (int i = 0; i < listaPrzewodow.Count(); i++)
            {
                if (listaPrzewodow[i].firstObject == id)
                {
                    ret.Push(listaPrzewodow[i].secondObject);
                    if (listaPrzewodow[i].isFirstObjectLeft) left++;
                    else right++;
                }
                if (listaPrzewodow[i].secondObject == id)
                {
                    ret.Push(listaPrzewodow[i].firstObject);
                    if (listaPrzewodow[i].isSecondObjectLeft) left++;
                    else right++;
                }

            }
            return new Tuple<Stack<int>, Tuple<int, int>>(ret, new Tuple<int, int>(left, right));
        }
        public void isCircuitClosed(ObjectList obj)
        {
            Dictionary<int, int> objMap = new Dictionary<int, int>();
            int i = 0, a = 0;
            Stack<int> stosz;
            Stack<int> stosz2;
            foreach (var item in obj.returnLista())
            {
                objMap.Add(i, 0);
                i++;
            }
            stosz = (getConnections(0).Item1);
            if (getConnections(0).Item2.Item1 == 0 || getConnections(0).Item2.Item2 == 0)
            {
                if (!obj.isObjectNode(0))
                {
                    isTheWireCorrect = false; return;
                }
            }
            while (stosz.Count() > 0)
            {
                int g;
                g = stosz.First();
                stosz.Pop();
                if (objMap[g] == 1) continue;
                objMap[g] = 1;
                a++;
                stosz2 = (getConnections(g).Item1);
                if (getConnections(g).Item2.Item1 == 0 || getConnections(g).Item2.Item2 == 0)
                {
                    if (!obj.isObjectNode(0))
                    {
                        isTheWireCorrect = false; return;
                    }
                }
                while (stosz2.Count > 0)
                {
                    if (objMap[stosz2.First()] != 1)
                    {
                        stosz.Push(stosz2.First());

                    }
                    stosz2.Pop();
                }

            }
            if (i == a)
            {
                isTheWireCorrect = true;
            }
            else
            {
                isTheWireCorrect = false;
            }

        }

    }




}
