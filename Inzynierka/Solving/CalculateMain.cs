using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.Encodings.Web;
using ElectricData;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using static Program;

namespace Calculations
{
    public struct WireParts
    {
        public List<int> ElementsInWire;
        public int point1, point2;
        public double collectiveResistance;
        public double collectiveVoltage;
        public double collectiveCurrent;
        public bool wasTouched;
    }
    public static class CalculateMain
    {
        public static Dictionary <int, Tuple<int,int>> wireState;

        public static void CreateWireState (Hierophant h)
        {
            wireState = new Dictionary<int, Tuple<int, int>>();
            for (int i =0; i<h.Nodes.Count(); i++)
            {
                if (h.wireConnections[h.Nodes[i]].Count > 1);
                wireState.Add(h.Nodes[i], new Tuple<int,int>(0, h.wireConnections[h.Nodes[i]].Count));

            }
        }
        public static Hierophant createHierophant(ObjectList obj, ConnectionList con)
        {
            Hierophant ret = new Hierophant();
            for (int i =0 ;i < obj.getLenght(); i++)
            {
                ret.AddObject(i,obj.returnLista()[i]);

                List<Wire> a1 = con.GetWiresForOneLEFT(i);
                List<Wire> a2 = con.GetWiresForOneRIGHT(i);

                if (!ret.wireConnections.ContainsKey(i+ 1000))
                {
                ret.AddConnection(i + 1000, i + 1000);
                }
                if (!ret.wireConnections.ContainsKey(i+ 2000))
                ret.AddConnection(i + 2000, i + 2000);                    
                

                foreach (var item in a1)
                {
                        if (i ==item.secondObject)
                        {
                        int h = ret.AddConnection(item.firstObject + (item.isFirstObjectLeft ? 2000 : 1000), i + 2000);
                        if (h%1000!=item.firstObject) ret.AddConnectionSwallow(h,i+2000);
                        ret.AddConnection(h, item.firstObject + (item.isFirstObjectLeft ? 2000 : 1000));
                        }
                        else
                        {
                        int h =ret.AddConnection(item.secondObject + (item.isSecondObjectLeft ? 2000 : 1000), i + 2000);
                        if (h%1000!=item.secondObject) ret.AddConnectionSwallow(h,i+2000);
                        ret.AddConnection(h, item.secondObject + (item.isSecondObjectLeft ? 2000 : 1000));
                        }
                }
                foreach (var item in a2)
                {
                        if (i ==item.secondObject)
                        {
                        int h =ret.AddConnection(item.firstObject + (item.isFirstObjectLeft ? 2000 : 1000),i + 1000);
                        if (h%1000!=item.firstObject) ret.AddConnectionSwallow(h, i+1000);
                        ret.AddConnection(h, item.firstObject + (item.isFirstObjectLeft ? 2000 : 1000));

                        }
                        else
                        {
                        int h =ret.AddConnection(item.secondObject + (item.isSecondObjectLeft ? 2000 : 1000), i + 1000);
                       if (h%1000!=item.secondObject) ret.AddConnectionSwallow(h,i+1000);
                        ret.AddConnection(h, item.secondObject + (item.isSecondObjectLeft ? 2000 : 1000));

                        }
                }
            }   

            ret.RemoveUselessNodes();
            CreateWireState(ret);
            for (int i=0; i<ret.Nodes.Count; i++)
            {
                var g = ret.wireConnections[ret.Nodes[i]];
                if (g.Count > 2)
                {
                    for (int j =0; j<g.Count; j++)
                    {
                        WireParts wp = new WireParts();
                        wp.ElementsInWire = new List<int>();
                        wp.point1 = g[0];
                        int r = goTillTheEndOfWire(ref wp, g[j], ret);
                        if (r == -1)
                        continue;
                        else
                        {
                            if (wp.collectiveVoltage > 0 || wp.collectiveCurrent >0) ret.EnergySourcesWiresNumber ++;
                            if (wp.collectiveVoltage <0 || wp.collectiveCurrent <0 )
                            {
                                int dsadadwada = wp.point1;
                                wp.point1 = wp.point2;
                                wp.point2 = dsadadwada;
                                wp.collectiveVoltage *= -1;
                                wp.collectiveCurrent *= -1;
                            } 
                            wireState[ ret.Nodes[i]] = new Tuple<int, int>(wireState[ ret.Nodes[i]].Item1 + 1, wireState[ret.Nodes[i]].Item2);
                            ret.wireParts.Add(wp);

                            if (!ret.branchConnectionsToPoints.ContainsKey(wp.point1)) ret.branchConnectionsToPoints.Add(wp.point1, new List<int>());
                            if (!ret.branchConnectionsToPoints.ContainsKey(wp.point2)) ret.branchConnectionsToPoints.Add(wp.point2, new List<int>());
                            ret.branchConnectionsToPoints[wp.point1].Add(ret.wireParts.Count()-1);
                            ret.branchConnectionsToPoints[wp.point2].Add(ret.wireParts.Count()-1);

                        }
                    }
                }
            }

        return ret;
        }
    
        public static int goTillTheEndOfWire (ref WireParts wireParts, int element, Hierophant h)
        {
            int nextElement;
            if (element >= 2000) nextElement = element - 1000;
            else nextElement = element + 1000;
            wireParts.ElementsInWire.Add(element%1000);
            switch (h.objectsDictionary[element%1000].objectName )
            {
                case "Resistor":
                wireParts.collectiveResistance += h.objectsDictionary[element%1000].objectValues_list[0].Item2;
                break;
                case "Cell":
                //prad leci od 1000 do 2000
                if (element >=2000 && nextElement < 2000)
                wireParts.collectiveVoltage -= h.objectsDictionary[element%1000].objectValues_list[0].Item2;
                else if (element <2000 && nextElement >= 2000)
                wireParts.collectiveVoltage += h.objectsDictionary[element%1000].objectValues_list[0].Item2;
                break;
                case "ACSource":
                if (element >=2000 && nextElement < 2000)
                wireParts.collectiveCurrent -= h.objectsDictionary[element%1000].objectValues_list[0].Item2;
                else if (element <2000 && nextElement >= 2000)
                wireParts.collectiveCurrent += h.objectsDictionary[element%1000].objectValues_list[0].Item2;
                break;
            }

            if (h.wireConnections[h.getDaddy(nextElement)].Count >= 3)
            {
                int lol = h.getDaddy(nextElement);
                if (wireState[lol].Item2 == wireState[lol].Item1)
                {
                    wireParts = new WireParts();
                    return -1;
                }
                wireState[lol] = new Tuple<int, int>(wireState[lol].Item1 + 1, wireState[lol].Item2);
                wireParts.point2 = lol;
                
               // wireParts.point2 = 
            }
            else
            {
                int dad = h.getDaddy(nextElement);
                if (dad % 1000 == nextElement % 1000) dad = h.wireConnections[dad][1];
                return goTillTheEndOfWire (ref wireParts, dad,h);
            }
            return 1;
        }
    
        public static string CalculateMainHub (int a)
        {
             ObjectList obj = ObjectList.getInstance();
             ConnectionList con = ConnectionList.getInstance();
            if (!con.isCircuitClosed(obj)) return "Blad obwodu!";
            Hierophant mainH = createHierophant(obj, con);
            SimpleCalculations l = new SimpleCalculations();
            string s = l.CalculateEqResistance(mainH);
            finalEquationSolver finalEquationSolver = new finalEquationSolver();
            finalEquationSolver.getEquations(mainH);
            finalEquationSolver.Loop(mainH);
            return s;
        }    
    }
}