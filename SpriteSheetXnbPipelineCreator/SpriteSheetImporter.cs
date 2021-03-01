//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using System.Xml.Linq;
// monogame classes

//using Microsoft.Xna.Framework.Graphics;
//// the content pipeline
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.Content.Pipeline;
//using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
//using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
//using SpriteSheetData;

using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace SpriteSheetXnbPipelineCreator
{
    ///
    /// ContentImporter how we tell the processor to import files from the content folder.
    /// ContentImporter(".spr",  This tells the Pipeline tool to use this content importer for files with the .spr extension which ill be generating. 
    /// It also tells the Pipeline tool to use SpriteSheetProcessor as the default processor.
    ///
    [ContentImporter(".spr", DefaultProcessor = "SpriteSheetProcessor", DisplayName = "SpriteSheetImporter")]
    public class SpriteSheetImporter : ContentImporter<SpriteSheetContent>
    {
        public override SpriteSheetContent Import(string filename, ContentImporterContext context)
        {
            // Debug line that shows up in the Pipeline tool or your build output window.
            context.Logger.LogMessage("Importing SpriteSheet .spr file: {0}", filename);
            SpriteSheetContent ssc = new SpriteSheetContent();

            using (BinaryReader input = new BinaryReader(File.OpenRead(filename)))
            {
                ssc.name = input.ReadString();
                ssc.sheetWidth = input.ReadInt32();
                ssc.sheetHeight = input.ReadInt32();
                int spritesLength = input.ReadInt32();
                for (int i = 0; i < spritesLength; i++)
                {
                    var s = new SpriteContent();
                    s.nameOfSprite = input.ReadString();
                    s.sourceRectangle = new Rectangle(input.ReadInt32(), input.ReadInt32(), input.ReadInt32(), input.ReadInt32());
                    ssc.sprites.Add(s);
                }

                TextureImporter texImporter = new TextureImporter();

                // this is a little messed up cause i tried to use the name so it could be seperate but id have to fiddle with the filepaths and it really doesn't make sense just leave it as one texture with the same name as the sheet for now.
                //var texfilename = ssc.name;
                //var fullFilePath = Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(texfilename));
                //var pngfilename = Path.ChangeExtension(texfilename,".png");

                var pngfilename = Path.ChangeExtension(filename, ".png");
                context.Logger.LogMessage("Importing SpriteSheet .png file: {0}", pngfilename);
                var textureContent = (Texture2DContent)texImporter.Import(pngfilename, context);
                textureContent.Name = Path.GetFileNameWithoutExtension(pngfilename);

                ssc.textureSheet = textureContent;

                for (int i = 0; i < spritesLength; i++)
                {
                    ssc.sprites[i].texture = ssc.textureSheet;
                }
            }
            return ssc;
        }
    }
}
