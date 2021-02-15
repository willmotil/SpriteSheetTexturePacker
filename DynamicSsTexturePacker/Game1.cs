using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using SpriteSheetXnbReader;


// references for additional extras to make this pipelinable...
// Tom Spillman and Andy Dunn
// https://channel9.msdn.com/Series/Advanced-MonoGame-for-Windows-Phone-and-Windows-Store-Games/03?term=monogame%20content%20pipeline&lang-en=true
// My post here for how to make a processor importer for this and a little run thru of some of the troubles i had doing it.
// https://community.monogame.net/t/solved-content-importer-processor-how-to-process-seperate-files-into-one-xnb/12040/6

namespace DynamicSsTexturePacker
{

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        MouseState mouseState;

        // Were we will place textures that will be packed.
        List<Texture2D> textures = new List<Texture2D>();

        // Does the conversion.
        SpriteSheetCreator ssCreator;

        // A little class that encapsulates things that are related to stuff that are specific to a spritesheet like rectangles in it and stuff.
        SpriteSheet myGeneratedSpriteSheetInstance;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
            this.IsMouseVisible = true;
        }

        protected override void Initialize(){   base.Initialize();}

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("MgFont");

            // Well get the textures.
            textures.Add(LoadTexture("MonoGameLogoSpliffedup"));
            textures.Add(LoadTexture("TestOutlineImage"));
            textures.Add(LoadTexture("filterTestImage"));
            textures.Add(LoadTexture("sphereImage"));

            // This class is responsible for converting things to or from images to a spritesheet ect.
            ssCreator = new SpriteSheetCreator();

            string savepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "spriteSheetTest01.spr");
            ssCreator.MakeSpriteSheet(GraphicsDevice, "mySpriteSheet", 1024, 1024, textures, out myGeneratedSpriteSheetInstance, true, savepath);

            // open the directory were we saved the image and descriptor to.
            //Process.Start(Path.GetDirectoryName(savepath));
        }

        public Texture2D LoadTexture( string FileName)
        {
            return Content.Load<Texture2D>(FileName);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            mouseState = Mouse.GetState();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            // Draw the individual textures.
            for (int i = 0; i < textures.Count; i++)
            {
                spriteBatch.Draw(textures[i], new Rectangle(10 + (i * 100), 10, 100, 100), Color.White);
                spriteBatch.DrawString(font, textures[i].Name, new Vector2(10 + (i * 100), (i * 10) + 10), Color.White);
            }

            // Draw the resulting spritesheet.
            var offset = new Vector2(50, 250);
            spriteBatch.Draw(myGeneratedSpriteSheetInstance.textureSheet, offset, Color.White);

            // Draw the names of the sprites in the sheet at their locations allow color change over sprites.
            for (int i = 0; i < myGeneratedSpriteSheetInstance.sprites.Count; i++)
            {
                var spriteName = myGeneratedSpriteSheetInstance.sprites[i].nameOfSprite;
                var nameoffset = myGeneratedSpriteSheetInstance.sprites[i].sourceRectangle;
                nameoffset.Location = nameoffset.Location + offset.ToPoint();
                var color = Color.White;
                if (nameoffset.Contains(mouseState.Position))
                    color = Color.Red;
                spriteBatch.DrawString(font, spriteName, nameoffset.Center.ToVector2(), color);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        //protected override void Draw(GameTime gameTime)
        //{
        //    GraphicsDevice.Clear(Color.CornflowerBlue);

        //    spriteBatch.Begin();

        //    // Draw the individual textures.
        //    for (int i = 0; i < textures.Count; i++)
        //        spriteBatch.Draw(textures[i], new Rectangle(10 + (i * 100),  10 , 100, 100), Color.White);

        //    // Draw the resulting spritesheet.
        //    var offset = new Vector2(50, 250);
        //    spriteBatch.Draw(myGeneratedSpriteSheetInstance.textureSheet, offset, Color.White);

        //    spriteBatch.End();

        //    base.Draw(gameTime);
        //}

    }


    public class SpriteSheetCreator
    {
        GraphicsDevice graphics;

        public void MakeSpriteSheet(GraphicsDevice graphics, string sheetName, int w, int h, List<Texture2D> textures, out SpriteSheet spriteSheet, bool saveToFile, string savepath)
        {
            bool saveColorArrayInsteadOfTexture = false;
            this.graphics = graphics;
            Color[] ssColorArray = new Color[0];
            var len = textures.Count;
            var textureList = GetSortedTextureListLargestToSmallest(textures);
            Point requisiteSize = Point.Zero;
            var amountAdded = PrepPositionImagesIntoSpriteSheet(sheetName, w, h, textureList, out spriteSheet, out requisiteSize);
            Console.WriteLine(" AmountAdded " + len + " of " + len);
            Console.WriteLine(" RequisiteSize " + requisiteSize);
            //spriteSheet.sprites = splist;
            if (amountAdded == len)
                CreateSpriteSheetFromSprites(sheetName, w, h, ref spriteSheet, out ssColorArray);
            else
                spriteSheet = null;

            if (saveToFile)
                Save(savepath, spriteSheet, ssColorArray, saveColorArrayInsteadOfTexture);
        }

        public void Save(string filepath, SpriteSheet ss, Color[] ssColorArray, bool saveColorArrayInsteadOfTexture)
        {
            using (BinaryWriter output = new BinaryWriter(File.Create(filepath)))
            {
                output.Write(ss.name);
                output.Write(ss.sheetWidth);
                output.Write(ss.sheetHeight);
                output.Write(ss.sprites.Count);
                //output.WriteRawObject<Texture2D>(ss.textureSheet);
                for (int i = 0; i < ss.sprites.Count; i++)
                {
                    output.Write(ss.sprites[i].nameOfSprite);
                    output.Write(ss.sprites[i].sourceRectangle.X);
                    output.Write(ss.sprites[i].sourceRectangle.Y);
                    output.Write(ss.sprites[i].sourceRectangle.Width);
                    output.Write(ss.sprites[i].sourceRectangle.Height);
                    // skip texture we only write one and it is already written.
                }
                if (saveColorArrayInsteadOfTexture)
                {
                    for (int i = 0; i < ssColorArray.Length; i++)
                    {
                        output.Write(ssColorArray[i].R);
                        output.Write(ssColorArray[i].G);
                        output.Write(ssColorArray[i].B);
                        output.Write(ssColorArray[i].A);
                    }
                }
                else
                {
                    filepath = Path.ChangeExtension(filepath, ".png");
                    using (var fs = File.Create(filepath))
                    {
                        ss.textureSheet.SaveAsPng(fs, ss.textureSheet.Width, ss.textureSheet.Height);
                    }
                }
            }
        }

        public SpriteSheet Load(string filepath, Color[] ssColorArray, bool loadColorArrayInsteadOfTexture)
        {
            SpriteSheet ss = new SpriteSheet();
            using (BinaryReader input = new BinaryReader(File.OpenRead(filepath)))
            {
                ss.name = input.ReadString();
                ss.sheetWidth = input.ReadInt32();
                ss.sheetHeight = input.ReadInt32();
                var len = input.ReadInt32();

                for (int i = 0; i < ss.sprites.Count; i++)
                {
                    ss.sprites.Add(new SpriteSheet.Sprite());
                    ss.sprites[i].nameOfSprite = input.ReadString();
                    ss.sprites[i].sourceRectangle = new Rectangle(input.ReadInt32(), input.ReadInt32(), input.ReadInt32(), input.ReadInt32());
                    // skip texture we only write one and it is already written.
                }
                Color[] cols = new Color[len];
                if (loadColorArrayInsteadOfTexture)
                {
                    for (int i = 0; i < len; i++)
                        cols[i] = new Color(input.ReadByte(), input.ReadByte(), input.ReadByte(), input.ReadByte());
                    ss.textureSheet = new Texture2D(graphics, ss.sheetWidth, ss.sheetHeight);
                    ss.textureSheet.SetData(cols);
                }
                else
                {
                    filepath = Path.ChangeExtension(filepath, ".png");
                    using (var fs = File.Create(filepath))
                    {
                        ss.textureSheet = Texture2D.FromStream(graphics, fs);
                    }
                }
            }
            return ss;
        }

        /// <summary>
        /// Returns the index that was not placeable.
        /// </summary>
        private int PrepPositionImagesIntoSpriteSheet(string sheetName, int w, int h, List<Texture2D> textureList, out SpriteSheet spriteSheet, out Point requisiteSize)
        {
            int result = 0;
            requisiteSize = Point.Zero;
            spriteSheet = new SpriteSheet();
            spriteSheet.sheetWidth = w;
            spriteSheet.sheetHeight = h;
            spriteSheet.name = sheetName;
            Rectangle spriteSheetBounds = new Rectangle(0, 0, w, h);
            for (int i = 0; i < textureList.Count; i++)
            {
                var t = textureList[i];
                var bounds = textureList[i].Bounds;
                bool successResult = false;

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        bool placeFound = true;
                        bounds.X = x;
                        bounds.Y = y;
                        if (spriteSheet.sprites.Count > 0)
                        {
                            // check to make sure we intersect no sprites.
                            for (int si = 0; si < spriteSheet.sprites.Count; si++)
                            {
                                var alreadyPlacedSpriteBounds = spriteSheet.sprites[si].sourceRectangle;
                                alreadyPlacedSpriteBounds.Location = alreadyPlacedSpriteBounds.Location - new Point(2, 2);
                                alreadyPlacedSpriteBounds.Size = alreadyPlacedSpriteBounds.Size + new Point(4, 4);

                                bool isInsideSpriteSheet = bounds.Left >= 0 && bounds.Top >= 0 && bounds.Right < spriteSheetBounds.Right && bounds.Bottom < spriteSheetBounds.Bottom;
                                //if (spriteBounds.Intersects(bounds) || spriteBounds.Contains(bounds) || isInsideSpriteSheet)
                                if (IsNotOverlapping(alreadyPlacedSpriteBounds, bounds) == false || isInsideSpriteSheet == false)
                                {
                                    si = spriteSheet.sprites.Count;
                                    placeFound = false;
                                }
                            }
                            if (placeFound)
                            {
                                // in this case its a good position to add.
                                successResult = true;
                                placeFound = true;
                                //if (t.Name != null)
                                //    tmpSprite.nameOfSprite = t.Name;
                                spriteSheet.Add(t.Name, t, bounds);
                                // break all the way out to the next texture to find a place.
                                x = w;
                                y = h;
                                result++;
                                if (bounds.Right > requisiteSize.X)
                                    requisiteSize.X = bounds.Right + 1;
                                if (bounds.Bottom > requisiteSize.Y)
                                    requisiteSize.Y = bounds.Bottom + 1;
                                var s = spriteSheet.sprites[spriteSheet.sprites.Count - 1];
                                Console.WriteLine(" " + i + "  Sprite.Name " + s.nameOfSprite + " Sprite.source " + s.sourceRectangle);
                            }
                        }
                        else
                        {
                            // no sprites to check against so just check if it fits inside.
                            bool isInside = bounds.Left >= 0 && bounds.Top >= 0 && bounds.Right < spriteSheetBounds.Right && bounds.Bottom < spriteSheetBounds.Bottom;
                            if (isInside)
                            {
                                // in this case its a good position to add.
                                successResult = true;
                                placeFound = true;
                                spriteSheet.Add(t.Name, t, bounds);
                                if (bounds.Right > requisiteSize.X)
                                    requisiteSize.X = bounds.Right + 1;
                                if (bounds.Bottom > requisiteSize.Y)
                                    requisiteSize.Y = bounds.Bottom + 1;
                                // break all the way out to the next texture to find a place for.
                                x = w;
                                y = h;
                                result++;
                                var s = spriteSheet.sprites[spriteSheet.sprites.Count - 1];
                                Console.WriteLine(" 0 Sprite.Name " + s.nameOfSprite + " Sprite.source " + s.sourceRectangle);
                            }
                            else
                            {
                                placeFound = false;
                            }
                        }
                    }
                }
                if (successResult == false)
                {
                    // In this case we are in sort of a problem area we can't fit this texture so we really should just stop.
                    // We could get more fancy if we knew for sure that it was ok to save this for another texture but we dont.
                    // break out of the checking loop and the method.
                    //result = i;
                    i = textureList.Count;
                }
            }
            return result;
        }

        public List<Texture2D> GetSortedTextureListLargestToSmallest(List<Texture2D> textures)
        {
            List<Texture2D> textureList = new List<Texture2D>();
            for (int j = 0; j < textures.Count; j++)
            {
                if (textureList.Count == 0)
                    textureList.Add(textures[0]);
                else
                {
                    var t0len = textures[j].Width * textures[j].Height;
                    int elementToAddTo = textureList.Count;
                    for (int k = 0; k < textureList.Count; k++)
                    {
                        var t1len = textureList[k].Width * textureList[k].Height;
                        if (t0len > t1len)
                        {
                            elementToAddTo = k;
                            k = textureList.Count;
                        }
                    }
                    if (elementToAddTo >= textureList.Count)
                        textureList.Add(textures[j]);
                    else
                        textureList.Insert(elementToAddTo, textures[j]);
                }
            }
            return textureList;
        }

        private void CreateSpriteSheetFromSprites(string sheetName, int w, int h, ref SpriteSheet spriteSheet, out Color[] ssdcolorarray)
        {
            Color[] sscolor = new Color[w * h];
            for (int i = 0; i < spriteSheet.sprites.Count; i++)
            {
                var r = spriteSheet.sprites[i].sourceRectangle;
                var t = spriteSheet.sprites[i].texture;
                var scol = GetData(t);
                int sy = 0;
                for (int y = r.Top; y < r.Bottom; y++)
                {
                    int sx = 0;
                    for (int x = r.Left; x < r.Right; x++)
                    {
                        sscolor[y * w + x] = scol[sy * t.Width + sx];
                        sx++;
                    }
                    sy++;
                }
            }
            Texture2D tmp = new Texture2D(graphics, w, h);
            tmp.SetData(sscolor);
            spriteSheet.textureSheet = tmp;
            for (int j = 0; j < spriteSheet.sprites.Count; j++)
                spriteSheet.sprites[j].texture = tmp;
            ssdcolorarray = sscolor;
        }
        private Color[] GetData(Texture2D t)
        {
            Color[] colors = new Color[t.Width * t.Height];
            t.GetData<Color>(colors);
            return colors;
        }

        public bool IsNotOverlapping(Rectangle A, Rectangle B)
        {
            if ((A.Right < B.Left || A.Left > B.Right) || (A.Bottom < B.Top || A.Top > B.Bottom))
                return true;
            else
                return false;
        }
        public bool IsOverlapping(Rectangle A, Rectangle B)
        {
            if ((A.Right < B.Left || A.Left > B.Right) || (A.Bottom < B.Top || A.Top > B.Bottom))
                return false;
            else
                return true;
        }
    }


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
