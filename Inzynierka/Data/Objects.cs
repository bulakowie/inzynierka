using System.Diagnostics;
using System.Drawing;
using System.Security.Authentication;
using OpenTK.Mathematics;

namespace ElectricData
{
    public abstract class UIElement
    {
        public int elementID {get; set;}
        public List<int> frames {get; set;}
        public int imageLink {get;set;}
        public int iterator {get;set;}

    }
    public class ButtonCreateElement : UIElement
    {
        public int whatElement {get;set;}
        public int startingImage {get; set;}
        public ButtonCreateElement (int a, List<int> l)
        {
           
            elementID = 0;
            whatElement = a;
            frames = l;
            imageLink = frames[0];
            iterator = 0;
        }
    }
    

    public abstract class Animation
    {
        public int animationId;
        public List<int> animationFrames {get;set;}
        public int currentFrame {get;set;}
        public bool isRepeat {get;set;}
    }

    public class InsertAnimation : Animation
    {
        public bool isLeft {get;set;}
        public int objectId {get;set;}
        public InsertAnimation (bool isLeft, int objectId)
        {
            animationId = 0;
            animationFrames =  new List<int>{66,67,68,69,50,51};
            isRepeat = false;
            currentFrame = 0;
            this.isLeft = isLeft;
            this.objectId = objectId;
        }
    }
    public class ProperFaceElement : Animation
    {
        public List<int> animationFrames2;

        public ProperFaceElement ()
        {
            animationId = 1;
            animationFrames2 = new List<int>{52,53,54};
            animationFrames = new List<int>{55,56,57,58,59,40,41,42};
        }
    }
    public class ButtonAnimation : Animation
    {
        public int buttonId {get;set;}
        public ButtonAnimation (bool isLeft, int buttonId, List<int> l)
        {
            animationId = 1;
            animationFrames =  new List<int>{4,5,6,7,8,9};
            isRepeat = true;
            currentFrame = 0;
            this.buttonId = buttonId;
        }
    }
    

    public class Wire
    {
        public int firstObject {get;set;}
        public bool isFirstObjectLeft {get;set;}
        public int secondObject {get;set;}

        public bool isSecondObjectLeft {get;set;}

        public Wire (int x, int y, bool a, bool b)
        {
            this.firstObject = x; this.secondObject = y;
            this.isFirstObjectLeft = a; this.isSecondObjectLeft = b;
        }
    }

    public abstract class ElectricObject
    {
        public int objectID {get;set;}
        public float X { get; set; }
        public float Y { get; set; }

        public float imageLink{ get; set; }
    }

    public class Resistor : ElectricObject
    {
        public double resistance { get; set; }

        public Resistor (float x, float y)
        {
            X = x;
            Y = y;
            imageLink = 97;
        }
    }
    public class Cell : ElectricObject
    {
        public double voltage { get; set; }
        public Cell (float x, float y)
        {
            X = x;
            Y = y;
            imageLink = 92;
        }
    }
     public class ACSource : ElectricObject
    {
        public double voltage { get; set; }
        public ACSource (float x, float y)
        {
            X = x;
            Y = y;
            imageLink = 94;
        }
    }
    public class Node : ElectricObject
    {
        public Node (float x, float y)
        {
            X = x;
            Y = y;
            imageLink = 96;
        }
    }

    public class Header : ElectricObject
    {
        public Header (float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}

