
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Globular.Transformers
{
    /* Priority determines the order of execution of the modifiers
    * BASE: can only be one in the chain, will be executed first  
    * NONE: will be executed first, unless a base is present, thus on the lowest layer
    * LOW | MEDIUM | HIGH: will be executed in the order of their priority
    * NO_ERASE: will be executed last, and cannot be overriden by any other modifier, including itself 
    */
    public enum Priority 
    {
        BASE = -2,
        NONE = -1,
        LOW = 0,
        MEDIUM = 1,
        HIGH = 2,
        NO_ERASE = 3 // Does not allow the modifier to be overriden by any other and places last
        
    }

    public class GenericModifier
    {
        public Priority priority = Priority.NONE;

        public virtual Image<T> Modify<T>(Image<T> image, string outputFileName)
            where T : unmanaged, IPixel<T>
        {
            image.Save(outputFileName);
            return image;
        }


    }
}