
// Tom Spillman and Andy Dunn video here.
// https://channel9.msdn.com/Series/Advanced-MonoGame-for-Windows-Phone-and-Windows-Store-Games/03?term=monogame%20content%20pipeline&lang-en=true

using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using SpriteSheetXnbReader;

namespace SpriteSheetXnbPipelineCreator
{
    // We write files as a xnb file.
    [ContentTypeWriter]
    public class SpriteSheetAnimDataWriter : ContentTypeWriter<SpriteSheetAnimContent>
    {  
        protected override void Write(ContentWriter output, SpriteSheetAnimContent ss)
        {
            output.Write(ss.name);
            output.Write(ss.sheetWidth);
            output.Write(ss.sheetHeight);

            // sprites
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

            // animation set information.
            output.Write(ss.sets.Count);
            for (int i = 0; i < ss.sets.Count; i++)
            {
                output.Write(ss.sets[i].nameOfAnimation);
                output.Write(ss.sets[i].time);
                output.Write(ss.sets[i].spriteIndexs.Count);
                for (int j = 0; j < ss.sets[i].spriteIndexs.Count; j++)
                {
                    output.Write(ss.sets[i].spriteIndexs[j]);
                }
            }

            output.WriteRawObject((Texture2DContent)ss.textureSheet);           
        }
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(SpriteSheetAnimContent).AssemblyQualifiedName;
        }
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(SpriteSheetAnimReader).AssemblyQualifiedName;
        }

    }
}
