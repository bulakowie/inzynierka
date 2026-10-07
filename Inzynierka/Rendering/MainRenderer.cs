using System;
using ElectricData;
using OpenTK.Windowing.GraphicsLibraryFramework;


namespace Render
{

    public class MainRenderer
    {
        public ConnectionList _connectionList;
        public ObjectList _objectList;

        public float[] renderVertices (RenderDataCollector dataCollector)
        {
             List<RenderData> renderDatas = dataCollector.giveRenderData();
            float[] vertices = new float[(renderDatas.Count()) * 24];
            for (int i=0; i<renderDatas.Count(); i++)
            {
                RenderData a = renderDatas[i];

               vertices[24 * i] = a.cord_x2;
                vertices[24 * i + 1] = a.cord_y2;
                vertices[24 * i + 2] = 0;
                vertices[24 * i + 3] = a.img_x2;
                vertices[24 * i + 4] = a.img_y2;
                vertices[24 * i + 5] = a.img_ID;

                vertices[24 * i + 6] = a.cord_x2;
                vertices[24 * i + 7] = a.cord_y1;
                vertices[24 * i + 8] = 0;
                vertices[24 * i + 9] = a.img_x2;
                vertices[24 * i + 10] = a.img_y1;
                vertices[24 * i + 11] = a.img_ID;

                vertices[24 * i + 12] = a.cord_x1;
                vertices[24 * i + 13] = a.cord_y1;
                vertices[24 * i + 14] = 0;
                vertices[24 * i + 15] = a.img_x1;
                vertices[24 * i + 16] = a.img_y1;
                vertices[24 * i + 17] = a.img_ID;

                vertices[24 * i + 18] = a.cord_x1;
                vertices[24 * i + 19] = a.cord_y2;
                vertices[24 * i + 20] = 0;
                vertices[24 * i + 21] = a.img_x1;
                vertices[24 * i + 22] =  a.img_y2;
                vertices[24 * i + 23] = a.img_ID; 
            }
                
            return vertices;
        }
        public uint[] renderIndices(RenderDataCollector dataCollector)
        {
             List<RenderData> renderDatas = dataCollector.giveRenderData();

            uint[] indices = new uint[renderDatas.Count() * 6];
            for (int i = 0; i < renderDatas.Count(); i++)
            {
                indices[i * 6] = (uint)(int)i * 4;
                indices[i * 6 + 1] = (uint)(int)i * 4 + 1;
                indices[i * 6 + 2] = (uint)(int)i * 4 + 3;
                indices[i * 6 + 3] = (uint)(int)i * 4 + 1;
                indices[i * 6 + 4] = (uint)(int)i * 4 + 2;
                indices[i * 6 + 5] = (uint)(int)i * 4 + 3;
            }
            return indices;
        }
    }
}