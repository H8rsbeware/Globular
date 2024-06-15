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
        // Temp directory for the background image.
        Temp path; 
        // Default temp-background image name. TODO: attach some random string to the name.
        static string name = "bg-image.png";

        public BackgroundSetter(Temp p)
        {
            // Create the temp directory.
            path = p;
        }

        public BackgroundSetter()
        {
            // Create the temp directory.
            path = new Temp();
        }

        // These import is used to set the desktop background image.
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SystemParametersInfo(UInt32 uiAction, UInt32
        uiParam, String pvParam, UInt32 fWinIni);
        private static UInt32 SPI_SETDESKWALLPAPER = 20;
        private static UInt32 SPIF_UPDATEINIFILE = 0x1;

        /**
        * SetBackground sets the desktop background image, with one of 3 input types:
        * 1. Image<Rgba32> - an image object, defined in the SixLabors.ImageSharp(.PixelFormats) namespace.
        * 2. string - a file path to an image.
        * 3. Frame.Frame - a frame object, defined in the FrameHandlers.Frame namespace.
        */

        public void SetBackground(Image<Rgba32> image)
        {
            image.Save(path.ConcatWith(name));

            // Set the desktop background image, using the SystemParametersInfo function, defined in System.Runtime.InteropServices.
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
