//using System;

//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using System.IO;
//using System.Xml.Linq;
// monogame classes

//using Microsoft.Xna.Framework.Graphics;
//// the content pipeline
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.Content.Pipeline;

//using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
//using SpriteSheetData;

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace SpriteSheetAnimationPipeline
{
    public class SpriteSheetContent : Texture2DContent
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
