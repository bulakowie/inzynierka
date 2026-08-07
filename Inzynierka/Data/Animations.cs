using System.Data;
using OpenTK.Mathematics;
using static Program;

namespace ElectricData
{
    public class AnimationList
    {
         public AnimationList instance;
         int faceState = -1;
         int globalCoutner = 0;
        List<Animation> listaAnimacji;
        public AnimationList()
        {
            listaAnimacji = new List<Animation>();
            listaAnimacji.Add(new ProperFaceElement());
        }
        public AnimationList getInstance()
        {
            if (instance == null)
            {
                instance = new AnimationList();
                return instance;
            }
            else return instance;
        }
        
   public float[] renderVertices(ObjectList obj)
        {
            float problem;
            float[] vertices = new float[(listaAnimacji.Count()) * 24];
            float screenHeight = screenHeightGlobal;
            float screenWidth = screenWidthGlobal;
            float box_size = 0.1f;
            float proportion = screenWidthGlobal / screenHeightGlobal;
            Tuple <float,float> coords1 = new Tuple<float, float>(1f,1f);
            Tuple <float,float> coords2 = new Tuple<float, float>(1f,1f);
 float textureX = 0.0f, textureY = 0.0f;
            float textureId = 3;

            if(ConnectionList.isTheWireCorrect) faceState = 1;
            else faceState = -1 ;
            for (int i = 0; i < listaAnimacji.Count(); i++)
            {
                switch(listaAnimacji[i].animationId)
                {
                    case 0:

                    InsertAnimation? a = listaAnimacji[i] as InsertAnimation;
                    if (!a.isLeft)
                    {
                        coords1= obj.returnObjectCoords(a.objectId);

                        coords1 = new Tuple <float,float>(coords1.Item1 -(screenWidth / 2), coords1.Item2 - (screenHeight /2));
                        coords1 = new Tuple <float,float>(coords1.Item1 /(screenWidth / 2), coords1.Item2 / (screenHeight /2));
                         coords1 = new Tuple <float,float>(coords1.Item1  + 2*box_size/proportion , coords1.Item2);
                        coords2 =new Tuple<float, float>(coords1.Item1 + 4*box_size /proportion,coords1.Item2  + 4*box_size /proportion );
                        
                    }
                    else
                    {
                        coords2= obj.returnObjectCoords(a.objectId);

                        coords2 = new Tuple <float,float>(coords2.Item1 -(screenWidth / 2), coords2.Item2 - (screenHeight /2));
                        coords2 = new Tuple <float,float>(coords2.Item1 /(screenWidth / 2), coords2.Item2 / (screenHeight /2));
                        coords1 =new Tuple<float, float>(coords2.Item1 ,coords2.Item2 + 4* box_size/proportion);
                        coords2 = new Tuple <float,float>(coords2.Item1 - 4*box_size / proportion, coords2.Item2);

                    }
                    
                    box_size = 0.2f;
                    if (a.currentFrame>= a.animationFrames.Count)  a.currentFrame = 0;
                    textureId = a.animationFrames[a.currentFrame];
                    problem = textureId%100;
                    textureId = (textureId - problem) / 100;
               
                textureY = (problem - problem%10)/100;
                
                textureX = (problem%10)/10;
                    listaAnimacji[i].currentFrame++;


                    break;
                    case 1:
                    ProperFaceElement b = listaAnimacji[i] as ProperFaceElement;
                    coords1= new Tuple<float,float>(1000f,10f);

                        coords1 = new Tuple <float,float>(coords1.Item1 -(screenWidth / 2), coords1.Item2 - (screenHeight /2));
                        coords1 = new Tuple <float,float>(coords1.Item1 /(screenWidth / 2), coords1.Item2 / (screenHeight /2));
                         coords1 = new Tuple <float,float>(coords1.Item1, coords1.Item2 );
                        coords2 =new Tuple<float, float>(coords1.Item1 + 20*box_size /proportion,coords1.Item2  + 30*box_size /proportion );
                        tick(i);
                        if (faceState == -1) 
                        {
                            textureId = b.animationFrames[b.currentFrame];

                        }
                        else
                        {
                            if (b.currentFrame>2) b.currentFrame = 0;
                            textureId = b.animationFrames2[b.currentFrame];
                        } 
                        
                    problem = textureId%100;
                    textureId = (textureId - problem) / 100;
               
                textureY = (problem - problem%10)/100;
                
                textureX = (problem%10)/10;
                    break;
                }

              


               // if (listaAnimacji[i].isFirstObjectLeft) coords1 = new Tuple <float,float>(coords1.Item1 - 0.5f * box_size, coords1.Item2 + box_size/(2*proportion));
               // else coords1 = new Tuple <float,float>(coords1.Item1 +  1.5f*box_size/proportion , coords1.Item2 + box_size/(2*proportion) );

               // if (listaAnimacji[i].isSecondObjectLeft) coords2  = new Tuple <float,float>(coords2.Item1 - 0.5f * box_size, coords2.Item2 + box_size/(2*proportion));
               // else coords2  = new Tuple <float,float>(coords2.Item1  + 1.5f*box_size/proportion, coords2.Item2  + box_size/(2*proportion));

                vertices[24 * i] = coords2.Item1 ;
                vertices[24 * i + 1] = coords2.Item2 ;        
                vertices[24 * i + 2] = 0;
                vertices[24 * i + 3] = textureX + 0.1f;
                vertices[24 * i + 4] = textureY + 0.1f;
                vertices[24 * i + 5] = 0;

                vertices[24 * i + 6] = coords2.Item1;
                vertices[24 * i + 7] = coords1.Item2;
                vertices[24 * i + 8] = 0;                       
                vertices[24 * i + 9] = textureX + 0.1f;
                vertices[24 * i + 10] = textureY;
                vertices[24 * i + 11] = 0;

                vertices[24 * i + 12] = coords1.Item1;
                vertices[24 * i + 13] = coords1.Item2;
                vertices[24 * i + 14] = 0;
                vertices[24 * i + 15] = textureX;
                vertices[24 * i + 16] = textureY;
                vertices[24 * i + 17] = 0;

                vertices[24 * i + 18] = coords1.Item1;
                vertices[24 * i + 19] = coords2.Item2;
                vertices[24 * i + 20] = 0;
                vertices[24 * i + 21] = textureX;
                vertices[24 * i + 22] = textureY + 0.1f;
                vertices[24 * i + 23] = 0;
            }
            return vertices;

        }
   
   public void addInsertAnimation (int objectId, bool isLeft)
        {
            Console.WriteLine("dodano animacje do" + objectId + "o tagu" + isLeft);
        listaAnimacji.Add(new InsertAnimation(isLeft, objectId));
        }
    
    public void killInsertAnimation (int objectId)
        {
            for (int i=0; i<listaAnimacji.Count(); i++)
            if (listaAnimacji[i].animationId == 0)
                {
                    InsertAnimation? a = listaAnimacji[i] as InsertAnimation;
                    if (a.objectId == objectId)
                    {
                        listaAnimacji.Remove(listaAnimacji[i]);
                    }
                }
        }
public void tick(int id)
        {
            ProperFaceElement? a = listaAnimacji[id] as ProperFaceElement;

            if (globalCoutner == 2000)
            {
                a.currentFrame++;
                if (a.currentFrame == a.animationFrames.Count && faceState == -1)
                {
                    a.currentFrame = 0;
                } 
                else if (a.currentFrame == a.animationFrames2.Count && faceState == 1) a.currentFrame = 0;
                globalCoutner = 0;
            }
            else
                globalCoutner++;

        }
  
    } 
}