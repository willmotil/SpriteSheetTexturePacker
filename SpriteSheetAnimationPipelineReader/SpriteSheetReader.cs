//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Xml.Serialization;

//using Microsoft.Xna.Framework.Content;
////using Microsoft.Xna.Framework.Content.Pipeline;
////using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
//using SpriteSheetData;

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpriteSheetAnimationPipelineReader
{
    // We read in the xnb.
    public class SpriteSheetReader : ContentTypeReader<SpriteSheet>
    {
        protected override SpriteSheet Read(ContentReader input, SpriteSheet existingInstance)
        {
            SpriteSheet ss = new SpriteSheet();

            ss.name = input.ReadString();
            ss.sheetWidth = input.ReadInt32();
            ss.sheetHeight = input.ReadInt32();
            int spritesLength = input.ReadInt32();
            for (int i =0;i< spritesLength; i++)
            {
                var s = new SpriteSheet.Sprite();
                s.nameOfSprite = input.ReadString();
                s.sourceRectangle = new Rectangle(input.ReadInt32(), input.ReadInt32(), input.ReadInt32(), input.ReadInt32());
                ss.sprites.Add(s);
            }
            // from nkasts ex.
            IGraphicsDeviceService graphicsDeviceService = (IGraphicsDeviceService)input.ContentManager.ServiceProvider.GetService(typeof(IGraphicsDeviceService));
            var device = graphicsDeviceService.GraphicsDevice;
            Texture2D sst = new Texture2D(device, ss.sheetWidth, ss.sheetHeight);
            sst = ReadTexture2D(input, sst); //input.ReadRawObject<Texture2D>();
            ss.textureSheet = sst;
            for (int i = 0; i < spritesLength; i++)
            {
                ss.sprites[i].texture = sst;
            }
            return ss;
        }
        // nkasts read method
        private Texture2D ReadTexture2D(ContentReader input, Texture2D existingInstance)
        {
            Texture2D output = null;
            try
            {
                output = input.ReadRawObject<Texture2D>(existingInstance);
            }
            catch (NotSupportedException)
            {
                var assembly = typeof(Microsoft.Xna.Framework.Content.ContentTypeReader).Assembly;
                var texture2DReaderType = assembly.GetType("Microsoft.Xna.Framework.Content.Texture2DReader");
                var texture2DReader = (ContentTypeReader)Activator.CreateInstance(texture2DReaderType, true);
                output = input.ReadRawObject<Texture2D>(texture2DReader, existingInstance);
            }
            return output;
        }
    }
}
