//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
// monogame classes
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
// the content pipeline
//using Microsoft.Xna.Framework.Content;

// Tom Spillman and Andy Dunn video here.
// https://channel9.msdn.com/Series/Advanced-MonoGame-for-Windows-Phone-and-Windows-Store-Games/03?term=monogame%20content%20pipeline&lang-en=true


using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using SpriteSheetAnimationPipelineReader;

namespace SpriteSheetAnimationPipeline
{
    // We write files as a xnb file.
    [ContentTypeWriter]
    public class SpriteSheetDataWriter : ContentTypeWriter<SpriteSheetContent>
    {  
        protected override void Write(ContentWriter output, SpriteSheetContent ss)
        {
            output.Write(ss.name);
            output.Write(ss.sheetWidth);
            output.Write(ss.sheetHeight);
            output.Write(ss.sprites.Count);
            for (int i = 0; i < ss.sprites.Count; i++)
            {
                output.Write(ss.sprites[i].nameOfSprite);
                output.Write(ss.sprites[i].sourceRectangle.X);
                output.Write(ss.sprites[i].sourceRectangle.Y);
                output.Write(ss.sprites[i].sourceRectangle.Width);
                output.Write(ss.sprites[i].sourceRectangle.Height);
                // skip texture we only write one and it is already written.
            }
            output.WriteRawObject((Texture2DContent)ss.textureSheet);           
        }
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(SpriteSheetContent).AssemblyQualifiedName;
        }
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(SpriteSheetReader).AssemblyQualifiedName;
        }

    }
}
