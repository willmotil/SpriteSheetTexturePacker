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
            context.Logger.LogMessage("Importing SpriteSheet file: {0}", filename);
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

                var texfilename = ssc.name;
                //var fullFilePath = Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(texfilename));
                var fullFilePath = Path.ChangeExtension(texfilename,".png");
                context.Logger.LogMessage("Importing SpriteSheet file: {0}", fullFilePath);
                var textureContent = (Texture2DContent)texImporter.Import(fullFilePath, context);
                textureContent.Name = Path.GetFileNameWithoutExtension(fullFilePath);

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
