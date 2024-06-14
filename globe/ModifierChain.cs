using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Globular.Transformers {
    public class ModifierChain {
        List<GenericModifier> modifiers = new List<GenericModifier>();

        public void AddModifier(GenericModifier modifier) {
            // Add the modifier to the chain
        }

        public void RemoveModifier(GenericModifier modifier) {
            // Remove the modifier from the chain
        }

        public void ExecuteChain() {
            // Execute the chain of modifiers  
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

