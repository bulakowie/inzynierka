using Calculations;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Text.Encodings.Web;
using System.Windows.Markup;
using ElectricData;
using OpenTK.Graphics.ES20;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input.Hid;
using OpenTK.Mathematics;
using static Program;
public  class PrepareEquations()
{
    public Hierophant h;
    public Dictionary<string, List<Equation>> Encyclopedia = new Dictionary<string, List<Equation>>();

    public Dictionary<string, double> ActualValues = new Dictionary<string, double>();

    public Dictionary<string,bool> isKnown = new Dictionary<string, bool>();

    public void getEquations (Hierophant h,ref Dictionary<string,  List<Equation>> e,ref Dictionary<string, double> a,ref Dictionary<string,bool> u )
    {
        getEyesEquationsIntoEquations(h);
        getNodesEquationsIntoEquations(h);
        writeOwnedValues(h);
        Ohm(h);
        e = this.Encyclopedia;
        a = this.ActualValues;
        u = this.isKnown;
    }
    int REVERSE(int a)
        {
         return -a -1;
        }
    public void getEyesEquationsIntoEquations(Hierophant h)
{
    for (int i = 0; i < h.EyesCurrentsEquations.Count; i++)
    {
        EyesEquations n = h.EyesCurrentsEquations[i];
        EyeToEquationTransform(n, h);

       /* int startWire = n.startVoltageID;
        for (int j = 0; j < n.VoltagesID.Count; j++)
        {
            EyesEquations g = n;

            g.VoltagesID = new List<int>(n.VoltagesID);

            int movedCurrent = g.VoltagesID[j];
            g.startVoltageID = movedCurrent;
            g.VoltagesID[j] = REVERSE(startWire);

            if (g.startVoltageID < 0)
            {
                g.startVoltageID = REVERSE(g.startVoltageID);

                for (int k = 0; k < g.VoltagesID.Count; k++)
                    g.VoltagesID[k] = REVERSE(g.VoltagesID[k]);
            }

            EyeToEquationTransform(g, h);
        } */
    }
}
        
        public void Ohm (Hierophant h)
        {
            for (int i=0; i<h.wireParts.Count; i++)
            {
                WireParts n = h.wireParts[i];
                if (n.collectiveResistance >0)
                {
                    Equation alfa = new Equation();
                    alfa.values = new List<string>();
                    string a = "+V" + i;
                    string b = "+I" + i;
                    string c = "*+R" + i;
                    alfa.values.Add(b);
                    alfa.values.Add(c);
                    if (!Encyclopedia.ContainsKey(a)) Encyclopedia[a] = new List<Equation>();
                    Encyclopedia[a].Add(alfa);

                    alfa = new Equation();
                    alfa.values = new List<string>();
                    a = "+R" + i;
                    b = "+V" + i;
                    c = "/+I" + i;
                    alfa.values.Add(b);
                    alfa.values.Add(c);
                    if (!Encyclopedia.ContainsKey(a)) Encyclopedia[a] = new List<Equation>();
                    Encyclopedia[a].Add(alfa);

                     alfa = new Equation();
                    alfa.values = new List<string>();
                    a = "+I" + i;
                    b = "+V" + i;
                    c = "/+R" + i;
                    alfa.values.Add(b);
                    alfa.values.Add(c);
                    if (!Encyclopedia.ContainsKey(a)) Encyclopedia[a] = new List<Equation>();
                    Encyclopedia[a].Add(alfa);
                }
            }
        }
        public void NodeToEquationTransform ( NodeEquations n , Hierophant h)
        {
            Equation l = new Equation();
            l.values = new List<string>();
            string a = "";
            for (int i=0; i<n.CurrentsID.Count; i++)
            {
                if (n.CurrentsID[i] >= 0) a = "+I" + n.CurrentsID[i];
                else a="-I" + REVERSE(n.CurrentsID[i]);

                l.values.Add(a);
                a = "";
            }
            a = "+I" + n.startCurrentID;
            if (!Encyclopedia.ContainsKey(a)) Encyclopedia[a] = new List<Equation>();
            Encyclopedia[a].Add(l);
            return;
        }
        public void EyeToEquationTransform ( EyesEquations n , Hierophant h)
        {
            Equation l = new Equation();
            l.values = new List<string>();
            string v = "";
            string a = "";
            a = "+V" + n.startVoltageID;
            
            for (int i=0; i<n.VoltagesID.Count; i++)
            {
                if (n.VoltagesID[i] >= 0) v = "+V" + n.VoltagesID[i];
                else v ="-V" + REVERSE( n.VoltagesID[i]);
                l.values.Add(v);
            }
            if (!Encyclopedia.ContainsKey(a)) Encyclopedia[a] = new List<Equation>();
            Encyclopedia[a].Add(l);
            return;
        }
        public void writeOwnedValues(Hierophant h)
        {
              string s;
            for (int i=0; i<h.wireParts.Count; i++)
            {
                WireParts n = h.wireParts[i];
                 s = "+R" + i;
                if (n.collectiveResistance > 0)
                {
                   
                    ActualValues[s] = n.collectiveResistance;
                    isKnown[s]= true; 
                }
                else isKnown[s]= false;
                s = "+V" + i;
                if (n.collectiveVoltage > 0)
                {
                    ActualValues[s] = n.collectiveVoltage;
                    isKnown[s]= true; 
                }
                else isKnown[s]= false;
                s = "+I" + i;
                if (n.collectiveCurrent >0)
                {
                     ActualValues[s] = n.collectiveCurrent;
                    isKnown[s]= true; 
                }
                else isKnown[s]= false;
            } 
        }

        public void getNodesEquationsIntoEquations(Hierophant h)
        {
            for (int i=0; i<h.NodesCurrentsEquations.Count; i++)
            {
                NodeEquations n = h.NodesCurrentsEquations[i];
                NodeToEquationTransform(n, h);
                for (int j=0; j<h.NodesCurrentsEquations[i].CurrentsID.Count(); j++)
                {
                    NodeEquations g = h.NodesCurrentsEquations[i];
                    g.CurrentsID = new List<int>(g.CurrentsID);
                    int startWire = g.startCurrentID;
                    int lol =g.CurrentsID[j];
                    g.CurrentsID[j]= REVERSE(startWire);
                    g.startCurrentID = REVERSE(lol);

                    if (g.startCurrentID < 0)
                    {
                        g.startCurrentID = REVERSE(g.startCurrentID);
                        for (int adwa = 0; adwa< g.CurrentsID.Count; adwa++) g.CurrentsID[adwa] = REVERSE(g.CurrentsID[adwa]);
                    }
                    NodeToEquationTransform(g, h);
                }
            }
        }
       

}