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
        ObjectList objects =  ObjectList.getInstance();
        ConnectionList connections = new ConnectionList();
        AnimationList animations = new AnimationList();
        UIList uis = new UIList();
        objects.addObject(0, 100,100);
        objects.addObject(0, 5,50);
        objects.addObject(1, 200,100);
        TextureTest window = new TextureTest("Papierosik",(int)screenWidthGlobal , (int)screenHeightGlobal, objects,connections,animations, uis);
        window.uruchom();

    }
    static public void changeScreenSize (int x, int y)
    {
        screenHeightGlobal = y;
        screenWidthGlobal = x;
    }
    
}

