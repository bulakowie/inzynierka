using System.ComponentModel;
using System.Data;
using OpenTK.Graphics.ES20;
using OpenTK.Mathematics;
using static Program;

namespace ElectricData
{
    public class RenderData
    {
        public float img_ID;
        public float img_x1, img_y1, img_x2, img_y2;
        public float cord_x1, cord_y1, cord_x2, cord_y2;
        public RenderData(float id,
        float img_x, float img_y,float img_x2,float img_y2,
        float cord_x1, float cord_y1,float cord_x2,float cord_y2)
        {
            img_ID = id;
            this.img_x1 = img_x;
            this.img_y1 = img_y;
            this.img_x2 = img_x2;
            this.img_y2 =img_y2;
            this.cord_x1 =cord_x1;
            this.cord_y1 = cord_y1;
            this.cord_x2 = cord_x2;
            this.cord_y2 = cord_y2;
        }
    }
    public class RenderDataCollector
    {
        List<RenderData> renderDatas;

        public RenderDataCollector()
        {
            renderDatas = new List<RenderData>();
        }

        public void addRenderData (float id,
        float img_x, float img_y,float img_x2,float img_y2,
        float cord_x1, float cord_y1,float cord_x2,float cord_y2)
        {
            renderDatas.Add(new RenderData(id, img_x,img_y,img_x2,img_y2,cord_x1,cord_y1,cord_x2,cord_y2));
        }

        public void clearRender ()
        {
            renderDatas = new List<RenderData>(); 
        }
        public List<RenderData>  giveRenderData()
        {
            return renderDatas;
        }
    }
}
