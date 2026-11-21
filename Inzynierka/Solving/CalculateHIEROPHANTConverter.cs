using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text.Encodings.Web;
using ElectricData;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using static Program;

namespace Calculations
{
    public class Hierophant
    {
        int objectsCount = 0;
        public List<int> Nodes = new List<int>();
        public Dictionary<int, ElectricObject> objectsDictionary;
        public Dictionary<int, List<int>> wireConnections; 
        public Dictionary<int, List<int>> branchConnectionsToPoints; //punkty i wsztstkie branche ktore z nich wychodza

        public int EnergySourcesWiresNumber = 0;
        public double CollectiveResistance = 0;
        public List<WireParts> wireParts; //wszystkie branche podlaczone do punktow
        public List<NodeEquations> NodesCurrentsEquations = new List<NodeEquations>();

        public List<EyesEquations> EyesCurrentsEquations = new List<EyesEquations>();
        public Hierophant()
        {
            objectsDictionary = new Dictionary<int, ElectricObject>();
            wireConnections = new Dictionary<int, List<int>>();
            wireParts = new List<WireParts>();
            branchConnectionsToPoints = new Dictionary<int, List<int>>();
            
        }
        public  void AddObject(int i, ElectricObject e)
        {
            objectsDictionary.Add(i,e);
            objectsCount++;
        }
        public int getDaddy(int j)
        {
            if (wireConnections[j][0] != j) return getDaddy(wireConnections[j][0]);
            else return j;
        }
        public int AddConnection (int j, int k)
        {
            if (!wireConnections.ContainsKey(j))
            {
            wireConnections.Add(j,new List<int>());
            wireConnections[j].Add(k);
            return j;
            }
            else
            {
                if (wireConnections[j][0]!=j)
                {
                    return AddConnection(wireConnections[j][0], k); 
                }
                else
                wireConnections[j].Add(k);
                return j;
            }

        }
        public List<int> CheckFamiliarity(int node1, int node2)
        {
            List<int> ret = new List<int>();
            HashSet<int> n1 = new HashSet<int>();

            for (int i=0; i<wireConnections[node1].Count(); i++) n1.Add(wireConnections[node1][i] % 1000);
            for (int i=0; i<wireConnections[node2].Count(); i++) 
            if (n1.Contains(wireConnections[node2][i] % 1000)) ret.Add(wireConnections[node2][i] % 1000);
            return ret;
        }
        public int AddConnectionSwallow(int j, int k)
        {
            if (getDaddy(k)!=k && getDaddy(k) % 1000 != j % 1000 ) //jesli nie jest dominujący, przenieś mnie do dominującego ALE
            {
                return AddConnectionSwallow(j, getDaddy(k));
            }
            else if (getDaddy(k)!=k && getDaddy(k) % 1000 == j % 1000 && getDaddy(k) != j )
            {
                int g = getDaddy(k);
                for (int i =0; i<wireConnections[g].Count(); i++)
                if (!isNodeAlreadyIn(j,wireConnections[g][i])) wireConnections[j].Add(wireConnections[g][i]);
                wireConnections[g] =new List<int>();
                wireConnections[g].Add(j);
                return g;
            }
            else
            {
                for (int i =0; i<wireConnections[k].Count(); i++)
                if (!isNodeAlreadyIn(j,wireConnections[k][i])) wireConnections[j].Add(wireConnections[k][i]);
                wireConnections[k] =new List<int>();
                wireConnections[k].Add(j);
                return k;
            }
        }
        public bool isNodeAlreadyIn(int j, int k)
        {
            if(!wireConnections.ContainsKey(j)) return false;
            for (int i =0; i< wireConnections[j].Count; i++)
            {
                if (wireConnections[j][i] == k) return true;
            }
            return false;
        }
        public void searchAndDie(int j, int k)
        {
            //3 pryzpadki:
            //1 : element nie jest rodzicem i jest już przypisany do tego rezystora który jest "główny"
            //2 : element jest rodzicem i trzeba go ukatrupić
            //3 : element nie jest rodzicem, ale jest przypisany do kolejnego rodzica nie bedacego tym rezystorem.
            if (wireConnections[k].Count == 0) return;
            if (getDaddy(k)% 1000== j) //opcja 1
            {
            RemoveElement(wireConnections[k][0], k);
            return;
            }
            //opcja 2
            if (getDaddy(k) == k)
            {
                //znajdujemy po ktorej stronie oryginalnego rezystora sie znajduje
                for (int i=1; i<wireConnections[k].Count(); i++)
                {
                    if (wireConnections[k][i] % 1000 == j) //znajdujemy stronę
                    {
                        int lg = wireConnections[k][i];
                        AddConnectionSwallow(lg, k);
                        RemoveElement(lg,k);
                        Nodes.Add(lg);
                        
                    }
                }
                return;
            }
            //opcja 3
            else
            {
                for (int i=0; i<wireConnections[k].Count(); i++)
                {
                    if (wireConnections[k][i] % 1000 == j) //znajdujemy stronę
                    {
                        int lg = wireConnections[k][i];
                        int k2 = getDaddy(k);
                        AddConnectionSwallow(lg, k2);
                        RemoveElement(lg,k2);
                    }
                }
            }

        }
        public void RemoveElement(int j,int k)
        {
            int hk = k;
            k = k% 1000;
            for (int i = Nodes.Count - 1; i >= 0; i--)
            {
                if (Nodes[i]== hk)
                Nodes.RemoveAt(i);
            }
            for (int i = wireConnections[j].Count - 1; i >= 0; i--)
            {
                if (wireConnections[j][i] % 1000 == k)
                wireConnections[j].RemoveAt(i);
            }
        }
        public void RemoveUselessNodes()
        {
           foreach (var item in wireConnections)
            {
                if (item.Value.Count() >= 2) //wireConnections.Remove(item.Key);
                {
                    wireConnections[item.Key] = wireConnections[item.Key].Distinct().ToList();
                    Nodes.Add(item.Key);
                }
            }

        }
        public void RemoveKilledNodes(int k)
        {
            foreach (var item in wireConnections)
            {
                for(int i=item.Value.Count() -1; i>=0; i--) 
                if (item.Value[i] % 1000 == k) 
                item.Value.RemoveAt(i);
            }  
        }
        public int RetObjectsCount ()
        {
            return objectsCount;
        }
        public double getValue (int id, objectValues v)
        {
            for (int i=0; i<objectsDictionary[id].objectValues_list.Count();i++)
            {
                if (objectsDictionary[id].objectValues_list[i].Item1 == v) return objectsDictionary[id].objectValues_list[i].Item2;
            }            
            return -1;
        }
        public double setValue (int id, objectValues v, double r)
        {
            for (int i=0; i<objectsDictionary[id].objectValues_list.Count();i++)
            {
                if (objectsDictionary[id].objectValues_list[i].Item1 == v) objectsDictionary[id].objectValues_list[i] = new Tuple<objectValues, double>(v,r);
            }            
            return -1;
        }
        
    }
}