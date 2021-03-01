using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Game1_OldLoader
{
    public class AnimatedSpriteSheet
    {
        public string name = "None";
        public int sheetWidth = 0;
        public int sheetHeight = 0;
        public Texture2D textureSheet;
        public List<Sprite> sprites = new List<Sprite>();
        public List<Set> sets = new List<Set>();

        public void Add(string name, Texture2D texture, Rectangle source)
        {
            sprites.Add(new Sprite(name, texture, source));
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

        public AnimatedSpriteSheet() { }

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

        public class Set
        {
            public string nameOfAnimation = "";
            public float time = .25f;
            public List<int> spriteIndexs = new List<int>();
        }

    }
}
