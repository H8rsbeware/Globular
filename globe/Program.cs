using System;
using Globular.Transformers;
using Globular.Frame;
using Globular.TempPath;

namespace Globular
{
    class Program
    {
        static Temp temp = new Temp();
        static void Main(string[] args)
        {
            temp.SaveInitialBackground();

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            FrameGenerator frameGenerator = new FrameGenerator(temp);
            BackgroundSetter backgroundSetter = new BackgroundSetter();

            Frame.Frame f = frameGenerator.GenerateBlankFrame(1920, 1080);
            backgroundSetter.SetBackground(f);

            System.Threading.Thread.Sleep(500);

        }
        static void OnProcessExit(object? sender, EventArgs e)
        {
            temp.ReplaceBackground();
            temp.Clear();
        }
    }
}