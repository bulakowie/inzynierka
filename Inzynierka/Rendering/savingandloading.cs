using Render;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ElectricData;
using System.Data;
using System.Runtime.ExceptionServices;
using System.Globalization;
using System.Text;
using Microsoft.Win32;
using System.IO;
namespace ElectricData
{
    public static class SavingAndLoading
{
    public static int sorter(int l)
        {
            switch (l)
            {
                            case 7:
                            Save();
                            break;
                            case 8:
                            Load();
                            break;
                            case 9:
                            return 5;

            };
            return -1;
        }
    static void Save()
    {
        ObjectList obj = ObjectList.getInstance();
        ConnectionList con = ConnectionList.getInstance();
        string s = "pliczek.txt";
        string path = s;
        string h = "";
        h+= "OBJECTS START\n";
        for (int i =0; i< obj.getLenght(); i++)
            {
                h+= "OBJECT " + i + "\n";
                var g = obj.returnLista()[i];
                h += g.objectName + "-" + g.X + "-" + g.Y + "-";
                for (int j =0; j<g.objectValues_list.Count; j++)
                {
                    var m = g.objectValues_list[j];
                    h+= m.Item1 + "-" + m.Item2 + "-";
                }
                 h+="\n";
            }
        h+= "OBJECTS STOP\n";
        h+= "CONNECTIONS START\n";
        for (int i =0; i<con.getLenght(); i++)
            {
                h+= "CONNECTION " + i + "\n";
                var g = con.GetWires()[i];
                h += g.firstObject + "-" + g.secondObject + "-" + g.isFirstObjectLeft + "-" + g.isSecondObjectLeft + "\n";
            }
        h+= "CONNECTIONS STOP\n";
  
        string sciezka = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),path);

    File.WriteAllText(sciezka, h);
    }
    public static ElectricObject CreateObject(
    string nazwa,
    float x,
    float y,
    List<Tuple<objectValues, double>> wartosci)
    {
    switch (nazwa)
    {
        case "Resistor":
            return new Resistor(x, y, wartosci);

        case "Cell":
            return new Cell(x, y, wartosci);
        case "ACSource":
            return new ACSource(x,y,wartosci);
        default:
        return null;
    }
    }
     static void Load()
    {
         ObjectList obj = ObjectList.getInstance();
        ConnectionList con = ConnectionList.getInstance();
        string tryb = "";
        string sciezka = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
    "pliczek.txt");
        string[] linie = File.ReadAllLines(sciezka);
        ElectricObject a;
        Wire b;
         obj.clear();
         con.clear();
    foreach (string linia in linie)
    {
    if (linia == "OBJECTS START") tryb = "OBJECT";
    else if (linia == "CONNECTIONS START") tryb = "CONNECTIONS";
    string[] dane = linia.Split('-');
    switch (tryb)
    {
     case "OBJECT":
                    if (dane.Count() > 2)
                        {
                            List<Tuple<objectValues, double>> wartosci = new List<Tuple<objectValues, double>>();
                            for (int i=3;i<dane.Count()-1; i+=2)
                            wartosci.Add(new Tuple<objectValues, double>(Enum.Parse<objectValues>(dane[i]), double.Parse(dane[i+1])));
                            a = CreateObject(dane[0],float.Parse(dane[1]), float.Parse(dane[2]), wartosci);
                            obj.professionalAddObject(a);
                        }
                  break;  


                  case "CONNECTIONS":
                        {
                            if (dane.Count() > 2)
                            {
                                b = new Wire(int.Parse(dane[0]), int.Parse(dane[1]), bool.Parse(dane[2]), bool.Parse(dane[3]));
                            con.professionalAddObject(b);
                            }
                           
                        }
                  break;
    };

    }

    }
}
}
