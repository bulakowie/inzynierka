using System.Drawing;
using OpenTK.Mathematics;

namespace ElectricData
{

    public class Wire
    {
        public int firstObject {get;set;}
        public int secondObject {get;set;}

        public Wire (int x, int y)
        {
            this.firstObject = x; this.secondObject = y;
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

    public class Header : ElectricObject
    {
        public Header (float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}

