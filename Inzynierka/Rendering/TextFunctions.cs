using System.Xml.Serialization;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System.Transactions;
using System.Runtime.InteropServices;
using ElectricData;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using static Program;
using System.Text;
using System.Net.Mail;
using System.Globalization;


public class TextManager
{
    int blocker = 0; //blocker blokuje powtarzanie się jednej funkcji 10 klatek na sekunde
    List<Tuple<float, float>> piotrus = new List<Tuple<float, float>>();
    string g = "";
    int line_interest = 0;
    int line_numbers =0;
    int currentObject = -1;
    public List<Tuple<float, float>> LetterToSpriteCoords()
    {
        int lineC = 0;
        piotrus = new List<Tuple<float, float>>();
        for (int i =0; i<g.Length; i++)
        {
            float letterId = (float)g[i];

        float posx = letterId %16;
        float posy = (letterId - posx) /16  +1;
        float x = posx / 16;
        float y = posy / 16; 
        y = 1 - y;
       if (letterId == 10)
       {
            if (line_interest == lineC) piotrus.Add(new Tuple<float,float>(0.9375f,0.625f));
            lineC++;
            x = -1;  
       }
        piotrus.Add(new Tuple<float,float>(x,y));
        }
        
        return piotrus;
    }
    public void renderVertices(RenderDataCollector dataCollector)
        {
            LetterToSpriteCoords();
            float[] vertices = new float[piotrus.Count * 24];
            float screenHeight = screenHeightGlobal;
            float
             screenWidth = screenWidthGlobal;
            float x_coords;
            float y_coords;
           
            float textureId;
            int lineAdd = 0;
            int lineProblem =0 ;

  for (int i=0; i <piotrus.Count; i ++)
        {
                if (piotrus[i].Item1 == -1)
            {
                lineAdd++;
                lineProblem = i+1;
                continue;
            }

                textureId = 1;
                float textureX, textureY;
                
                x_coords = 100;
                y_coords = 800;
                x_coords -= (screenWidth / 2);
                y_coords -= (screenHeight / 2);
                float box_size = 0.05f;
                float proportion = screenWidthGlobal / screenHeightGlobal;

                x_coords /= (screenWidth / 2);
                y_coords /= (screenHeight / 2);
                textureX = piotrus[i].Item1;
                textureY = piotrus[i].Item2;

                int gh = i-lineProblem;
                float line = 2 * (gh - (gh%16))/ 16;
                line += lineAdd * 2;
dataCollector.addRenderData(textureId, textureX, textureY,textureX +  1/16f, textureY  +  1/16f,
                x_coords + gh%16 *box_size / proportion, y_coords - line * box_size / proportion,
                x_coords + box_size / proportion + gh%16 *box_size/ proportion, y_coords + box_size - line * box_size / proportion);
        }

    }
           
    public void writingObjectData(int ret2)
    {
        if (ret2 == currentObject) return;
        line_numbers = 0;
        if (ret2 < 0)
        {
            g = "";
            return;
        }
        ObjectList obj = ObjectList.getInstance();
        ElectricObject a = obj.returnLista()[ret2];
        
        g = "";
        g += a.objectName + '\n';
        for (int i=0; i< a.objectValues_list.Count(); i++)
        {
            g += a.objectValues_list[i].Item1.ToString()+ ":"+  a.objectValues_list[i].Item2.ToString();
            g += '\n';
            line_interest = i;
            line_numbers++;
            currentObject = ret2;
        }
        

    }
    public void receiveInputText(StringBuilder input)
    {
        if (!(input.ToString()[0] >= '0') || !(input.ToString()[0] <= '9')) return;
        int a = 0;
         for (int i=0; i< g.Length; i++)
         if (g[i] == '\n')
            {
                if (a == line_interest)
                {
                g = g.Insert(i, input.ToString());
                LetterToSpriteCoords();
                return;
                }
                else a++;
            }
        //g+= input.ToString();
    }
    public int react(int symbol, int ret2)
    {
        if (symbol == 2) //Down
        {
            if (blocker == 0) return 0;
            line_interest += 1;
            if (line_interest > line_numbers) line_interest = line_numbers;
            blocker = 0;
        }
        else if (symbol ==3) //Up
        {
            if (blocker == 0) return 0;
            line_interest -=1;
            if (line_interest <0) line_interest = 0;
            blocker = 0;
        }
        else if (symbol == 1) //zczytaj wartosci, zamknij kram.
        {
            if (blocker == 0) return 0;
             ObjectList obj = ObjectList.getInstance();
            bool count = false;
            string number = "";
            int whichValue = 0;
            for (int i=0; i< g.Length; i++)
            {
                if (g[i] == ':')
                {
                    count = true;
                }
                else if (count && g[i]!='\n')
                {
                    number+=g[i];
                }
                else if (count && g[i]=='\n')
                {
                    obj.changeValueInObject(ret2,whichValue,double.Parse(number));
                    whichValue ++;
                    number = "";
                    count = false;

                }
            }
            g = "";
            piotrus = new List<Tuple<float, float>>();
            currentObject = -1;
            blocker = 0;
            return 1;
        }
        else if (symbol ==0)
        {
            if (blocker == 0) return 0;
            int linesCounted = -1 ;
            for (int i =0; i<g.Length; i++)
            {
                if (g[i] == '\n')
                {
                    linesCounted++;
                }
                if (linesCounted == line_interest)
                {
                    char execution = g[i-1];
                    if ((g[i-1]>='0' && g[i-1]<='9') || g[i-1] == ',' || g[i-1] == '.')
                    {
                        Console.WriteLine(execution + " " + (i-1));
                        g = g.Remove(i-1,1);
                        LetterToSpriteCoords();
                        blocker = 0;
                        return 0;
                    }
                    else return 0;
                }
            }
        }
        else
        {
            blocker = 1;
        }
            return 0;

    }   
}
