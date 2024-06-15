using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using Globular.TempPath;


namespace Globular.Frame
{
    public class FrameGenerator
    {
        // Temp directory for the frame image, defined in TempPath namespace.
        public Temp temp;

        public FrameGenerator(Temp t)
        {
            this.temp = t;
        }

        public FrameGenerator()
        {
            this.temp = new Temp();
        }

        /* Generate a black frame at a given path with a given width and height, returns void */
        public void GenerateBlackFrameAtPath(int width, int height, string outputFileName)
        {
            using (Image<Rgba32> image = new Image<Rgba32>(width, height))
            {
                image.Mutate(x => x.BackgroundColor(Rgba32.ParseHex("000000")));
                image.Save(outputFileName);
            }
        }

        /* Generate a black frame at a given path with a given width and height, returns a Globular.Frame containing path, image, and an ok */
        public Frame GenerateBlankFrame(int width, int height)
        {
            using (Image<Rgba32> image = new Image<Rgba32>(width, height))
            {
                
                string path = temp.ConcatWith("frame.png");
                image.Mutate(x => x.BackgroundColor(Rgba32.ParseHex("000000")));
                image.Save(path);
                

                return new Frame(
                    image.Clone(),
                    path,
                    true
                );
                
            }
        }
    }

    /**
    *  Frame is a class that contains an image, a path, and a boolean ok
    *  if ok if false then the frame is not valid
    */
    public class Frame {
        public Image<Rgba32> frame;
        public string path;
        public bool ok;

        public Frame(Image<Rgba32> frame, string path, bool ok)
        {
            this.frame = frame;
            this.path = path;
            this.ok = ok;
        }
    }
    

}