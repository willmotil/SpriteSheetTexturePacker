
using System;
using Microsoft.Xna.Framework.Content.Pipeline;

// 
// We pass the result the spritesheet to the ContentWriter to make the xnb.
//
namespace SpriteSheetPipeline
{
    /// <summary>
    /// We process Data from the ssa file in the content folder. 
    /// The content processor takes data such as sprite rectangles and textures from the importers i thinks. 
    /// public class SpriteSheetProcessor : ContentProcessor Input, Output
    /// </summary>
    [ContentProcessor(DisplayName = "SpriteSheetProcessor")]
    public class SpriteSheetProcessor : ContentProcessor<SpriteSheetContent, SpriteSheetContent>
    {
        //[DisplayName("Scale")]
        //[DefaultValue(1)]
        //[Description("Set the scale of the model.")]
        //public float Scale { get; set; }
        public SpriteSheetProcessor()
        {
            // Maybe you want to do some default preprocessing things. 
            // For example, 
            // You may add parameters to the Pipeline process. Variables that you can change in the Pipeline tool can be added here:
            // then process things here accordingly.
        }
        public override SpriteSheetContent Process(SpriteSheetContent input, ContentProcessorContext context)
        {
            try
            {
                context.Logger.LogMessage("Processing SpriteSheetAnim");
                return input;
            }
            catch (Exception ex)
            {
                context.Logger.LogMessage("Error {0}", ex);
                throw;
            }
        }
    }
}

//public override SpriteSheet Process(TextureContent input, ContentProcessorContext context)
//{
//    try
//    {
//        context.Logger.LogMessage("Processing SpriteSheet Texture");
//        var ss = new SpriteSheet();
//        var texInput = input as Texture2DContent;
//        if (texInput == null)
//            throw new NotSupportedException("Only 2d textures allowed." + this.ToString());

//        //var face = texInput.Faces[0][0] as PixelBitmapContent<Color>;

//        return ss;
//    }
//    catch (Exception ex)
//    {
//        context.Logger.LogMessage("Error {0}", ex);
//        throw;
//    }
//}

//// A SpriteSheet is created here in the processor. 
//// The input type is BinaryReader as defined by the ContentImporter 
//// The output is SpriteSheet but i still need the texture associated with it. 
//// We pass the result the spritesheet to the ContentWriter to make the xnb.
////
//public override SpriteSheet Process(TextureContent input, ContentProcessorContext context)
//{
//    try
//    {
//        context.Logger.LogMessage("Processing SpriteSheet Texture");
//        var ss = new SpriteSheet();
//        var texInput = input as Texture2DContent;
//        if (texInput == null)
//            throw new NotSupportedException("Only 2d textures allowed." + this.ToString());

//        //var face = texInput.Faces[0][0] as PixelBitmapContent<Color>;

//        return ss;
//    }
//    catch (Exception ex)
//    {
//        context.Logger.LogMessage("Error {0}", ex);
//        throw;
//    }
//}
