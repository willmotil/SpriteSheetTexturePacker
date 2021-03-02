
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace SpriteSheetXnbPipelineCreator
{
    public class SpriteSheetAnimContent : Texture2DContent
    {
        public string name;
        public int sheetWidth = 0;
        public int sheetHeight = 0;
        public TextureContent textureSheet;
        public List<SpriteContent> sprites = new List<SpriteContent>();
        public List<SetContent> sets = new List<SetContent>();
    }
    public class SpriteContent
    {
        public string nameOfSprite;
        public TextureContent texture;
        public Rectangle sourceRectangle;
    }

    public class SetContent
    {
        public string nameOfAnimation = "";
        public float time = .25f;
        public List<int> spriteIndexs = new List<int>();
    }
}
