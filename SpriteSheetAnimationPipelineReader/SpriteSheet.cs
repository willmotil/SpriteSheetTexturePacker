
// Tom Spillman and Andy Dunn
// https://channel9.msdn.com/Series/Advanced-MonoGame-for-Windows-Phone-and-Windows-Store-Games/03?term=monogame%20content%20pipeline&lang-en=true

//using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpriteSheetAnimationPipelineReader
{
    public class SpriteSheet
    {
        public string name = "None";
        public int sheetWidth = 0;
        public int sheetHeight = 0;
        public Texture2D textureSheet;
        public List<Sprite> sprites = new List<Sprite>();

        public void Add(string name, Texture2D texture, Rectangle source)
        {
            sprites.Add(new Sprite(name, texture, source));
            //return sprites[sprites.Count - 1];
        }
        public void Remove(Sprite s)
        {
            sprites.Remove(s);
        }
        public Rectangle GetSourceRectangle(Sprite s)
        {
            return s.sourceRectangle;
        }
        public Texture2D GetTexture(Sprite s)
        {
            return s.texture;
        }

        public SpriteSheet() { }

        public class Sprite
        {
            public Sprite() { }
            public Sprite(string name, Texture2D texture, Rectangle source)
            {
                nameOfSprite = name;
                this.texture = texture;
                sourceRectangle = source;
            }
            public string nameOfSprite;
            public Texture2D texture;
            public Rectangle sourceRectangle;
        }
    }
}
