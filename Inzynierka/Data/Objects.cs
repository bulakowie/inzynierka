using System.Diagnostics;
using System.Drawing;
using System.Security.Authentication;
using OpenTK.Mathematics;

namespace ElectricData
{
    public abstract class UIElement
    {
        public int elementID {get; set;}
        public int imageLink {get; set;}
    }
    public class ButtonCreateElement : UIElement
    {
        public int whatElement {get;set;}
        public int tickLimit {get; set;}
        public int startingImage {get; set;}
        public ButtonCreateElement (int a, int image, int frame)
        {
            elementID = 0;
            whatElement = a;
            imageLink = image;
            tickLimit = frame;
            startingImage = image;
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
            animationFrames =  new List<int>{4,5,6,7,8,9};
            isRepeat = false;
            currentFrame = 0;
            this.isLeft = isLeft;
            this.objectId = objectId;
        }
    }
    public class ButtonAnimation : Animation
    {
        public int buttonId {get;set;}
        public ButtonAnimation (bool isLeft, int buttonId)
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
            imageLink = 1;
        }
    }
    public class Cell : ElectricObject
    {
        public double voltage { get; set; }
        public Cell (float x, float y)
        {
            X = x;
            Y = y;
            imageLink = 2;
        }
    }
    public class Node : ElectricObject
    {
        public Node (float x, float y)
        {
            X = x;
            Y = y;
            imageLink = 23;
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

