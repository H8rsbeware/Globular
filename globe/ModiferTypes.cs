

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Processing.Processors;
using SixLabors.ImageSharp.Memory;
using Globular.Frame;

namespace Globular.Transformers
{
    public class OverwriteModifier : GenericModifier{

        public OverwriteModifier(int[] center, int[] box_side_lengths, Priority priority, Image<Rgba32> image) 
        : base(center, box_side_lengths, priority, image)
        {
        }

        public override Image<Rgba32> Modify(Image<Rgba32> image)
        {
            using (Image<Rgba32> map = this.imap)
            {
                if(!this.MapFitsImage(image)){
                    Console.WriteLine(this + " rejected image modification, map is too large or not framed properly");
                    return image;
                }

                int[] start = this.start_position();

                image = OntoTransparent(image, map, start);

                image.Mutate(x => x
                    .DrawImage(map, new Point(start[0], start[1]), 1f)
                );
                return image;
            }

        }

    }
 
    public class AlteringModifier : GenericModifier
    {
        public AlteringModifier(int[] center, int[] box_side_lengths, Priority priority, Image<Rgba32> map) 
        : base(center, box_side_lengths, priority, map)
        {
        }

        public override Image<Rgba32> Modify(Image<Rgba32> image)
        {
            return image;
        }

        private Image<Rgba32> Cast()
        {
            return this.imap.CloneAs<Rgba32>();
        }

    }



    public class LuminancyModifier : AlteringModifier
    {
        public LuminancyModifier(int[] center, int[] box_side_lengths, Priority priority, Image<Rgba32> map) 
        : base(center, box_side_lengths, priority, map)
        {
        }
        public override Image<Rgba32> Modify(Image<Rgba32> image)
        {
            if(!this.MapFitsImage(image)){
                Console.WriteLine(this + " rejected image modification, map is too large or not framed properly");
                return image;
            }

            image = OntoTransparent(image, this.imap, this.start_position());
            Configuration configuration = Configuration.Default;

            LuminancyRowOperation operation = new LuminancyRowOperation(
                configuration,
                in this.imap.CloneAs<Rgba32>(),
                in image
            );
            
            var interest = Rectangle.Intersect(
                this.imap.Bounds, destination.Bounds
            );

            ParallelRowIterator.IterateRows<LuminancyRowOperation>(
                configuration,
                interest,
                in operation
            );

            return image;
        }

        private readonly struct LuminancyRowOperation : IRowOperation{
            private readonly Buffer2D<Rgba32> source;
            private readonly Buffer2D<Rgba32> destination;
            private readonly Configuration configuration;


            public LuminancyRowOperation(Configuration configuration, Buffer2D<Rgba32> source, Buffer2D<Rgba32> destination)
            {
                this.configuration = configuration;
                this.source = source;
                this.destination = destination;
            }

            public void Invoke(int y)
            {
                Span<Rgba32> sourceRow = this.source.DangerousGetRowSpan(y);
                Span<Rgba32> destinationRow = this.destination.DangerousGetRowSpan(y);

                for(int x = 0; x < destinationRow.Length; x++)
                {
                    var dp = destinationRow[x];
                    destinationRow[x] = new Rgba32(dp.R, dp.G, dp.B, sourceRow[x].A);
                }
            }
        }
    }
    
    public class LayerDrawModifier : OverwriteModifier{
        public LayerDrawModifier(int[] center, int[] box_side_lengths, Priority priority, Image<Rgba32> image) 
        : base(center, box_side_lengths, priority, image)
        {
        }

        public override Image<Rgba32> Modify(Image<Rgba32> image)
        {

            using (Image<Rgba32> map = this.imap)
            {
                // memomoise the image fit, and check if it every changes. If it doesnt, this check is more expensive and can be avoided
                if(!this.MapFitsImage(image)){
                    Console.WriteLine(this + " rejected image modification, map is too large or not framed properly");
                    return image;
                }

                int[] start = this.start_position();

                image = OntoTransparent(image, in map, start);

                image.Mutate(x => x
                    .DrawImage(map, new Point(start[0], start[1]), 1f)
                );
                return image;
            }

        }

    }
}
