using Render;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ElectricData;

public static class Program
{
    public static float screenWidthGlobal = 2000;
    public static float screenHeightGlobal = 1000;
    static void Main ()
    {
        ObjectList objects = new ObjectList();
        ConnectionList connections = new ConnectionList();
        objects.addObject(0, 100,100);
        objects.addObject(0, 5,50);
        objects.addObject(1, 0,0);
        objects.addObject(1, 10,30);
        TextureTest window = new TextureTest("Papierosik",(int)screenWidthGlobal , (int)screenHeightGlobal, objects,connections);
        window.uruchom();

    }
    static public void changeScreenSize (int x, int y)
    {
        screenHeightGlobal = y;
        screenWidthGlobal = x;
    }
    
}

