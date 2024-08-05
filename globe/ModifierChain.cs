using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Globular.Transformers {
    public class ModifierChain {
        List<GenericModifier> modifiers = new List<GenericModifier>();

        public void AddModifier(GenericModifier modifier) {
            modifiers.Add(modifier);
        }

        public void RemoveModifier(GenericModifier modifier) {
            modifiers.Remove(modifier);
        }

        public void ExecuteChain() {
            // Execute the chain of modifiers
            // Modifiers create a map of changes to the base image, then they are compiled into a layered frame
            // and applied to the base image. If no base image is present, a blank background is created.

            Image<Rgba32> base_image = modifiers[0].priority == Priority.BASE ? modifiers[0].imap : new Image<Rgba32>(100, 100);
            List<List<GenericModifier>> grouped_modifiers = ChunkModifiersByPriority(modifiers);

            // Apply the base image

            foreach (List<GenericModifier> group in grouped_modifiers) {
                Image<Rgba32> layer = new Image<Rgba32>(base_image.Width, base_image.Height);

                foreach (GenericModifier modifier in group) {
                    layer = modifier.Modify(layer);
                }

                // DRAWLAYER(layer, base_image)
            }

            // Remember ImageSharp is a library that allows for pixel manipulation in rows, which is more efficient.
        }

        // create each priority group into a layer, that cant write over each other, and then layer them on top of each other
        public void SortModifiers() {
            // Sort the modifiers based on their priority
            modifiers = modifiers.OrderBy(modifier => modifier.priority).ToList();

            if (modifiers[1].priority == Priority.BASE) {
                throw new Exception("There can only be one base modifier in the chain");
            }


        }

        public void ConjurgateModifiers() {
            // Take all the modifiers of the same type, scale them to the full image, and then apply them all to a layer in one run

            List<List<GenericModifier>> priority_groups = ChunkModifiersByPriority(this.modifiers);
            
            for(int i = 0; i < priority_groups.Count; i++){
                Console.WriteLine(priority_groups[i] + " " + priority_groups[i].Count);
            }
            


        
        }

        private List<List<GenericModifier>> ChunkModifiersByPriority(List<GenericModifier> modifiers) {
           return modifiers.GroupBy(modifier => modifier.priority).Select(group => group.ToList()).ToList();
        }


    }
    
}

