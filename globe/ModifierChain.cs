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
            SortModifiers();
            // Modifiers create a map of changes to the base image, then they are compiled into a layered frame
            // and applied to the base image. If no base image is present, a blank background is created.

            // Remember ImageSharp is a library that allows for pixel manipulation in rows, which is more efficient.
        }

        void SortModifiers() {
            // Sort the modifiers based on their priority
            modifiers = modifiers.OrderBy(modifier => modifier.priority).ToList();

            if (modifiers[1].priority == Priority.BASE) {
                throw new Exception("There can only be one base modifier in the chain");
            }
        }


    }
    
}

