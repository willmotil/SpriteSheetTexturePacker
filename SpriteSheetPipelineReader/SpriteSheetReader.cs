

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpriteSheetPipelineReader
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
                var sprite = new SpriteSheet.Sprite();
                sprite.nameOfSprite = input.ReadString();
                sprite.sourceRectangle = new Rectangle(input.ReadInt32(), input.ReadInt32(), input.ReadInt32(), input.ReadInt32());
                ss.sprites.Add(sprite);
            }

            // read in animation sets info.
            var setlen = input.ReadInt32();
            ss.sets = new List<SpriteSheet.Set>();
            for (int i = 0; i < setlen; i++)
            {
                SpriteSheet.Set set = new SpriteSheet.Set();
                set.nameOfAnimation = input.ReadString();
                set.time = input.ReadSingle();
                var indiceLen = input.ReadInt32();
                for (int j = 0; j < indiceLen; j++)
                {
                    set.spriteIndexs.Add(input.ReadInt32());
                }
                ss.sets.Add(set);
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
