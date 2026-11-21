using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text.Encodings.Web;
using ElectricData;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input.Hid;
using OpenTK.Mathematics;
using static Program;

namespace Calculations
{
    public struct NodeEquations
    {
        public int startCurrentID;
        public List<int> CurrentsID;
    }
    public struct EyesEquations
    {
        public int startVoltageID;
        public  List<int> VoltagesID;
    }
    public class SimpleCalculations
    {
        Hierophant h;
        List<double> resistorList;
            string s ="";

        public string CalculateEqResistance(Hierophant h2)
        {
            double r = 0;
            this.h = h2;
            resistorList = new List<double>();
            for (int i =0; i<h.RetObjectsCount(); i++)
            {
                if (h.objectsDictionary[i].objectName == "Resistor")
                {
                    resistorList.Add(h.getValue(i,objectValues.Resistance));
                }
                else  resistorList.Add(-0.33);
            }
            s += "Poczatek programu! Obliczamy rezystancje zastepcza!\n";
            while (true)
            {
                int i =0;
                for (i =0; i<h.Nodes.Count(); i++)
                {
                    if (verifyEmptyLines())
                    {
                        break;
                    }
                    if (inSeries(h.Nodes[i]))
                    {
                        break;
                    }

                    if (parallel(h.Nodes[i]))
                    {
                        break;
                    }
                    

                }
                if (i == h.Nodes.Count()) break;
                //zweryfiukujmy czy został tylko jeden rezystor, jesli tak, to zakonczmy program
            r = 0;
            for (int j=0; j<resistorList.Count(); j++)
            { //idziemy po rezystorach
                if (resistorList[j] == -1 ||resistorList[j] == -0.33 ) continue; //jesli rezystor sprzatniety lub generator frajdy, skip
                else //ale jesli...
                {
                    if (r != 0)
                        {
                            r = -1;
                        }
                    else r = resistorList[j];
                }
            }
            if (r>0)
            {
                h.CollectiveResistance =r;
                CalculateMoreStuff_one_source (h);

            s+="Koniec programu!\n";
            return s;
            }
            }
            r = 0;
            for (int j=0; j<resistorList.Count(); j++)
            {
                if (resistorList[j] == -1 ||resistorList[j] == -0.33 ) continue;
                else
                {
                    if (r != 0)
                        {
                            r = -1;
                            CalculateMoreStuff_one_source (h);
                            s+="Nie da sie obliczyc! Koniec programu!\n";
                            return s;
                        }
                    r = resistorList[j];
                }
            }
            h.CollectiveResistance = r;
            CalculateMoreStuff_one_source (h);
            s+="Wynik to: " + r +"\n";
            s+="Koniec programu\n";
            Console.Write(s);
            return s;

            
           
        }
        public bool inSeries (int node)
        {
            double r = 0;
            if (h.wireConnections[node].Count() == 2)
            {
                int obj1 = h.wireConnections[node][0] % 1000;
                int obj2 = h.wireConnections[node][1] % 1000;
                if (obj1 == obj2)
                {
                h.RemoveElement(h.wireConnections[node][0], h.wireConnections[node][1]);
                return true;
                }
                if (resistorList[obj1] == -0.33 || resistorList[obj2] == -0.33) return false;
                int otherNode;
                r =  resistorList[obj1] + resistorList[obj2];
                double r1 =  resistorList[obj1], r2= resistorList[obj2];
                if (h.wireConnections[node][1] >= 2000) otherNode = 1000 +obj2;
                else otherNode = 2000 + obj2;
                h.AddConnectionSwallow(h.wireConnections[node][0],otherNode);
                h.RemoveElement(h.wireConnections[node][0],otherNode);
                resistorList[obj1] = r;
                resistorList[obj2] = -1;
                s += "Uzywamy polaczenia szeregowego na rezystorach " + obj1 + " i " + obj2 + "\n";
                s += "Obliczamy: " + r1.ToString("F2") + " + " + r2.ToString("F2") + " = " + r.ToString("F2")  + "\n";
                s += "Pozostaje rezystor " + h.wireConnections[node][0] % 1000 + " o rezystancji " + r.ToString("F2") + "\n";
                h.RemoveKilledNodes(obj2);
                return true;
            }
            return false;
        }
        public bool parallel (int node)
        {
            string sa ="";
            for (int i =0; i<h.Nodes.Count(); i++)
            {
                if (h.Nodes[i]!= node)
                {
                    List <int> a = h.CheckFamiliarity(node,h.Nodes[i]);
                    
                    if (a.Count() >= 2)
                    {
                        int test_ = 0;
                        sa += "Uzywamy polaczenia rownoleglego na rezystorach: " + a[0];

                        int obj1 = a[0] % 1000;
                        if (resistorList[obj1] == -0.33) return false;
                        for (int j = 1; j<a.Count(); j++)
                        {
                            double r = resistorList[a[0]];
                            int obj2 = a[j] % 1000;
                             double r1 =resistorList[a[0]], r2 = resistorList[obj2];
                            if (resistorList[obj2] < 0) continue;
                            if (obj1 == obj2) continue;
                            r = r * resistorList[obj2] / ( r +  resistorList[obj2]);
                            h.searchAndDie(a[0], 1000 + a[j]);
                            h.searchAndDie(a[0], a[j] + 2000 );
                            resistorList[obj2] = -1;
                            sa+= "i " +  obj2 + "\n";
                            sa += "Obliczamy :" + r1.ToString("F2") + " * " + r2.ToString("F2") + " / " + r1.ToString("F2") + " + " + r2.ToString("F2") + " + " + r.ToString("F2") + "\n";
                            sa += "Pozostaje rezystor " + a[0] % 1000 + " o wartosci " + r.ToString("F2") + "\n";
                            resistorList[a[0]] = r;
                            test_ ++;
                            h.RemoveKilledNodes(obj2);

                        }
                        if (test_ >0)
                        {
                            s += sa;
                        return true;

                        }
                        else return false;
                    }
                    
                }
            }
            return false;
        } 
    
        public bool verifyEmptyLines()
        {
    bool changed = false;
    for (int i = 0; i < h.Nodes.Count(); i++)
    {
        int l = h.Nodes[i];
        var connections = h.wireConnections[l];

        // Sprawdzamy pary połączeń 1000 + x oraz 2000 + x
        for (int j = connections.Count - 1; j >= 0; j--)
        {
            int connection = connections[j];

            if (connection < 1000 || connection >= 3000)
                continue;

            int x = connection % 1000;

            int first = 1000 + x;
            int second = 2000 + x;

            if (connections.Count <= 2 && connections.Contains(first) &&
                connections.Contains(second))
            {
                // Usuwamy oba połączenia
                connections.Remove(first);
                connections.Remove(second);

                // Dezaktywujemy rezystor
                resistorList[x] = -1;

                s += "Usuwamy rezystor " + x
                    + ", ponieważ znaleziono połączenia "
                    + first + " i " + second + ".\n";

                changed = true;

                // Lista uległa zmianie, więc zaczynamy od nowa
                j = connections.Count;
            }
        }

        // Dotychczasowa logika wykrywania pustego połączenia
        for (int j = 0; j < connections.Count; j++)
        {
            int connection = connections[j];

            if (connections.Count <= 2 &&connection % 1000 == l % 1000 &&
                connection != l)
            {
                int obj = l % 1000;

                resistorList[obj] = -1;

                s += "Usuwamy rezystor " + obj
                    + ", ponieważ istnieje puste połączenie.\n";

                changed = true;
                break;
            }
        }
    }

    return changed;
            }
         
        public bool CellsInfo ()
        {
            for (int i=0 ;i<resistorList.Count; i++ )
            {
                if (resistorList[i] == -0.33f)
                {
                    
                }
            }

            return false;
        }
        
    
    int REVERSE (int a)
        {
            return -a -1;
        }
        void CalculateMoreStuff_one_source (Hierophant h)
        {
            if (h.EnergySourcesWiresNumber> 1)
            {
                return;
            }
            double TotalVoltage = 0;
            int mainWire = -1;
            for (int i=0; i<h.wireParts. Count; i++) 
            {
                if (h.wireParts[i].collectiveVoltage > 0 || h.wireParts[i].collectiveCurrent > 0) mainWire = i;
                TotalVoltage += h.wireParts[i].collectiveVoltage;
            }
            if (mainWire<0) return;

            //obliczenie current dla obwodu glownego, potem mozna isc dalej
            WireParts l = h.wireParts[mainWire];
            if (h.CollectiveResistance > 0)
            {
                l.collectiveCurrent =  h.wireParts[mainWire].collectiveVoltage / h.CollectiveResistance;
                l.collectiveVoltage -=  l.collectiveCurrent* h.wireParts[mainWire].collectiveResistance;
            }
            h.wireParts[mainWire] = l;
            s+="Napiecie na calym obwodzie=" + l.collectiveCurrent.ToString() + "\n";
            HashSet<int> bleh = new HashSet<int>();
            HashSet<int> bleh2 = new HashSet<int>();
            ProceedNodesCalculations(h,h.wireParts[mainWire].point2, bleh, mainWire);
            //Przygotowanie wzorow na wezly
            ProceedEyesCalculations(h,h.wireParts[mainWire].point2, bleh2, new List<int>(), mainWire, h.wireParts[mainWire].point2, mainWire,0 );
            
        }

        void ProceedNodesCalculations(Hierophant h, int point, HashSet<int> ha, int direction)                                                                          
        {
            if (ha.Contains(point)) return;
            ha.Add(point);
            NodeEquations a = new NodeEquations();
            List <Tuple<int,int>> newPoints = new List<Tuple<int,int>>();
            a.startCurrentID = direction;
            a.CurrentsID = new List<int>();
            for (int i=0; i<h.branchConnectionsToPoints[point].Count(); i++)
            {
                if (h.branchConnectionsToPoints[point][i]==direction)
                {
                    var l= h.wireParts[h.branchConnectionsToPoints[point][i]];
                    l.wasTouched = true;
                    h.wireParts[h.branchConnectionsToPoints[point][i]] = l;
                    continue;
                } 
                var g = h.wireParts[h.branchConnectionsToPoints[point][i]];
                int pathID =h.branchConnectionsToPoints[point][i];
                //przypadki:
                //Zle ustawiony przez domyslne tworzenie: obroc i przypisz
                //Zle ustawiony JUŻ PRZEZ ZMIANE: Dodaj jako ujemnego lorda
                //Dobrze, no to dobrze, nie ma co się produkować
                if (g.point2 == point && !g.wasTouched)
                {
                    int dsadadwada = g.point2;
                    g.point2 = g.point1;
                    g.point1 = dsadadwada;
                    g.wasTouched = true;
                    h.wireParts[h.branchConnectionsToPoints[point][i]] = g;
                    a.CurrentsID.Add(h.branchConnectionsToPoints[point][i]);
                    newPoints.Add(new Tuple<int,int>(g.point2,pathID));
                }
                else if (g.point2 == point && g.wasTouched)
                {
                    g.wasTouched = true;
                    h.wireParts[h.branchConnectionsToPoints[point][i]] = g;
                    a.CurrentsID.Add(REVERSE(h.branchConnectionsToPoints[point][i]));
                    newPoints.Add(new Tuple<int,int>(g.point1,pathID));
                }
                else if (g.point1 == point)
                {
                    g.wasTouched = true;
                    h.wireParts[h.branchConnectionsToPoints[point][i]] = g;
                    a.CurrentsID.Add(h.branchConnectionsToPoints[point][i]);
                    newPoints.Add(new Tuple<int,int>(g.point2,pathID));
                }
            }
            h.NodesCurrentsEquations.Add(a);
            for (int i=0; i<newPoints.Count; i++)
            ProceedNodesCalculations(h, newPoints[i].Item1,ha, newPoints[i].Item2);
        }

        void ProceedEyesCalculations(Hierophant h, int point,  HashSet<int> ha, List<int> a, int direction, int mainEndPoint, int mainEndWire, int reverse)
        {
            HashSet<int> bleh = new();
            for (int j =0; j<a.Count(); j++) bleh.Add(a[j]);
            if ( ha.Contains(point))
            {
                if (direction == mainEndWire && point == mainEndPoint)
                {
                    a.RemoveAt(0);
                    EyesEquations g = new EyesEquations();
                    g.startVoltageID = mainEndWire;
                    g.VoltagesID = new List<int>(a);
                    h.EyesCurrentsEquations.Add(g);
                    return;
                }
                else if (a.Count!=0 && direction != REVERSE(a[a.Count()-1]))
                {
                    List <int> z = new List<int>(a);
                    int countWires = 0;
                    int p1= -999, p2 = -999;
                    for (int i=0; i<h.branchConnectionsToPoints[point].Count; i++)
                    {
                        if (bleh.Contains(h.branchConnectionsToPoints[point][i]))
                        {
                            countWires++;
                            if (p1 == -999) p1 =h.branchConnectionsToPoints[point][i];
                            else p2 = h.branchConnectionsToPoints[point][i];
                        }
                    }
                    if (countWires < 2) return;
                    while (true)
                    {
                        if (z[0] == direction) return;
                        if (z[0]==p1)
                        {
                            z.RemoveAt(0);
                            break;
                        }
                        else if (z[0]==p2)
                        {
                            z.RemoveAt(0);
                            break;
                        }
                        else
                        {
                            z.RemoveAt(0);
                        }
                    }
                    if (reverse != 1)
                    {
                        for (int i=0; i<z.Count(); i++)
                        {
                        z[i] = REVERSE(z[i]);
                        }
                    }
                    
                    EyesEquations g = new EyesEquations();
                    g.startVoltageID = direction;
                    g.VoltagesID = new List<int>(z);
                    h.EyesCurrentsEquations.Add(g);
                    return;
                }
                else return;
            }
            ha.Add(point);
            if (reverse == 1) a.Add(REVERSE(direction));
            else a.Add(direction);
            for (int i=0; i<h.branchConnectionsToPoints[point].Count(); i++)
            {
                if(h.branchConnectionsToPoints[point][i] == direction) continue;

                WireParts wire = h.wireParts[h.branchConnectionsToPoints[point][i]];
                
                int exitPoint = 0, doReverse = 0;
                if (wire.point1 == point)  exitPoint = wire.point2;
                else if (wire.point2 == point ) 
                {
                    doReverse = 1;
                    exitPoint = wire.point1;
                }
                else continue;


                ProceedEyesCalculations(h,exitPoint,new HashSet<int>(ha), new List<int>(a),h.branchConnectionsToPoints[point][i], mainEndPoint, mainEndWire, doReverse);


            }
        }
   }
}
