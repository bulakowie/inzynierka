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


public static class TextManager
{

    public static List<Tuple<float, float>> LetterToSpriteCoords(string a)
    {
         List<Tuple<float, float>> piotrus = new List<Tuple<float, float>>();
        for (int i =0; i<a.Length; i++)
        {
            float letterId = (float)a[i];
        float posx = letterId %16;
        float posy = (letterId - posx) /16  +1;
        float x = posx / 16;
        float y = posy / 16; 
        y = 1 - y;
        piotrus.Add(new Tuple<float,float>(x,y));
        }
        
        return piotrus;
    }
    public static float[] renderVertices( List<Tuple<float, float>> piotrus)
        {
            float[] vertices = new float[piotrus.Count * 24];
            float screenHeight = screenHeightGlobal;
            float
             screenWidth = screenWidthGlobal;
            float x_coords;
            float y_coords;
           
            float textureId;

  for (int i=0; i <piotrus.Count; i ++)
        {
                

                textureId = 1;
                float textureX, textureY;
                float line = 2 * (i - (i%16))/ 16;
                x_coords = 100;
                y_coords = 800;
                x_coords -= (screenWidth / 2);
                y_coords -= (screenHeight / 2);
                float box_size = 0.2f;
                float proportion = screenWidthGlobal / screenHeightGlobal;

                x_coords /= (screenWidth / 2);
                y_coords /= (screenHeight / 2);
textureX = piotrus[i].Item1;
textureY = piotrus[i].Item2;

                vertices[  24 * i  ] = x_coords + box_size / proportion + i%16 *box_size/ proportion;
                vertices[  24 * i   + 1] = y_coords + box_size - line * box_size / proportion;
                vertices[  24 * i   + 2] = 0;
                vertices[  24 * i   + 3] = textureX + 1/16f;
                vertices[  24 * i   + 4] = textureY + 1/16f;
                vertices[  24 * i   + 5] = textureId;

                vertices[  24 * i   + 6] = x_coords + box_size / proportion + i%16 *box_size/ proportion ;
                vertices[  24 * i   + 7] = y_coords - line * box_size / proportion;
                vertices[  24 * i   + 8] =  0;
                vertices[  24 * i   + 9] = textureX +  1/16f;
                vertices[  24 * i   + 10] = textureY;
                vertices[  24 * i   + 11] = textureId;

                vertices[  24 * i   + 12] = x_coords + i%16 *box_size / proportion;
                vertices[  24 * i   + 13] = y_coords - line * box_size / proportion;
                vertices[  24 * i   + 14] =  0;
                vertices[  24 * i   + 15] = textureX;
                vertices[  24 * i   + 16] = textureY;
                vertices[  24 * i   + 17] = textureId;

                vertices[  24 * i   + 18] = x_coords + i%16 *box_size / proportion ;
                vertices[  24 * i   + 19] = y_coords + box_size - line * box_size / proportion;
                vertices[  24 * i   + 20] =  0;
                vertices[  24 * i   + 21] = textureX;
                vertices[  24 * i   + 22] = textureY +  1/16f;
                vertices[  24 * i   + 23] = textureId;
        }                return vertices;

            }
           

        
}
