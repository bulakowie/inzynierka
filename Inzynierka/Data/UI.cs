using System.Data;
using OpenTK.Graphics.ES20;
using OpenTK.Mathematics;
using static Program;

namespace ElectricData
{
    public class UIList
    {
        List<UIElement> listaElementow;
        public UIList instance;
        public UIList getInstance()
        {
            if (instance == null)
            {
                instance = new UIList();
                return instance;
            }
            else return instance;
        }
        float top = 1.0f;
        float bottom = 0.8f;
        float left = -1.0f;
        float right = 1.0f;
        public UIList()
        {
            listaElementow = new List<UIElement>();
            listaElementow.Add(new ButtonCreateElement(0,new List<int>{73,74,75,76}));
            listaElementow.Add(new ButtonCreateElement(1,new List<int>{77,78,79,60,61,62,63,64,65}));
            listaElementow.Add(new ButtonCreateElement(2,new List<int>{81,82,83,84,85,86,87,88,89,70,71,72}));
            listaElementow.Add(new ButtonCreateElement(3,new List<int>{95}));
            listaElementow.Add(new ButtonCreateElement(4,new List<int>{99}));
            listaElementow.Add(new ButtonCreateElement(5,new List<int>{91}));
            listaElementow.Add(new ButtonCreateElement(6,new List<int>{93}));


        }

        public float[] renderVertices()
        {
            float[] vertices = new float[(listaElementow.Count() + 1) * 24];
            float screenHeight = screenHeightGlobal;
            float
             screenWidth = screenWidthGlobal;
            float proportion = screenWidthGlobal / screenHeightGlobal;
            float x_coords;
            float y_coords;
            float box_size = 0.2f;
            float textureId;
            float textureX = 0f, textureY = 0.9f;



            vertices[0] = right;
            vertices[1] = top;
            vertices[2] = 0;
                vertices[3] = textureX + 0.1f;
                vertices [4] = textureY + 0.1f;
            vertices[5] = 0;

            vertices[6] = right;
            vertices[7] = bottom;
            vertices[8] = 0;
                vertices[9] = textureX + 0.1f;
                vertices[10] = textureY;
            vertices[11] = 0;

            vertices[12] = left;
            vertices[13] = bottom;
            vertices[14] = 0;
                vertices[15] = textureX;
                vertices[16] = textureY;
            vertices[17] = 0;

            vertices[18] = left;
            vertices[19] = top;
            vertices[20] = 0;
                vertices[21] = textureX;
                vertices[22] = textureY + 0.1f;
            vertices[23] = 0;


            for (int i = 1; i < listaElementow.Count() + 1; i++)
            {
                int j = i - 1;
                textureId = listaElementow[j].imageLink;
                 float problem = textureId%100;
                textureId = (textureId - problem) / 100;
                textureY = (problem - problem%10)/100;
                
                textureX = (problem%10)/10;

                x_coords = 0.2f;
                y_coords = 0.2f;
                x_coords -= (screenWidth / 2);
                y_coords -= (screenHeight / 2);

                x_coords /= (screenWidth / 2);
                y_coords /= (screenHeight / 2);

                vertices[24 * i] = left + box_size * j + box_size;
                vertices[24 * i + 1] = top;
                vertices[24 * i + 2] = 0;
                vertices[24 * i + 3] = textureX + 0.1f;
                vertices[24 * i + 4] = textureY + 0.1f;
                vertices[24 * i + 5] = textureId;

                vertices[24 * i + 6] = left + box_size * j + box_size;
                vertices[24 * i + 7] = bottom;
                vertices[24 * i + 8] = 0;
                vertices[24 * i + 9] = textureX + 0.1f;
                vertices[24 * i + 10] = textureY;
                vertices[24 * i + 11] = textureId;

                vertices[24 * i + 12] = left + box_size * j;
                vertices[24 * i + 13] = bottom;
                vertices[24 * i + 14] = 0;
                vertices[24 * i + 15] = textureX;
                vertices[24 * i + 16] = textureY;
                vertices[24 * i + 17] = textureId;

                vertices[24 * i + 18] = left + box_size * j;
                vertices[24 * i + 19] = top;
                vertices[24 * i + 20] = 0;
                vertices[24 * i + 21] = textureX;
                vertices[24 * i + 22] = textureY + 0.1f;
                vertices[24 * i + 23] = textureId;
            }
            return vertices;

        }
        public int isAbove(float mouseX, float mouseY)
        {
            mouseY = screenHeightGlobal - mouseY;
            if (mouseY < top * screenHeightGlobal && mouseY > bottom * screenHeightGlobal)
            {
                //Console.WriteLine(mouseY);

                if (mouseX < 0.1f * screenWidthGlobal)
                {
                    tick(0);
                    return 0;
                }
                else if (mouseX < 0.2f * screenWidthGlobal)
                {
                    tick(1);
                    return 1;
                }
                else if (mouseX < 0.3f * screenWidthGlobal)
                {
                    tick(2);
                    return 4;
                }
                // else Console.WriteLine(mouseX + "+" + (left +  0.2f) *screenWidthGlobal);
            }

            return -1;
        }
        int globalCoutner = 0;
        public void tick(int id)
        {
            ButtonCreateElement? a = listaElementow[id] as ButtonCreateElement;

            if (globalCoutner == 2000)
            {
                a.iterator++;
                if (a.iterator == a.frames.Count) a.iterator = 0;
               a.imageLink = a.frames[a.iterator];
                globalCoutner = 0;
            }
            else
                globalCoutner++;

        }
        public void spawnObject()
        {

        }

    }

}