using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

using System.Runtime.InteropServices;
using Globular.TempPath;


namespace Globular
{
    public class BackgroundSetter
    {
        static Temp path = new Temp();
        static string name = "bg-image.png";

        // These import is used to set the desktop background image.
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SystemParametersInfo(UInt32 uiAction, UInt32
        uiParam, String pvParam, UInt32 fWinIni);
        private static UInt32 SPI_SETDESKWALLPAPER = 20;
        private static UInt32 SPIF_UPDATEINIFILE = 0x1;


        public void SetBackground(Image<Rgba32> image)
        {
            image.Save(path.ConcatWith(name));

            SystemParametersInfo(
                SPI_SETDESKWALLPAPER, 
                1, 
                path.ConcatWith(name), 
                SPIF_UPDATEINIFILE
            );
        }

        public void SetBackground(string file_path)
        {
            Image<Rgba32> image = Image.Load<Rgba32>(file_path);
            SetBackground(image);
        }

        public void SetBackground(Frame.Frame frame)
        {   
            Image<Rgba32> f = frame.frame;
            if (frame.ok)
                SetBackground(f);
        }
    }
}
