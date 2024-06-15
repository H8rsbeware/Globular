
using System.ComponentModel.Design;
using System.Numerics;
using System.Threading.Channels;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Globular.Transformers
{
    /** Priority determines the order of execution of the modifiers
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

    public struct ModiferArea{
        public int[] global_center;
        public int[] size_lengths;

        public ModiferArea(int[] g, int[] s){
            this.global_center = g;
            this.size_lengths = s;
        }
    }

    public class GenericModifier
    {
        public Priority priority = Priority.NONE;
        public ModiferArea global_impact = new ModiferArea(
            new int[]{-1,-1}, 
            new int[]{-1,-1}
            );

        public int[] row_bounds = new int[2]{-1,-1};


        public GenericModifier(int[] center, int[] box_side_lengths, Priority priority){
            this.priority = priority;
            this.global_impact = new ModiferArea(
                center,
                box_side_lengths
            );
            this.row_bounds = calculate_bounds();
        }

        public virtual Image<T> Modify<T>(Image<T> image, string outputFileName)
            where T : unmanaged, IPixel<T>
        {
            image.Save(outputFileName);
            return image;
        }

        public bool InFrame(int current_search_row)
        {

            if(this.row_bounds[0] < current_search_row 
            && current_search_row< this.row_bounds[1])
                return true;

            return false;
        }   


        private int[] calculate_bounds()
        {
            int center_row = global_impact.global_center[1];
            double row_span = global_impact.size_lengths[1] / 2;

            if(row_span % 1 != 0)
                row_span += 0.5;       

            return new int[2]{
                center_row - (int)row_span,
                center_row + (int)row_span
            };
        }

    }
}