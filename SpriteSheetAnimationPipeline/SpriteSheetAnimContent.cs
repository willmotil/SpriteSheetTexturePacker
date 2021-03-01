
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace SpriteSheetAnimationPipeline
{
    public class SpriteSheetAnimContent : Texture2DContent
    {
        public string name;
        public int sheetWidth = 0;
        public int sheetHeight = 0;
        public TextureContent textureSheet;
        public List<SpriteContent> sprites = new List<SpriteContent>();
    }
    public class SpriteContent
    {
        public string nameOfSprite;
        public TextureContent texture;
        public Rectangle sourceRectangle;
    }
}
