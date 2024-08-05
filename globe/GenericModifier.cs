
using System.ComponentModel.Design;
using System.Numerics;
using System.Threading.Channels;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;


using Globular.Frame;

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
        public Image<Rgba32> imap;

        public GenericModifier(int[] center, int[] box_side_lengths, Priority priority, Image<Rgba32> imap){
            this.priority = priority;
            this.global_impact = new ModiferArea(
                center,
                box_side_lengths
            );
            this.row_bounds = calculate_bounds();
            this.imap = imap;
        }

        public virtual Image<Rgba32> Modify(Image<Rgba32> image)
        {


            return image;
        }

        public virtual FrameT Modify(FrameT frame)
        {
            Image<Rgba32> i = Modify(frame.frame.CloneAs<Rgba32>());
            frame.frame = i;
            return frame;
            
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

        public Image<Rgba32> OntoTransparent(Image<Rgba32> overlay_image, ImageInfo base_image, int[] start_position = null)
        {
            Image<Rgba32> new_image = new Image<Rgba32>(base_image.Width, base_image.Height, Rgba32.Transparent);
           
            bool center = (start_position()[0] == -1 && start_position()[1] == -1) || start_position == null;

            if(center){
                int x_difference = base_image.Width - overlay_image.Width;
                int y_difference = base_image.Height - overlay_image.Height;

                if (x_difference < 0 || y_difference < 0)
                    throw new System.IndexOutOfRangeException("Overlay image is larger than the base image");

                new_image.Mutate(x => x.DrawImage(overlay_image, new Point(x_difference / 2, y_difference / 2), 1));
                return new_image;
            }

            new_image.Mutate(x => x.DrawImage(overlay_image, new Point(start_position[0], start_position[1]), 1));
            return new_image;
        }


        public virtual int[] start_position()
        {
            return new int[2]{
                global_impact.global_center[0] - global_impact.size_lengths[0] / 2,
                global_impact.global_center[1] - global_impact.size_lengths[1] / 2
            };
        }

        public virtual bool MapFitsImage(Image<Rgba32> image)
        {
            if(image.Width >= global_impact.size_lengths[0])
                Console.WriteLine("Image is too wide");
                return false;

            if(image.Height >= global_impact.size_lengths[1])
                Console.WriteLine("Image is too tall");
                return false;
            
            if (global_impact.global_center[0] - image.Width / 2 < 0)
                Console.WriteLine("Image is too far left");
                return false;
            
            if (global_impact.global_center[0] + image.Width / 2 > image.Width)
                Console.WriteLine("Image is too far right");
                return false;
            
            if (global_impact.global_center[1] - image.Height / 2 < 0)
                Console.WriteLine("Image is too far up");
                return false;
            
            if (global_impact.global_center[1] + image.Height / 2 > image.Height)
                Console.WriteLine("Image is too far down");
                return false;
            return true;
        }



    }
}