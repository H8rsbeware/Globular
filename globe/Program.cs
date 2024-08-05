using System;
using Globular.Transformers;
using Globular.Frame;
using Globular.TempPath;
using Globular.Transformers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

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

            FrameT f = frameGenerator.GenerateBlankFrame(1920, 1080);

            // Image<Rgba32> i = Image.Load("C:\\Users\\alive\\Code\\cs\\globular\\globe\\base.png");

            LuminancyModifier lm = new LuminancyModifier(
                [860, 540],
                [1920,1080],
                Priority.LOW,
                Image.Load<Rgba32>("C:\\Users\\alive\\Code\\cs\\globular\\globe\\lm.png")
            );

            LayerDrawModifier ld = new LayerDrawModifier(
                [860, 540],
                [100,100],
                Priority.LOW,
                Image.Load<Rgba32>("C:\\Users\\alive\\Code\\cs\\globular\\globe\\overlay.png")
            );

            LayerDrawModifier lc = new LayerDrawModifier(
                [860, 540],
                [100,100],
                Priority.HIGH,
                Image.Load<Rgba32>("C:\\Users\\alive\\Code\\cs\\globular\\globe\\overlay.png")
            );

            // i = lm.Modify(i);
            // i = ld.Modify(i);
            // f.frame = i;

            ModifierChain mc = new ModifierChain();
            mc.AddModifier(lm);
            mc.AddModifier(ld);
            mc.AddModifier(lc);

            mc.ConjurgateModifiers();

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