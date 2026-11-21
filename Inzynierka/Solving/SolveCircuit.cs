using System.ComponentModel;
using System.Data.SqlTypes;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Windows.Markup;
using ElectricData;
using OpenTK.Graphics.ES20;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input.Hid;
using OpenTK.Mathematics;
using static Program;

namespace Calculations
{
    public struct Equation
    {
        public List<string> values;
    }
    public struct CurrentRelations
    {
        public List<Tuple<double,string>> values;
        public HashSet<string> containedKeys;
    }

    public class finalEquationSolver
    {
        public Dictionary<string, List<Equation>> Encyclopedia = new Dictionary<string, List<Equation>>();

        public List<CurrentRelations> currentRelations1 = new List<CurrentRelations>();
    
        public Dictionary<string, double> ActualValues = new Dictionary<string, double>();

        public Dictionary<string,bool> isKnown = new Dictionary<string, bool>();
        int REVERSE(int a)
        {
         return -a -1;
        }

        public void useOhmToEyes()
        {
            string g = "V";
            int constVcount = 0;
            while (true)
            {
                g = "V";
                g = "+" + "V" +constVcount.ToString();
                if (!Encyclopedia.ContainsKey(g)) break;
                else
                {
                    foreach (Equation z in Encyclopedia[g])
                    {
                        if (z.values[0][1]!='V') continue;
                        //Znajdujemy w encyklopedii wzory na bazie V = V + V + V i przemieniamy je na wzory V = IR + IR + IR 
                        //jesli R jest znane od razu na je miejsce wstawimy liczbe i dostaniemy uklad
                        //Opcje są dwie albo znamy V te poczatkowe i przez to mamy cos w stylu 26 = 4I + 2I + 4I
                        //Mysle ze to dobry pierwszy krok do rozwiazywania kazdego rownania bo pozwala na stowrzenie jakiegos stosunku pomiedzy napięciami
                        //Ale najpierw musze upenić się że oczka działają prawidłowo
                        //potrzebne są wzory oczkowe DOBRA WZORY OCZKOWE ZROBIONE LECVIMY DALEJ
                        else
                        {
                            CurrentRelations currentRelations = new CurrentRelations();
                            currentRelations.values = new List<Tuple<double, string>>();
                            currentRelations.containedKeys = new HashSet<string>();
                            string v_string = getKeyFromEquation(g);
                            string nm = getNumberFromKey(v_string);
                            //mamy V, jakie mamy przypadki?
                            //mozemy znac V, wtedy podstawiamy po prostu liuczbe
                            //mozemy znac R, wtedy podstawiamy pod R liczbe i zostaje 12 I3 narzyklad
                            //mozemy znac I, to wtedy na tej samej zasadzie
                            //po prostu teraz zamiast +V4 = +R4*+4I 
                            //robimy pary : liczba + klucz
                            string r_string = "+R" + nm;
                            string i_string = "+I" + nm;
                            if (isKnown[v_string])
                            {
                                double value = ActualValues[v_string];
                                Tuple<double,string> v = new Tuple<double, string>(-value, "1");
                                currentRelations.values.Add(v);
                            }
                            else if (isKnown[r_string] || isKnown[i_string])
                            {
                                double value;
                                string s;
                               if (isKnown[r_string])
                               {
                                value = ActualValues[r_string];
                                s = "+I" + getNumberFromKey(r_string);
                               }
                               else 
                               {
                                value =ActualValues[i_string];
                                s = "+R" + getNumberFromKey(i_string);
                               }
                               Tuple<double,string> v = new Tuple<double, string>(-value, s);
                                currentRelations.values.Add(v);
                                currentRelations.containedKeys.Add(s);
                            }
                            for (int i=0; i<z.values.Count; i++)
                            {
                                v_string = getKeyFromEquation(z.values[i]);
                                nm = getNumberFromKey(v_string);
                                bool isReverse =false;
                                if (z.values[i][0] == '-') isReverse = true; 
                                else isReverse = false;
                                r_string = "+R" + nm;
                                i_string = "+I" + nm;
                                if (isKnown[v_string])
                                {
                                    double value = ActualValues[v_string];
                                    if (isReverse) value *=-1;
                                    Tuple<double,string> v = new Tuple<double, string>(value, "1");

                                    currentRelations.values.Add(v);
                                }
                                else if (isKnown[r_string] || isKnown[i_string])
                                {
                                double value;
                                string s;
                               if (isKnown[r_string])
                               {
                                value = ActualValues[r_string];
                                if (isReverse)
                                value *=-1;
                                s = "+I" + getNumberFromKey(r_string);
                               }
                               else 
                               {
                                value =ActualValues[i_string];
                                if (isReverse)
                                value *=-1;
                                s = "+R" + getNumberFromKey(i_string);
                               }
                               Tuple<double,string> v = new Tuple<double, string>(value, s);
                                currentRelations.values.Add(v);
                                currentRelations.containedKeys.Add(s);
                                }
                            }
                           currentRelations1.Add(currentRelations);  
                        }
                       
                    }
                }
                constVcount++;
            }
        }
        
        public void addCurrentRelations()
        {
            string g = "I";
            int constIcount = 0;
            while (true)
            {
                g = "I";
                g = "+" + "I" +constIcount.ToString();
                if (!Encyclopedia.ContainsKey(g)) break;
                else
                {
                    foreach (Equation z in Encyclopedia[g])
                    {
                        if (z.values[0][1]!='I') continue;
                        else
                        {
                            CurrentRelations currentRelations = new CurrentRelations();
                            currentRelations.values = new List<Tuple<double, string>>();
                            currentRelations.containedKeys = new HashSet<string>();
                            string i_string = getKeyFromEquation(g);
                            string nm = getNumberFromKey(i_string);
                            //mamy V, jakie mamy przypadki?
                            //mozemy znac V, wtedy podstawiamy po prostu liuczbe
                            //mozemy znac R, wtedy podstawiamy pod R liczbe i zostaje 12 I3 narzyklad
                            //mozemy znac I, to wtedy na tej samej zasadzie
                            //po prostu teraz zamiast +V4 = +R4*+4I 
                            //robimy pary : liczba + klucz
                                Tuple<double,string> v = new Tuple<double, string>(-1, i_string);
                                currentRelations.values.Add(v);
                                currentRelations.containedKeys.Add(i_string);
                            
                            for (int i=0; i<z.values.Count; i++)
                            {
                                i_string = getKeyFromEquation(z.values[i]);
                                nm = getNumberFromKey(i_string);
                                bool isReverse =false;
                                if (z.values[i][0] == '-') isReverse = true; 
                                else isReverse = false;
                                double value = 1;
                                if (isReverse) value *=-1;
                                v = new Tuple<double, string>(value, i_string);

                               currentRelations.values.Add(v);
                               currentRelations.containedKeys.Add(i_string);
                            }
                               currentRelations1.Add(currentRelations);  
                            }
                        }
                    }
                    constIcount++;
                }
        }
        
        public void removeRepeatsFromCurrentRelations()
        {
            
        }
        public void getEquations(Hierophant h)
        {
            PrepareEquations a = new PrepareEquations();
            a.getEquations(h,ref Encyclopedia, ref ActualValues,ref  isKnown);
        }

        public void Loop (Hierophant h)
        {
            useOhmToEyes();
            addCurrentRelations();
            removeRepeatsFromCurrentRelations();
        }
        public bool ALGEBRAcanBeCalculated(string s)
        {
            if (isKnown[s]) return false; //nie ma co liczyc jak juz znamy wartosc
            List< Equation> z = Encyclopedia[s]; //wszystkie wzory danego elementu
            for (int j =0; j < z.Count(); j++)
            {
                foreach (Equation e in z)
                {
                    if (ALGEBRAcheckEquationSolvable(e))
                    {
                        isKnown[s] = true;
                        ActualValues[s] = ALGEBRAEquationRealNumbersSolver(e,s);
                        return true;
                    }
                }
            }
            
            return false;
        }
        public bool ALGEBRAcheckEquationSolvable(Equation e)
        {
            for (int i=0; i<e.values.Count(); i++) 
            {
                if (getKeyFromEquation(e,i) == "ignore") continue;
                if (!isKnown[getKeyFromEquation(e,i)]) return false;
            }

            return true;
        }
        public bool ALGEBRAreplacement (string s) //dostajemy klucz do ktorego bedziemy probowac przypisac nowe wartosci? wzory! ty chamie!
        {
            List< Equation> z = Encyclopedia[s]; //wszystkie wzory danego elementu
            for (int j =0; j < z.Count(); j++) //idziemy po wszystkich wzorach danego elementu
            {
                foreach (Equation e in z) //szukamy takich ktore zmnjiejsza lub zostawia liczbe nieznanych zmiennych taka sama
                {
                        HashSet<string> AddedEquationSymbols = new HashSet<string>();

                    for (int i=0; i<e.values.Count(); i++) //lecimy zliczamy wartosc
                    {
                        AddedEquationSymbols.Add(getKeyFromEquation(e,i));
                    } 
                    for (int i=0; i<e.values.Count(); i++) //jak juz zobaczylismny jakie wartosci sa w naszym tym wszystkim trzeba teraz przyrownac inne wzory
                    {
                        string changingStuff = getKeyFromEquation(e,i); //oto wartosc ktora podmieniamy
                        if (changingStuff == "ignore") continue;
                        foreach (Equation piep in Encyclopedia[changingStuff])
                        {
                            int liczbaNowychZmiennych = 0;
                            for (int h=0; h<piep.values.Count(); h++) //lecimy zliczamy wartosc
                            {
                                 if(!AddedEquationSymbols.Contains(getKeyFromEquation(piep,h)))
                                {
                                    if (getKeyFromEquation(piep,h) == "ignore") continue;
                                    if (!isKnown[getKeyFromEquation(piep,h)])
                                    liczbaNowychZmiennych++;
                                    if (liczbaNowychZmiennych == 2) break;
                                }
                                if (getKeyFromEquation(piep,h) == s)
                                {
                                    liczbaNowychZmiennych = 10;
                                }
                            }
                            if (liczbaNowychZmiennych < 2)
                            {
                                Equation piotr = ALGEBRAreplaceInEquation(s, changingStuff,e,piep);
                                piotr = ALGEBRAsimplifyEquation(piotr);
                                if (ALGEBRAcheckForRepeats(s,piotr))
                                {
                                    Encyclopedia[s].Add(piotr);
                                    return true;
                                }
                            }
                        }
                    }
                    

                }
            }
            return false;
        }
        public bool ALGEBRAcheckForRepeats(string s, Equation piotr)
        {
            HashSet<string> symbols = new HashSet<string>();
            for (int i=0; i<piotr.values.Count; i++)
            {
                if (piotr.values[i]!="(" && piotr.values[i]!=")")
                symbols.Add(piotr.values[i]);
            }
            foreach  (Equation e in Encyclopedia[s])
            {
                int guard = 0;
                for (int i=0; i<e.values.Count(); i++)
                {
                    if (!symbols.Contains(e.values[i]) && e.values[i]!="(" && e.values[i]!=")")
                    {
                        guard = 1;
                    }
                }
                if (guard == 0) return false;
            }
            return true;
        }
        public Equation ALGEBRAsimplifyEquation (Equation piotr)
        {
            Equation piep = new Equation();
            //usuwanie nawiasow te sprawy. Na razie nie dotykam. Pozniej sie tym zajme
            return piotr;
        }
        public Equation ALGEBRAreplaceInEquation(string original, string replacedVariable, Equation originalEq, Equation replacedEquation)
        {
            Equation Frank = new Equation();
            Frank.values = new List<string>();
            for (int i=0; i<originalEq.values.Count(); i++)
            {
                if (getKeyFromEquation(originalEq,i)==replacedVariable)
                {
                    
                    string add = "";
                    Frank.values.Add(originalEq.values[i][0] + "(");
                    for(int j=0; j<replacedEquation.values.Count; j++)
                    {
                        Frank.values.Add(replacedEquation.values[j]);
                    }
                    Frank.values.Add(")");
                }
                else
                {
                    Frank.values.Add(originalEq.values[i]);
                }
            }
            return Frank;
        }
        public string getKeyFromEquation(Equation e, int i)
        {
            string g = e.values[i];
            if ( g[0] == ')') return "ignore";
            if (g[1] == '(') return "ignore";
            if (g[0]!= '+')
            {
                string h = "+";
                for (int j = 0; j<g.Count(); j++)
                    if (g[j]>='A' && g[j]<='Z')
                    {
                        h+=g[j];
                        h+=g[j+1];
                    }
                g = h;
            }
            
            return g;
        }
        public string getNumberFromKey(string s)
        {
            string a = s;
            string ret = "";
            for (int i=0; i<a.Count(); i++)
            {
                if (a[i]>='0' && a[i]<='9') ret += a[i];
            }
            return ret;
        }
        public string getKeyFromEquation(string s)
        {
            string g = s;
            if ( g[0] == ')') return "ignore";
            if (g[1] == '(') return "ignore";
            if (g[0]!= '+')
            {
                string h = "+";
                for (int j = 0; j<g.Count(); j++)
                    if (g[j]>='A' && g[j]<='Z')
                    {
                        h+=g[j];
                        h+=g[j+1];
                    }
                g = h;
            }
            
            return g;
        }
        public double ALGEBRAEquationRealNumbersSolver(Equation e, string s)
        {
            double a = 0;
            int weryfikacjaOhma = 0;
            for (int i=0; i<e.values.Count(); i++)
            {
                string g = e.values[i];
                string key = getKeyFromEquation(e, i);
                switch (g[0])
                {
                    case '+':
                    a += ActualValues[key];
                    break;

                    case '-':
                    a -= ActualValues[key];
                    break;

                    case '/':
                    if (g[1] == '+') a /= ActualValues[key];
                    else a /= - ActualValues[key];
                    break;

                    case '*':
                    if (g[1] == '+') a *= ActualValues[key];
                    else a *= -ActualValues[key];
                    break;
                }
                if ((g[1]=='V' || g[2]=='V') && ActualValues[key] < 0)
                {
                    weryfikacjaOhma = 1;
                }
            }
            if (weryfikacjaOhma >= 1)
            {
                if (a < 0 ) a *= -1;
            }
            return a;

        }
    
    }
}