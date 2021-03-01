using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Game1_OldLoader
{
    public class AnimatedSpriteSheetCreator
    {
        GraphicsDevice graphics;

        public void MakeSpriteSheet(GraphicsDevice graphics, string sheetName, int w, int h, List<Texture2D> textures, out AnimatedSpriteSheet spriteSheet, bool saveToFile, string savepath)
        {
            MakeSpriteSheet(graphics, sheetName, w, h, textures, null, out spriteSheet, saveToFile, savepath);
        }

        public void MakeSpriteSheet(GraphicsDevice graphics, string sheetName, int w, int h, List<Texture2D> textures, List<AnimatedSpriteSheet.Set> sets, out AnimatedSpriteSheet spriteSheet, bool saveToFile, string savepath)
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
            {
                CreateSpriteSheetFromSprites(sheetName, w, h, ref spriteSheet, out ssColorArray);
                if (sets != null)
                    spriteSheet.sets = sets;
            }
            else
            {
                spriteSheet = null;
            }

            if (saveToFile)
            {
                if (spriteSheet != null)
                    Save(savepath, spriteSheet, ssColorArray, saveColorArrayInsteadOfTexture);
                else
                    Debug.Assert(false, "The SpriteSheet cant be saved as its null... the images probably don't fit in the sheet.");
            }
        }

        public void Save(string filepath, AnimatedSpriteSheet ss, Color[] ssColorArray, bool saveColorArrayInsteadOfTexture)
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
                }
                // write out animation set information.
                output.Write(ss.sets.Count);
                for (int i = 0; i < ss.sets.Count; i++)
                {
                    output.Write(ss.sets[i].nameOfAnimation);
                    output.Write(ss.sets[i].time);
                    output.Write(ss.sets[i].spriteIndexs.Count);
                    for (int j = 0; j < ss.sets[i].spriteIndexs.Count; j++)
                    {
                        output.Write(ss.sets[i].spriteIndexs[j]);
                    }
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

        public AnimatedSpriteSheet Load(string filepath, Color[] ssColorArray, bool loadColorArrayInsteadOfTexture)
        {
            AnimatedSpriteSheet ss = new AnimatedSpriteSheet();
            using (BinaryReader input = new BinaryReader(File.OpenRead(filepath)))
            {
                ss.name = input.ReadString();
                ss.sheetWidth = input.ReadInt32();
                ss.sheetHeight = input.ReadInt32();
                var len = input.ReadInt32();

                for (int i = 0; i < ss.sprites.Count; i++)
                {
                    ss.sprites.Add(new AnimatedSpriteSheet.Sprite());
                    ss.sprites[i].nameOfSprite = input.ReadString();
                    ss.sprites[i].sourceRectangle = new Rectangle(input.ReadInt32(), input.ReadInt32(), input.ReadInt32(), input.ReadInt32());
                    // skip texture we only write one and it is already written.
                }

                // read in animation sets info.
                var setlen = input.ReadInt32();
                ss.sets = new List<AnimatedSpriteSheet.Set>();
                for (int i = 0; i < setlen; i++)
                {
                    AnimatedSpriteSheet.Set n = new AnimatedSpriteSheet.Set();
                    n.nameOfAnimation = input.ReadString();
                    n.time = input.ReadSingle();
                    var indiceLen = input.ReadInt32();
                    for (int j = 0; j < indiceLen; j++)
                    {
                        n.spriteIndexs.Add(input.ReadInt32());
                    }
                }

                // texture array info.
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
        private int PrepPositionImagesIntoSpriteSheet(string sheetName, int w, int h, List<Texture2D> textureList, out AnimatedSpriteSheet spriteSheet, out Point requisiteSize)
        {
            int result = 0;
            requisiteSize = Point.Zero;
            spriteSheet = new AnimatedSpriteSheet();
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

        private void CreateSpriteSheetFromSprites(string sheetName, int w, int h, ref AnimatedSpriteSheet spriteSheet, out Color[] ssdcolorarray)
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
}
